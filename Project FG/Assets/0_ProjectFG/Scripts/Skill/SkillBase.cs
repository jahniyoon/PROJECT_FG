using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;

namespace JH
{
    public class SkillBase : MonoBehaviour
    {

        [Header("Data")]
        [SerializeField] protected SkillData m_data;
        [SerializeField] protected LevelData m_skillLevelData;

        [Header("Skill Caster")]
        [SerializeField] protected ISkillCaster m_skillCaster;      //  스킬의 캐스터 정보
        [Header("Skill State")]
        [SerializeField] private bool m_freeze;                     // 스킬 사용가능한 상태
        [SerializeField] private bool m_fixed;                     // 스킬의 캐스터가 정지해야할 때
        [SerializeField] protected SkillState m_state;               // 스킬 상태
        [SerializeField] protected bool m_routine;               // 스스로 쿨이 돌면 사용한다.

        [Header("Target")]
        [SerializeField] protected Transform m_skillTarget;
        [SerializeField] protected bool m_isAlwaysShoot = true;               // 스스로 쿨이 돌면 사용한다.
        protected Collider[] m_scanColls;

        [Header("CoolDown")]
        [SerializeField] protected float m_skillCoolDown;           // 쿨타임
        [SerializeField] protected float m_skillCoolDownTimer;      // 현재 타이머
        [SerializeField] private float m_duration;                  // 지속시간

        [Header("Buff")]
        [SerializeField] protected List<BuffBase> m_buffs = new List<BuffBase>();

        [Header("Projectile")]
        [SerializeField] protected List<ProjectileBase> m_projectiles = new List<ProjectileBase>();
        [SerializeField] protected List<ProjectileBase> m_activeProjectiles = new List<ProjectileBase>();

        [Header("Effects")]
        [SerializeField] protected List<ParticleSystem> m_particles = new List<ParticleSystem>();
        [SerializeField] protected List<VisualEffect> m_visualEffects = new List<VisualEffect>();

        [Header("Event")]
        [HideInInspector] public UnityEvent CastEvent = new UnityEvent();
        [HideInInspector] public UnityEvent ActiveEvent = new UnityEvent();
        [HideInInspector] public UnityEvent InactiveEvent = new UnityEvent();


        Coroutine m_skillRoutine;
        private bool m_init;

        #region Property
        public bool IsActive => m_state == SkillState.Active;
        public bool IsFixed => m_fixed;
        public SkillData Data => m_data;
        public LevelData LevelData => m_skillLevelData;
        public ISkillCaster Caster => m_skillCaster;
        public int ID => m_data.ID;
        public Transform Target => m_skillTarget;
        public List<BuffBase> Buffs => m_buffs;
        public float Duration => m_duration;
        public float CoolDown => m_skillCoolDown;
        public SkillState State => m_state;
        #endregion

        #region LifeCycle
        private void Awake()
        {
            m_scanColls = new Collider[30];
            AwakeInit();
        }

        protected virtual void AwakeInit()
        {

        }
        private void Update()
        {
            UpdateBehavior();
        }
        #endregion

        #region Init
        // 스킬 초기화. 캐스터를 정해준다.
        public void SkillInit(ISkillCaster caster, bool routine = false, LevelData levelData = default)
        {
            // 캐스터를 가져온다.
            m_skillCaster = caster;

            // 레벨데이터를 세팅한다.
            if (levelData == default)
                SetLevelData(m_data.LevelData);

            // 듀레이션과 쿨타임을 가져온다.
            SetDuration(LevelData.Duration);
            SetCoolDown();


            // 스킬에 버프를 장착한다.
            m_buffs.Clear();
            for (int i = 0; i < Data.BuffID.Length; i++)
            {
                BuffBase buff = GFunc.TryGetBuff(Data.BuffID[i]);
                if (buff == null) continue;

                buff.SetCaster(m_skillCaster, Caster.Transform);
                buff.SetBuffValue(LevelData.TryGetBuffValues(i));
                m_buffs.Add(buff);
            }

            // 스킬에 투사체를 장착한다.
            m_projectiles.Clear();
            for (int i = 0; i < Data.ProjectileID.Length; i++)
            {
                ProjectileBase projectile = GFunc.GetProjectilePrefab(Data.ProjectileID[i]);
                if (projectile == null) continue;
                m_projectiles.Add(projectile);
            }
            ProjectileInit();

            m_routine = routine;

            m_init = true;
            Init();
        }

        // 스킬 초기화
        protected virtual void Init() { }

        #endregion Init


