using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{
    public class SkillBase : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] protected SkillData m_data;

        [Header("Skill State")]
        [SerializeField] private GameObject m_caster;   // 스킬 캐스터
        [SerializeField] private bool m_isActive;       // 스킬 활성화 여부
        protected bool m_isCast;

        protected Transform m_casterPosition;
        protected Transform m_skillTarget;
        [Header("CoolDown")]

        [SerializeField] private float m_duration;
        [SerializeField] protected float m_skillCoolDown;
        [SerializeField] protected float m_skillCoolDownTimer;

        [Header("Buff")]
        [SerializeField] protected List<BuffBase> m_buffs = new List<BuffBase>();

        [Header("Projectile")]
        [SerializeField] protected List<ProjectileBase> m_projectiles = new List<ProjectileBase>();


        [Header("Event")]
        [HideInInspector] public UnityEvent ActiveEvent = new UnityEvent();
        [HideInInspector] public UnityEvent InactiveEvent = new UnityEvent();

        Coroutine m_skillRoutine;
        Coroutine m_infiniteRoutine;
        protected ISkillCaster m_skillCaster;
        Collider[] m_scanColls;

        #region Property
        public bool IsActive => m_isActive;
        public SkillData Data => m_data;
        public GameObject Caster => m_caster;
        public Transform Position => m_casterPosition;
        public int ID => m_data.ID;
        public Transform Target => m_skillTarget;
        public List<BuffBase> Buffs => m_buffs;
        public float Duration => m_duration;
        public float CoolDown => m_skillCoolDown;
        #endregion

        #region LifeCycle

        private void Update()
        {
            UpdateBehavior();
        }
        #endregion

        // 스킬 초기화. 캐스터를 정해준다.
        public void SkillInit(GameObject caster, Transform casterPosition)
        {
            m_caster = caster;
            m_casterPosition = casterPosition;
            m_skillCaster = caster.GetComponent<ISkillCaster>();

            // 스킬을 즉시 시작 가능하게 할지 말지 정해진다.
            m_skillCoolDownTimer = 0;
            if (m_data.ActiveTime == SkillActiveTime.CoolDown)
                ResetTimer();

            m_duration = m_data.SkillDuration;
            m_skillCoolDown = m_data.SkillCoolDown;
            // 스킬에 버프를 장착한다.
            m_buffs.Clear();
            for (int i = 0; i < Data.BuffID.Length; i++)
            {
                BuffData buffData = GFunc.GetBuff(Data.BuffID[i]);
                BuffBase buff = BuffFactory.CreateBuff(buffData);
                if (buff == null)
                    continue;

                buff.SetCaster(m_casterPosition);
                buff.SetBuffValue(m_data.BuffValues);
                m_buffs.Add(buff);
            }

            m_projectiles.Clear();
            for (int i = 0; i < Data.ProjectileID.Length; i++)
            {
                ProjectileBase projectile = GFunc.GetProjectilePrefab(Data.ProjectileID[i]);

                if (projectile == null) continue;

                m_projectiles.Add(projectile);
            }

            if (Data.AimType == AimType.Caster)
            {
                transform.parent = m_casterPosition;
                transform.localRotation = Quaternion.identity;
                transform.localPosition = Vector3.zero;
            }

            Init();
        }

        // 스킬 초기화
        protected virtual void Init() { }


        // 스킬에 업데이트가 필요한 부분
        protected virtual void UpdateBehavior()
        {
            m_skillCoolDownTimer = m_skillCoolDownTimer - Time.deltaTime <= 0 ? 0 : m_skillCoolDownTimer - Time.deltaTime;
            if (CheckCondition())
                CastSkill();

        }
        protected virtual bool CheckCondition()
        {
            if (m_skillCaster == null)
                return false;

            return m_skillCaster.CanActiveSkill() && m_skillCoolDownTimer <= 0;
        }

        // 실행 가능 확인
        public virtual bool CanActiveSkill(bool enable = true)
        {
            return enable;
        }
        public virtual void CastSkill()
        {
            if (this.gameObject.activeInHierarchy == false || m_isCast) return;

            m_isCast = true;
            ResetTimer();

            SetSkillAim(transform);


            // 스킬 사용 타이머
            m_skillCaster.UpdateSkillTimer(m_data.SkillSpeed);

            // 스킬 지속시간 이후 종료 루틴 시작
            if (m_skillRoutine != null)
            {
                StopCoroutine(m_skillRoutine);
                m_skillRoutine = null;
            }
            m_skillRoutine = StartCoroutine(ActiveSkillRoutine(m_duration));
        }
        public virtual void ActiveSkill()
        {
            m_isActive = true;
            ActiveEvent?.Invoke();
            m_isCast = true;

            if (m_data.AimType == AimType.NearTargetDirection)
                ScanTarget();
        }

        // 스킬 실행
        public virtual void LeagcyActiveSkill()
        {
            if (this.gameObject.activeInHierarchy == false)
                return;

            ResetTimer();
            SetSkillAim(transform);

            m_isActive = true;
            ActiveEvent?.Invoke();


            // 스킬 지속시간 이후 종료 루틴 시작
            if (m_skillRoutine != null)
            {
                StopCoroutine(m_skillRoutine);
                m_skillRoutine = null;
            }
            m_skillRoutine = StartCoroutine(ActiveSkillRoutine(m_duration));
        }

        public void TryActive()
        {
            if (0 < m_skillCoolDownTimer) return;

            CastSkill();
        }
        // 계속 반복하는 스킬을 넣는다.
        public void StartSkillRoutine()
        {
            if (m_infiniteRoutine != null)
            {
                StopCoroutine(m_infiniteRoutine);
                m_infiniteRoutine = null;
            }
            m_infiniteRoutine = StartCoroutine(InfiniteRoutine());
        }
        IEnumerator InfiniteRoutine()
        {
            while (true)
            {
                TryActive();
                yield return null;
            }
        }

        protected void ResetTimer()
        {
            m_skillCoolDownTimer = m_skillCoolDown;
        }

        // 스킬 비활성화
        public virtual void InactiveSkill()
        {
            if (m_skillRoutine != null)
                StopCoroutine(m_skillRoutine);

            m_isActive = false;
            m_isCast = false;
            InactiveEvent?.Invoke();
        }

        // 스킬의 지속시간을 변경한다.
        protected void SetDuration(float duration)
        {
            m_duration = duration;
        }


        public void SetTarget(Transform target)
        {
            m_skillTarget = target;
        }

        // 스킬 지속시간동안 이뤄지는 루틴
        protected virtual IEnumerator ActiveSkillRoutine(float duration = 0)
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

        // 타겟에 스킬의 버프를 붙인다.
        public void OnBuff(Transform target)
        {


            for (int i = 0; i < m_buffs.Count; i++)
            {
                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.OnBuff(Caster, m_buffs[i]);
            }
        }
        public void RemoveBuff(Transform target)
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.RemoveBuff(Caster, m_buffs[i]);
            }
        }

        // 조준 타입을 정해준다.
        protected virtual void SetSkillAim(Transform skillTransform)
        {
            switch (Data.AimType)
            {
                case AimType.Caster:
                    skillTransform.position = m_casterPosition.position;
                    break;

                case AimType.CasterPosition:
                    skillTransform.position = m_casterPosition.position;
                    break;

                case AimType.CasterDirection:
                    skillTransform.rotation = m_casterPosition.rotation;
                    break;

                case AimType.NearTargetDirection:
                    if (m_skillTarget)
                        skillTransform.LookAt(m_skillTarget.transform.position);
                    break;

                case AimType.TargetDirection:
                    if (m_skillTarget)
                        skillTransform.LookAt(m_skillTarget.transform.position);
                    break;

                case AimType.TargetPosition:
                    if (m_skillTarget)
                        skillTransform.position = m_skillTarget.transform.position;
                    break;

                case AimType.RandomDirection:
                    Vector3 randomRotation = Vector3.zero;
                    randomRotation.y = Random.Range(0, 360);
                    skillTransform.eulerAngles = randomRotation;
                    break;

                case AimType.RandomTargetDirection:
                    if (m_skillTarget)
                        skillTransform.LookAt(m_skillTarget.transform.position);
                    break;

                case AimType.RandomTargetPosition:
                    if (m_skillTarget)
                        skillTransform.position = m_skillTarget.transform.position;
                    break;
            }


        }

        protected ProjectileBase CreateProjectile(ProjectileBase projectile)
        {
        
            var cloneProjectile = Instantiate(projectile.gameObject).GetComponent<ProjectileBase>();
            SetAim(cloneProjectile.transform);
            cloneProjectile.SetSkill(this);

            return cloneProjectile;
        }

        protected Transform SetAim(Transform transform)
        {
            Transform parent = GameManager.Instance.ProjectileParent;
            Vector3 position = m_casterPosition.position;
            Quaternion rotation = m_casterPosition.rotation;

            Vector3 targetPos = transform.position;
            if (m_skillTarget)
            {
                targetPos = m_skillTarget.transform.position;
                targetPos.y = m_casterPosition.position.y;
            }

            switch (m_data.AimType)
            {
                case AimType.Caster:
                    parent = m_casterPosition;
                    position = m_casterPosition.position;
                    break;

                case AimType.CasterPosition:
                    position = Caster.transform.position;
                    parent = Caster.transform;
                    break;

                case AimType.CasterDropPosition:
                    rotation = Quaternion.identity;
                    break;

                case AimType.CasterDirection:
                    break; 
     
                case AimType.TargetDirection:
                        rotation.SetLookRotation(targetPos - position);
                    break;

                case AimType.NearTargetDirection:
                    rotation.SetLookRotation(targetPos - position);
                    break;

                case AimType.TargetPosition:
                    position = m_skillTarget.position;
                    break;

                case AimType.RandomDirection:
                    Vector3 randomRot = Vector3.zero;
                    randomRot.y =  Random.Range(0, 360); 
                    rotation.eulerAngles = randomRot;
                    break;

                case AimType.PointerDirection:
                    rotation.SetLookRotation(GameManager.Instance.Aim.position - transform.position);
                    break;
                default: break;
            }

            position.y = m_casterPosition.position.y;

            transform.position = position;
            transform.parent = parent;
            transform.rotation = rotation;
            return transform;
        }
        protected void ActiveProjectile(ProjectileBase projectile)
        {
            if (projectile == null)
                return;

            SetAim(projectile.transform);
            projectile.ActiveProjectile();
        }
        protected void ScanTarget()
        {
            m_scanColls = Physics.OverlapSphere(m_casterPosition.position + m_casterPosition.forward * m_data.ProjectileOffset, m_data.SkillRange);
            {
                for (int i = 0; i < m_scanColls.Length; i++)
                {
                    if (m_scanColls[i].isTrigger)
                        continue;

                    if (m_scanColls[i].CompareTag(m_data.SkillTarget.ToString()) == false)
                        continue;
                    float curDistance = Vector3.Distance(m_skillTarget.position, m_casterPosition.position);
                    float colDistance = Vector3.Distance(m_scanColls[i].transform.position, m_casterPosition.position);

                    if (colDistance < curDistance)
                    {
                        m_skillTarget = m_scanColls[i].transform;
                        Debug.Log(m_skillTarget);
                    }
                }


            }
        }


    }


}
