using JH;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public partial class EnemyController : MonoBehaviour, IPredationable, ISlowable, IKnockbackable, ISkillCaster, IPutrefaction, IFearable
{
    private FSM<EnemyController> m_fsm;
    private CapsuleCollider m_capsule;
    protected PlayerController m_player;
    protected NavMeshAgent m_agent;
    protected Damageable m_damageable;
    protected Transform m_model;
    protected HitEffect m_hitEffect;
    protected SpriteColor m_spriteColor;
    protected SpriteRenderer m_spriteRenderer;

    [Header("Enemy Data")]
    [SerializeField] protected EnemyData m_data;
    protected bool m_is2D;

    [Header("State")]

    [SerializeField] protected FSMState m_state;
    [SerializeField] protected EnemyMoveState m_moveState;
    [SerializeField] protected float m_distanceCheckDelay;



    [Header("Target")]
    [SerializeField] protected Transform m_target;
    [SerializeField] protected float m_targetDistance;

    [Header("HealthBar")]
    [SerializeField] private MiniHealthBar m_healthBar;
    [SerializeField] private float m_healthBarOffset = 1;

    [Header("Predation")]
    [SerializeField] private bool m_canPredation;
    [SerializeField] protected float m_fearPredationRatio;
    [SerializeField] private WorldSpaceIcon m_predationIcon;
    [SerializeField] protected float m_dieSpeed = 1.2f;
    protected BuffBase m_stunBuff;

    [Header("Attack CoolDown")]
    [SerializeField] protected float m_attackCoolDown;


    [Header("Buff")]
    protected BuffHandler m_buffHandler;
    protected float m_slowSpeed = 0;
    protected float m_fearSpeed = 0;
    [SerializeField] protected ParticleSystem m_stunEffect;
    [SerializeField] protected bool m_isKnockback;
    [SerializeField] protected bool m_isFear;

    [Header("Enemy Count")]
    [SerializeField] private bool m_notCount;
    [Header("Minimap Color")]
    [SerializeField] protected Color m_minimapColor = Color.red;

    [Header("에네미 스킬")]

    [SerializeField] protected float m_skillTimer = 0;
    [SerializeField] protected List<SkillBase> m_skills = new List<SkillBase>();
    [SerializeField] protected List<SkillBase> m_attackSkills = new List<SkillBase>();
    [SerializeField] protected List<SkillBase> m_routineSkills = new List<SkillBase>();


    [Header("DEBUG")]

    public GameObject m_MovePosition;


    protected Transform m_skillParent;


    protected float m_attackTimer = 0;
    private int m_instanceID;
    Vector3 m_moveDestination;


    public bool CanPredation => m_canPredation;
    public FSMState State => m_state;
    public int ID => m_data.ID;
    public Transform Transform => this.transform;
    public Transform Model => m_model;
    public GameObject GameObject => this.gameObject;
    public bool NotCount => m_notCount;
    public Damageable Damageable => m_damageable;
    public List<SkillBase> Skills => m_skills;

    #region Lifecycle
    private void Awake()
    {
        AwakeInit();
    }
    private void Start()
    {
        StartInit();
    }
    private void Update()
    {
        UpdateBehaviour();
    }
    private void OnEnable()
    {
        OnEnableInit();
    }
    private void OnDisable()
    {
        OnDisableInit();
    }
    #endregion

    protected virtual void OnEnableInit()
    {
        m_damageable.UpdateHealthEvent.AddListener(UpdateHealth);
        m_damageable.DamageEvent.AddListener(OnDamage);
        m_damageable.DieEvent.AddListener(Die);
        m_damageable.DieEvent.AddListener(OnPutrefaction);
    }
    protected virtual void OnDisableInit()
    {
        m_damageable.UpdateHealthEvent.RemoveListener(UpdateHealth);
        m_damageable.DamageEvent.RemoveListener(OnDamage);
        m_damageable.DieEvent.RemoveListener(Die);
        m_damageable.DieEvent.RemoveListener(OnPutrefaction);
    }

    protected virtual void AwakeInit()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            m_player = player.GetComponent<PlayerController>();
        }

        m_instanceID = this.gameObject.GetInstanceID();
        m_model = transform.GetChild(0);
        m_agent = GetComponent<NavMeshAgent>();
        m_damageable = GetComponent<Damageable>();
        m_capsule = GetComponent<CapsuleCollider>();

        // TODO : 에네미 다 만들고 게임 오브젝트 내에 있도록 수정
        var healthBar = transform.GetComponentInChildren<MiniHealthBar>();
        if (healthBar != null)
            m_healthBar = healthBar;

        else if (m_healthBar)
            m_healthBar = Instantiate(m_healthBar.gameObject, this.transform).GetComponent<MiniHealthBar>();


        m_hitEffect = GetComponent<HitEffect>();


        // 포식 아이콘 미리 꺼둔다.
        var predationIcon = transform.GetComponentInChildren<WorldSpaceIcon>();

        if (predationIcon != null)
            m_predationIcon = predationIcon;

        else if (m_predationIcon)
            m_predationIcon = Instantiate(m_predationIcon.gameObject, this.transform).GetComponent<WorldSpaceIcon>();


        m_buffHandler = GetComponent<BuffHandler>();


        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (m_spriteRenderer != null)
        {
            int id = m_data.ID - 20001;
            m_spriteRenderer.sortingOrder = 1;
            GetComponentInChildren<Animator>().SetInteger("EnemyColor", id);
            m_agent.angularSpeed = 0;
            m_is2D = true;
            m_hitEffect.enabled = false;
            Vector3 localPos = Vector3.zero;
            localPos.z += m_healthBarOffset;
            m_healthBar.transform.localPosition = localPos;
            localPos.z += 0.5f;

            m_predationIcon.transform.localPosition = localPos;
        }
        m_spriteColor = GetComponent<SpriteColor>();

        m_skillParent = new GameObject("Skill").transform;
        m_skillParent.parent = this.transform;
    }

    protected virtual void StartInit()
    {
        m_agent.speed = FinalSpeed(m_data.MoveSpeed);
        m_damageable.SetMaxHealth(m_data.Health);

        m_predationIcon.IconEnable(false);


        UIManager.Instance.MinimapUI.AddObject(m_instanceID, m_minimapColor);

        BuffBase stunBuff = BuffFactory.CreateBuff(m_data.PredationStun);
        m_stunBuff = stunBuff;


        m_fsm = new IdleState();

        if (m_player)
            SetTarget(m_player.transform);

        GetSkills();

    }



    protected virtual void UpdateBehaviour()
    {
        TargetDistanceUpdate();
        FSMHandler();
        CoolDownHandler();
        BuffHandler();

        m_skillTimer = m_skillTimer - Time.deltaTime <= 0 ? 0 : m_skillTimer -= Time.deltaTime;

        UIManager.Instance.MinimapUI.SetPosition(m_instanceID, this.transform.position);

    }

    private void TargetDistanceUpdate()
    {
        if (m_target == null)
        {
            m_targetDistance = -1;
            return;
        }
        m_targetDistance = Vector3.Distance(transform.position, m_target.position);
    }
    protected virtual void CoolDownHandler()
    {
        if (0 < m_attackCoolDown)
            m_attackCoolDown -= Time.deltaTime;
    }
    // 버프를 관리
    protected virtual void BuffHandler()
    {
    }


    protected virtual void SetTarget(Transform target)
    {
        m_target = target;
    }


    // 모델 방향 돌리기
    protected void ModelRotate(Vector3 position, bool navDir = true, bool instant = false)
    {
        Vector3 navRotation = m_agent.velocity.normalized;

        //  네비게이션 있으면 네비게이션 우선으로 방향 보기
        if (navRotation != Vector3.zero && navDir)
        {
            Quaternion rotation = Quaternion.LookRotation(m_agent.velocity.normalized, Vector3.up);

            if (m_is2D)
            {
                m_spriteRenderer.flipX = 180 <= rotation.eulerAngles.y;
                m_model.localRotation = rotation;

                return;
            }
            m_model.rotation = rotation;

            return;
        }


        Vector3 targetPos = position;
        targetPos.y = m_model.transform.position.y;

        Vector3 targetDir = targetPos - m_model.transform.position;

        if (targetDir.normalized == Vector3.zero)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDir.normalized);
        Quaternion newRotation = Quaternion.Slerp(m_model.rotation, targetRotation, (m_data.RotateSpeed * 10 * Time.deltaTime));

        if (instant)
            newRotation = targetRotation;

        if (m_is2D)
        {
            m_spriteRenderer.flipX = 180 <= newRotation.eulerAngles.y;
            m_model.localRotation = newRotation;


            return;
        }

        m_model.rotation = newRotation;
    }


    private void UpdateHealth()
    {
        float ratio = m_damageable.Health / (float)m_damageable.MaxHealth;
        m_healthBar.SetHealthSlider(ratio);

        // 체력을 업데이트 할 때마다 포식상태 체크 및 업데이트
        m_canPredation = CanPredationCheck();
        m_predationIcon.IconEnable(m_canPredation);
    }
    public void Predation()
    {
        m_buffHandler.OnBuff(this.gameObject, m_stunBuff);
    }

    protected virtual void OnRestore()
    {

    }

    protected virtual void OnDamage()
    {
        //ResetAttackTimer();
        if (m_is2D == false)
            m_hitEffect.Hit();

        m_spriteColor?.OnHit();
    }


    public void KillEnemy()
    {
        m_damageable.SetHealth(0);
        m_damageable.Die();
    }


    protected virtual void Die()
    {
        if (m_damageable.Excution)
            m_spriteRenderer.sortingOrder = 6;

        m_spriteColor?.StopFlicking();

        UIManager.Instance.MinimapUI.RemoveObject(m_instanceID);

        m_predationIcon.IconEnable(m_canPredation);

        m_healthBar.HealthBarEnable(false);
        m_predationIcon.gameObject.SetActive(false);
        // 버프 모두 지워주기
        m_buffHandler.RemoveAllBuff();


        // 죽으면 모든 스킬을 꺼준다.
        for (int i = 0; i < m_routineSkills.Count; i++)
        {
            m_routineSkills[i].InactiveSkill();
            m_routineSkills[i].RemoveSkill();
        }

        for (int i = 0; i < m_attackSkills.Count; i++)
        {
            m_attackSkills[i].InactiveSkill();
            m_attackSkills[i].RemoveSkill();

        }


        if (m_is2D == false)
            m_hitEffect.Die();

        UIManager.Instance.Debug.KillCountText(1);
        m_capsule.center = new Vector3(0, -10, 0);
        Invoke(nameof(Dead), 1f);
    }

    private void Dead()
    {
        this.transform.position = this.transform.position + Vector3.up * 1000;
        Destroy(gameObject, 1);
    }

    protected float TargetAngle()
    {
        Vector3 target = m_target.position;
        target.y = m_model.position.y;
        Vector3 dir = target - m_model.position;
        return Vector3.SignedAngle(m_model.forward, dir, Vector3.up);
    }

    // 처형
    public virtual void Execution(float damage = 0)
    {
        if (damage == 0)
            damage = m_damageable.Health;
        m_damageable.OnDamage(damage, true);
        m_canPredation = false;
    }
    public FoodPower GetFoodPower()
    {
        return m_data.FoodPower;
    }

    protected void ResetAttackTimer()
    {
        if (m_data.IgnoreAttack)
            return;
        m_attackTimer = 0;
    }


    #region State Check

    protected virtual bool CanAttackCheck()
    {
        return m_attackCoolDown <= 0;
    }


    protected virtual bool CCStateCheck()
    {
        return m_buffHandler.Status.CCStateCheck() || m_isKnockback || m_isFear;
    }


    ///  <summary> 플레이어와의 공격 가능한 거리를 계속 체크한다.</summary>
    /// <returns>TRUE : 공격 가능 / False : 공격 불가</returns>
    protected bool AttackRangeCheck()
    {
        if (m_target == null)
            return false;



        // 타겟과의 거리가 도망가는 거리보다 크고, 공격 가능거리보다 짧다.
        if (m_data.EscapeRange <= m_targetDistance && m_targetDistance < m_data.ChaseRange)
            return true;

        return false;
    }

    protected bool CanPredationCheck()
    {
        float curRatio = m_damageable.Health / (float)m_damageable.MaxHealth;

        if (m_damageable.IsDie || m_damageable.Excution)
            return false;
        // 공포 효과에 따라 달라짐
        return curRatio <= (m_data.PredationHealthRatio + m_fearPredationRatio) * 0.01f;
    }

    #endregion

    #region Speed

    public float FinalSpeed(float curSpeed)
    {
        return curSpeed * (100 - m_slowSpeed + m_fearSpeed) * 0.01f;
    }
    public void SetMoveSpeed(float value)
    {
        m_slowSpeed += value;
        m_agent.speed = FinalSpeed(m_data.MoveSpeed);
    }
    #endregion

    #region Skill
    // 모든 스킬을 가져온다.
    private void GetSkills()
    {
        foreach (int id in m_data.AttackSkillID)
        {
            SkillBase skill = CreateSkill(GFunc.GetSkillPrefab(id));
            if (skill == null) continue;

            m_attackSkills.Add(skill);
            m_skills.Add(skill);
        }

        bool routine = true;

        foreach (int id in m_data.RoutineSkillID)
        {
            // 루틴 스킬은 생성 시
            SkillBase skill = CreateSkill(GFunc.GetSkillPrefab(id), routine);
            if (skill == null) continue;

            m_routineSkills.Add(skill);
            m_skills.Add(skill);
        }
    }

    // 스킬 생성
    private SkillBase CreateSkill(SkillBase skill, bool routine = false)
    {
        if (skill == null)
            return null;
        SkillBase newSkill = Instantiate(skill.gameObject, transform.position, transform.rotation, m_skillParent).GetComponent<SkillBase>();

        if (m_target)
            newSkill.SetTarget(m_target);

        newSkill.SkillInit(this, routine);



        return newSkill;
    }

    protected SkillBase TryGetSkill(int index = 0)
    {
        for (int i = 0; i < m_attackSkills.Count; i++)
        {
            if (index == i)
                return m_attackSkills[i];
        }
        return null;
    }
    #endregion

    #region ISkillCaster
    public virtual bool CanActiveSkill()
    {
        if (m_state != FSMState.Attack)
            return false;

        return true;
    }

    // 스킬 타이머동안 움직이지 못함
    public void UpdateSkillTimer(float timer)
    {
        m_skillTimer = timer;
    }
    public float FinalDamage(float damage, DamageType type)
    {
        return m_buffHandler.Status.FinalAttackDamage(damage, type);
    }
    #endregion

    #region IPutrefaction
    public void AddPuterefaction(Putrefaction putrefaction)
    {
        m_buffHandler.Status.AddPutrefactionBuff(putrefaction);
    }
    public void RemovePuterefaction(Putrefaction putrefaction)
    {
        m_buffHandler.Status.RemovePutrefactionBuff(putrefaction);
    }
    // 사망 시 부패가 있으면 부패 폭발을 한다.
    public void OnPutrefaction()
    {
        m_buffHandler.Status.OnPutrefactionTransition();
    }

    // 전이된 부패는 예약해서 제거한다.
    public void SetPutrefactionOver(Putrefaction putrefaction)
    {
        Invoke(nameof(putrefaction.PutrefactionOver), putrefaction.Duration);
    }
    #endregion

    #region Move State
    // 이동 상태 업데이트
    protected void SetMoveState(EnemyMoveState nextState)
    {
        m_moveState = nextState;
    }


    // 이동 위치를 정해준다.
    protected void SetMoveDestination(Vector3 destination)
    {

        if (m_MovePosition != null)
        {
            m_MovePosition.transform.parent = transform.parent;
            m_MovePosition.transform.position = destination;
        }

        m_moveDestination = destination;

        m_agent.SetDestination(m_moveDestination);
        ModelRotate(m_moveDestination, false, true);
    }

    // 포위 위치를 찾는다.
    protected Vector3 FindSurroundPos()
    {
        Vector3 position = m_target.position;
        position.y = m_model.position.y;
        m_model.LookAt(position);

        // 랜덤한 포위거리
        float distance = Random.Range(m_data.SurroundDistance * -1, m_data.SurroundDistance);

        // 랜덤 포위 기반, 타겟 위치를 업데이트
        position = position + m_model.right * distance;

        // 네비게이션 확인
        position = GFunc.FindNavPos(transform.transform, position, m_data.SurroundDistance);


        return position;
    }

    /// <summary>
    /// 플레이어를 피해 도망갈 포지션을 찾는다.
    /// </summary>
    /// <returns></returns>
    protected Vector3 FindEscapePos()
    {
        // Vector2 일 때 방향 필요.
        // 현재 적의 위치
        Vector3 currentPosition = m_model.position;

        // 적에서 플레이어까지의 방향 벡터 (플레이어 방향)
        Vector3 directionToPlayer = m_target.position - currentPosition;

        // 플레이어로부터 반대 방향 (벡터의 반대 방향)
        Vector3 oppositeDirection = -directionToPlayer.normalized;


        float randomAngle = Random.Range(m_data.EscapeAngle * -0.5f, m_data.EscapeAngle * 0.5f);
        oppositeDirection = Quaternion.AngleAxis(randomAngle, Vector3.up) * oppositeDirection;

        // 도망가는 위치 = 현재 위치에서 반대 방향으로 일정 거리만큼 이동한 위치
        Vector3 escapePosition = currentPosition + oppositeDirection * m_data.EscapeRange;

        escapePosition = GFunc.FindNavPos(transform, escapePosition, m_data.EscapeRange * 2);
        return escapePosition;


        //// 이동가능한 곳인지 체크한다.

    }

    #endregion

    #region KnockBack
    Coroutine m_knockbackRoutine;

    public void OnKnockback(Vector3 hitPosition, float force, float duration)
    {
        if (m_knockbackRoutine != null)
        {
            StopCoroutine(m_knockbackRoutine);
            m_knockbackRoutine = null;
        }
        m_knockbackRoutine = StartCoroutine(KnockBackRoutine(hitPosition, force, duration));

    }

    IEnumerator KnockBackRoutine(Vector3 hitPos, float force, float duration)
    {
        m_isKnockback = true;


        float timer = 0;
        Vector3 startPos = transform.position;

        // 플레이어로부터 반대 방향 (벡터의 반대 방향)
        Vector3 knockbackDirection = -(hitPos - startPos).normalized;

        // 넉백 방향에 거리 추가
        Vector3 endPos = startPos + knockbackDirection * force;
        // 가능한 곳인지 확인 및 보정
        endPos = GFunc.FindNavPos(transform, endPos, force * 2);

        while (timer < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        m_isKnockback = false;
        yield break;
    }
    #endregion

    #region Fear
    Coroutine m_fearkRoutine;

    public void OnFear(Vector3 fearPos, float escapeSpeed, float duration, float predationHealthRatio)
    {
        if (m_fearkRoutine != null)
        {
            StopCoroutine(m_fearkRoutine);
            m_fearkRoutine = null;
        }
        m_fearkRoutine = StartCoroutine(FearRoutine(fearPos, escapeSpeed, duration, predationHealthRatio));
    }

    IEnumerator FearRoutine(Vector3 fearPos, float escapeSpeed, float duration, float predationHealthRatio)
    {
        m_isFear = true;
        m_fearPredationRatio = predationHealthRatio;
        m_fearSpeed = escapeSpeed;
        UpdateHealth(); // 체력 업데이트

        float timer = 0;
        if(timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        m_isFear = false;

        // 값 다시 되돌려놓는다.
        m_fearPredationRatio = 0;
        m_fearSpeed = 0;
        UpdateHealth(); // 체력 업데이트


        yield break;
    }
    #endregion

  

    public void DebugEnable(bool enable)
    {
        if (m_MovePosition != null)
            m_MovePosition.gameObject.SetActive(enable);
    }

}