        // 스킬에 업데이트가 필요한 부분
        protected virtual void UpdateBehavior()
        {
            // 쿨타임이 계속 돌아간다.
            m_skillCoolDownTimer = m_skillCoolDownTimer + Time.deltaTime < m_skillCoolDown ? m_skillCoolDownTimer + Time.deltaTime : m_skillCoolDown;

            if (m_skillCoolDown <= m_skillCoolDownTimer && State == SkillState.Reloading && m_freeze == false)
                SetState(SkillState.Ready);

            // 사용 가능하면 스킬을 캐스팅한다.
            if (CheckCondition())
                CastSkill();
        }

        #region SkillBase

        // 스킬이 사용한 조건을 체크한다.
        protected virtual bool CheckCondition()
        {
            // 캐스터가 없으면 패스
            if (Caster == null) return false;

            // 준비가 되어있지 않으면 패스
            if (State != SkillState.Ready)
                return false;

            // 루틴 스킬이면 캐스터 체크를 안해도 된다.
            if (m_routine)
                return true;


            // 캐스터의 상태조건을 체크하고, 쿨타임이 되어야한다.
            return Caster.CanActiveSkill();
        }


        /// <summary>
        /// 스킬 캐스팅.  
        /// 캐스팅을 한 후 스킬의 딜레이 이후에 스킬이 실행된다.
        /// 캐스팅 이후 스킬이 막혀 스킬을 사용하지 못하면 스킬은 캔슬된다.
        /// </summary>
        public virtual void CastSkill()
        {
            //if (this.gameObject.activeInHierarchy == false || m_isCast) return;
            SetState(SkillState.Cast);

            // 쿨타임을 리셋한다.
            ResetTimer();

            // 스킬 캐스터의 스킬 타이머 업데이트
            m_skillCaster.UpdateSkillTimer(Data.SkillSpeed);

            // 스킬 지속시간 이후 꺼지도록 루틴 시작
            if (m_skillRoutine != null)
            {
                StopCoroutine(m_skillRoutine);
                m_skillRoutine = null;
            }
            m_skillRoutine = StartCoroutine(ActiveSkillRoutine());

            CastEvent?.Invoke();
        }

        // 스킬 활성화
        public virtual void ActiveSkill()
        {
            SetState(SkillState.Active);


            ActiveEvent?.Invoke();
        }



        // 스킬 비활성화
        public virtual void InactiveSkill()
        {
            if (m_skillRoutine != null)
                StopCoroutine(m_skillRoutine);

            SetState(SkillState.Reloading);
            InactiveEvent?.Invoke();

        }
        // 스킬 삭제
        public virtual void RemoveSkill()
        {
            for (int i = 0; i < m_activeProjectiles.Count; i++)
            {
                if (m_activeProjectiles[i] == null) continue;

                m_activeProjectiles[i].InActiveProjectile();
                Destroy(m_activeProjectiles[i].gameObject);
            }
            SetState(SkillState.Disable);
        }
        // 스킬 지속시간동안 이뤄지는 루틴
        protected virtual IEnumerator ActiveSkillRoutine()
        {
            float timer = 0;

            while (timer < m_data.SkillDelay)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            timer = 0;
            ActiveSkill();

            // 듀레이션이 음수면 자동으로 안끈다.
            if (m_duration < 0)
                yield break;

            while (timer < m_duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            InactiveSkill();
            yield break;
        }
        #endregion SkillBase


        // 쿨타임을 초기화한다.
        protected void ResetTimer()
        {
            m_skillCoolDownTimer = 0;
        }
        protected void SetState(SkillState nextState)
        {
            // 비활성화가 되어있으면 다시 활성화 불가
            if (m_state == SkillState.Disable) return;

            m_state = nextState;
        }

        public void FreezeSkill(bool enable = true)
        {
            m_freeze = enable;
            InactiveSkill();
        }

        // 스킬의 지속시간을 동적으로 변경한다.
        protected void SetDuration(float duration)
        {
            m_duration = duration;
        }
        protected void SetCoolDown()
        {
            m_skillCoolDown = LevelData.CoolDown;

            // 스킬을 즉시 시작 가능하게 할지 말지 정해진다.
            if (m_init == false)
            {
                m_skillCoolDownTimer = m_skillCoolDown;
                if (Data.ActiveTime == SkillActiveTime.CoolDown || Data.ActiveTime == SkillActiveTime.CoolDownReset)
                    ResetTimer();
                return;
            }

            // 리셋 타이머의 경우에만 리셋한다.
            if (Data.ActiveTime == SkillActiveTime.ActiveReset || Data.ActiveTime == SkillActiveTime.CoolDownReset)
                ResetTimer();
        }

        // 스킬이 고정형인지 체크한다. 캐스터는 이 값을 참조해 멈춰야하는지 확인할 수 있다.
        protected void SetSkillFix(bool enable = true)
        {
            m_fixed = enable;
        }
        // 스킬의 레벨 데이터를 수정한다.
        protected void SetLevelData(LevelData levelData)
        {
            m_skillLevelData = levelData;
        }



        #region BUFF
        // 타겟에 스킬의 버프를 붙인다.
        public void OnBuff(Transform target)
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                // 지속형이면 붙일 필요가 없음
                if (m_buffs[i].Data.Condition == BuffEffectCondition.Area)
                    continue;

                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.OnBuff(Caster.GameObject, m_buffs[i]);
            }
        }

        public void OnAreaBuff(Transform target)
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                // 지속형이 아니면 붙일 필요가 없음
                if (m_buffs[i].Data.Condition != BuffEffectCondition.Area)
                    continue;

                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.OnBuff(Caster.GameObject, m_buffs[i]);
            }
        }
        // 타겟에 스킬의 버프를 떼어준다.
        public void RemoveBuff(Transform target)
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                // 지속형이면 붙일 필요가 없음
                if (m_buffs[i].Data.Condition == BuffEffectCondition.Area)
                    continue;

                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.RemoveBuff(Caster.GameObject, m_buffs[i]);
            }
        }
        // 타겟에 스킬의 버프를 떼어준다.
        public void RemoveAreaBuff(Transform target)
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                // 지속형이 아니면 붙일 필요가 없음
                if (m_buffs[i].Data.Condition != BuffEffectCondition.Area)
                    continue;

                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.RemoveBuff(Caster.GameObject, m_buffs[i]);
            }
        }
        #endregion BUFF

        #region Skill Projectile

        protected void ProjectileInit()
        {
            if (m_activeProjectiles.Count != 0) return;

            for (int i = 0; i < m_projectiles.Count; i++)
            {
                var projectile = CreateProjectile(m_projectiles[i]);
                projectile.gameObject.SetActive(false);
                m_activeProjectiles.Add(projectile);
            }
        }
        // 단 하나의 투사체만 존재해야할 경우
        protected void ResetProjectiles()
        {
            for (int i = 0; i < m_activeProjectiles.Count; i++)
            {
                m_activeProjectiles[i].InActiveProjectile();
                Destroy(m_activeProjectiles[i].gameObject);
            }
            m_activeProjectiles.Clear();
            ProjectileInit();
        }


        /// <summary>
        ///   단순히 투사체를 생성 및 발사하는 경우
        /// </summary>
        protected bool ShootProjectiles()
        {
            // 조준이 근처 타겟 검색이면 한번 스캔한다.
            if (Data.AimType == AimType.NearTargetDirection)
            {
                ScanTarget();

                // 타겟이 없으면 안함
                if (Target == null && m_isAlwaysShoot == false)
                {
                    return false;
                }
            }


            for (int i = 0; i < m_projectiles.Count; i++)
            {
                ProjectileBase projectile;

                // 여러발을 생성해야할 때
                for (int j = 0; j < LevelData.Count; j++)
                {
                    // 생성해야하는 경우 투사체를 생성한다.
                    projectile = CreateProjectile(m_projectiles[i]);

                    if (projectile == null)
                        continue;

                    ActiveProjectile(projectile, j);
                }
            }
            return true;
        }

        // 투사체를 생성하고 세팅한다.
        protected ProjectileBase CreateProjectile(ProjectileBase projectile)
        {
            var cloneProjectile = Instantiate(projectile.gameObject).GetComponent<ProjectileBase>();
            SetAim(cloneProjectile.transform);
            cloneProjectile.SetSkill(this);
            SetProjectile(cloneProjectile);

            return cloneProjectile;
        }

        /// <summary>
        /// 모든 투사체 실행한다.
        /// </summary>
        protected bool ActiveProjectiles()
        {

            // 조준이 근처 타겟 검색이면 한번 스캔한다.
            if (Data.AimType == AimType.NearTargetDirection)
            {
                ScanTarget();

                // 타겟이 없으면 안함
                if (Target == null && m_isAlwaysShoot == false)
                {
                    return false;
                }
            }

            for (int i = 0; i < m_activeProjectiles.Count; i++)
            {
                var projectile = m_activeProjectiles[i];

                projectile.gameObject.SetActive(true);

                // 여러발을 생성해야할 때
                for (int j = 0; j < LevelData.Count; j++)
                {

                    if (projectile == null)
                        continue;

                    ActiveProjectile(projectile, j);
                }
            }


            return true;
        }

        // 투사체 활성화. 실행된다.
        protected void ActiveProjectile(ProjectileBase projectile, int index = 0)
        {
            if (projectile == null)
                return;
            projectile.SetSkill(this);
            SetAim(projectile.transform, index);
            projectile.ActiveProjectile();
        }

        protected virtual void SetProjectile(ProjectileBase projectile)
        {

        }

        #endregion Skill Projectile

        #region Aim
        // 조준과 관련된 메서드
        public Transform SetAim(Transform transform, int index = 0)
        {
            Transform parent = GameManager.Instance.ProjectileParent;
            Vector3 position = Caster.Model.position;
            Quaternion rotation = Caster.Model.rotation;

            Vector3 targetPos = transform.position;
            Quaternion targetRotation = Caster.Model.rotation;
            if (Target)
            {
                targetPos = Target.transform.position;
                targetPos.y = Caster.Model.position.y;
                if (targetPos - position != Vector3.zero)
                    targetRotation.SetLookRotation(targetPos - position);

            }

            switch (Data.AimType)
            {
                case AimType.Caster:
                    parent = Caster.Model;
                    position = Caster.Model.position;
                    break;

                case AimType.CasterPosition:
                    position = Caster.Transform.position;
                    parent = Caster.Transform;
                    break;

                case AimType.CasterDropPosition:
                    rotation = Quaternion.identity;
                    break;

                case AimType.CasterDirection:
                    break;

                case AimType.TargetDirection:
                    rotation = targetRotation;
                    break;

                case AimType.NearTargetDirection:
                    rotation = targetRotation;
                    parent = Caster.Transform;
                    break;

                case AimType.TargetPosition:
                    position = Target.position;
                    break;

                case AimType.RandomDirection:
                    Vector3 randomRot = Vector3.zero;
                    randomRot.y = Random.Range(0, 360);
                    rotation.eulerAngles = randomRot;
                    break;

                case AimType.PointerDirection:
                    rotation.SetLookRotation(GameManager.Instance.Aim.position - transform.position);
                    break;
                default: break;
            }

            position.y = Caster.Model.position.y + 1f;

            transform.parent = parent;
            transform.rotation = rotation;
            transform.position = position + transform.forward * LevelData.ProjectileOffset;


            if (Data.AimType != AimType.RandomDirection && Data.AimType != AimType.RandomTargetDirection)
            {
                transform.rotation = GetProjectileDirection(transform, index);
            }

            return transform;
        }
        private Quaternion GetProjectileDirection(Transform t, int index, float angleIncrement = 30)
        {
            Quaternion direction = t.rotation;

            if (index == 0)
                return direction;

            // 홀수짝수 구분
            int n = (index + 1) / 2;
            // 홀수짝수에 따라 각도 크기 정하기
            float angle = n * angleIncrement;

            // 홀수면 음수
            if (index % 2 == 1)
                angle = angle * -1;

            direction = Quaternion.Euler(0, t.transform.eulerAngles.y + angle, 0);

            return direction;
        }

        #endregion

        #region Skill Target

        // 타겟을 정해준다.
        public void SetTarget(Transform target)
        {
            m_skillTarget = target;
        }

        // 타겟을 스캔한다.
        protected void ScanTarget()
        {
            int count = Physics.OverlapSphereNonAlloc(Caster.Model.position + Caster.Model.forward * LevelData.ProjectileOffset, LevelData.Range, m_scanColls, Data.TargetLayer, QueryTriggerInteraction.Ignore);
            if (count <= 0)
                return;
            for (int i = 0; i < m_scanColls.Length; i++)
            {
                if (m_scanColls[i] == null)
                    break;


                if (m_scanColls[i].CompareTag(Data.SkillTarget.ToString()) == false)
                    continue;


                if (Target == null)
                {
                    SetTarget(m_scanColls[i].transform);
                    return;
                }


                float curDistance = Vector3.Distance(Target.position, Caster.Model.position);
                float colDistance = Vector3.Distance(m_scanColls[i].transform.position, Caster.Model.position);

                if (colDistance < curDistance)
                    SetTarget(m_scanColls[i].transform);
            }
        }
        protected float TargetAngle()
        {
            Vector3 target = m_skillTarget.position;
            target.y = transform.position.y;
            Vector3 dir = target - transform.position;
            return Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        }
        #endregion

        #region Particle

        protected virtual void PlayEffect()
        {
            for (int i = 0; i < m_particles.Count; i++)
            {
                m_particles[i].gameObject.SetActive(true);
                m_particles[i].Stop();
                m_particles[i].Play();
            }
            for (int i = 0; i < m_visualEffects.Count; i++)
            {
                m_visualEffects[i].gameObject.SetActive(true);

                m_visualEffects[i].Stop();
                m_visualEffects[i].Play();
            }
        }
        protected virtual void StopEffect()
        {
            for (int i = 0; i < m_particles.Count; i++)
            {
                m_particles[i].Stop();
                m_particles[i].gameObject.SetActive(false);

            }
            for (int i = 0; i < m_visualEffects.Count; i++)
            {
                m_visualEffects[i].Stop();
                m_visualEffects[i].gameObject.SetActive(false);

            }
        }
        #endregion Particle

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            if (Caster != null)
                Gizmos.DrawSphere(Caster.Model.position + Caster.Model.forward * LevelData.ProjectileOffset, LevelData.Radius);
        }
    }


}
