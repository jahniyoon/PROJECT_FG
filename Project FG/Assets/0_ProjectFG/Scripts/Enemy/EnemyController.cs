using JH;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public partial class EnemyController : MonoBehaviour, IPredationable, ISlowable, IKnockbackable
{
    private FSM<EnemyController> m_fsm;
    protected PlayerController m_player;
    protected NavMeshAgent m_agent;
    protected Damageable m_damageable;
    protected Transform m_model;
    protected HitEffect m_hitEffect;

    [Header("Enemy Data")]
    [SerializeField] protected EnemyData m_data;
    [SerializeField] protected SpriteRenderer m_spriteRenderer;
    protected bool m_is2D;

    [Header("State")]

    [SerializeField] protected FSMState m_state;

    [Header("Buff")]
    protected BuffHandler m_buffHandler;
    protected float m_slowDebuff = 0;


    [Header("Target")]
    [SerializeField] protected Transform m_target;
    [SerializeField] protected float m_targetDistance;

    [Header("HealthBar")]
    [SerializeField] private MiniHealthBar m_healthBar;
    [SerializeField] private FoodPower m_FoodPower;

    [Header("Predation")]
    [SerializeField] private bool m_canPredation;
    [SerializeField] private WorldSpaceIcon m_predationIcon;

    [Header("Attack CoolDown")]
    [SerializeField] protected float m_attackCoolDown;

    [Header("Stun")]
    [SerializeField] protected ParticleSystem m_stunEffect;
    protected bool m_isKnockback;


    // 공격 타이머 : 공격 상태 돌입 후 해당 타이머가 공격속도에 해당하는 값이 되면 공격이 실행된다.
    protected float m_attackTimer = 0;

    public bool CanPredation => m_canPredation;

    public FSMState State => m_state;
    public int ID => m_data.ID;
    public Transform Transform => this.transform;

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
    }
    protected virtual void OnDisableInit()
    {
        m_damageable.UpdateHealthEvent.RemoveListener(UpdateHealth);
        m_damageable.DamageEvent.RemoveListener(OnDamage);
        m_damageable.DieEvent.RemoveListener(Die);
    }

    protected virtual void AwakeInit()
    {
        m_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        m_model = transform.GetChild(0);
        m_agent = GetComponent<NavMeshAgent>();
        m_damageable = GetComponent<Damageable>();
        if (m_healthBar)
            m_healthBar = Instantiate(m_healthBar.gameObject, this.transform).GetComponent<MiniHealthBar>();
        m_hitEffect = GetComponent<HitEffect>();

        m_FoodPower = m_data.FoodPower;

        // 포식 아이콘 미리 꺼둔다.
        m_predationIcon = Instantiate(m_predationIcon.gameObject, this.transform).GetComponent<WorldSpaceIcon>();
        m_predationIcon.IconEnable(false);

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
            localPos.z += 1;
            m_healthBar.transform.localPosition = localPos;
            m_predationIcon.transform.localPosition = localPos;
        }
    }

    protected virtual void StartInit()
    {
        m_agent.speed = FinalSpeed(m_data.MoveSpeed);
        m_damageable.SetMaxHealth(m_data.Health);

        m_fsm = new IdleState();

        if (m_player)
            SetTarget(m_player.transform);
    }



    protected virtual void UpdateBehaviour()
    {
        TargetDistanceUpdate();
        FSMHandler();
        CoolDownHandler();
        BuffHandler();
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
    public float FinalSpeed(float curSpeed)
    {
        return curSpeed * (100 - m_slowDebuff) * 0.01f;
    }
    public void SetSlowSpeed(float value)
    {
        m_slowDebuff += value;
        m_agent.speed = FinalSpeed(m_data.MoveSpeed);
    }

    protected virtual void SetTarget(Transform target)
    {
        m_target = target;
    }

    protected virtual void AttackCheck()
    {

    }

    protected bool CanPredationCheck()
    {
        float curRatio = m_damageable.Health / (float)m_damageable.MaxHealth;

        return curRatio <= m_data.PredationHealthRatio * 0.01f;
    }


    // 모델 방향 돌리기
    protected void ModelRotate(Vector3 position, bool navDir = true, bool instant = false)
    {

        Vector3 navRotation = m_agent.velocity.normalized;

        //  네비게이션 있으면 네비게이션 우선으로 방향 보기
        if (navRotation != Vector3.zero && navDir)
        {
            m_model.rotation = Quaternion.LookRotation(m_agent.velocity.normalized, Vector3.up);
            return;
        }


        Vector3 targetPos = position;
        targetPos.y = m_model.transform.position.y;

        Vector3 targetDir = targetPos - m_model.transform.position;



        Quaternion targetRotation = Quaternion.LookRotation(targetDir.normalized);
        Quaternion newRotation = Quaternion.Slerp(m_model.rotation, targetRotation, (m_data.RotateSpeed * Time.deltaTime));

        // 즉시 바라본다.
        if (instant)
        {
            newRotation = Quaternion.Slerp(m_model.rotation, targetRotation, ((m_data.RotateSpeed * 10f) * Time.deltaTime));
        }

        if (m_is2D)
        {
            m_spriteRenderer.flipX = newRotation.eulerAngles.y < 180;
            m_model.localRotation = newRotation;
            return;
        }

        m_model.rotation = newRotation;
    }


    private void UpdateHealth()
    {
        float ratio = m_damageable.Health / (float)m_damageable.MaxHealth;
        m_healthBar.SetHealthSlider(ratio);

        bool isPredation = m_canPredation;

        // 체력을 업데이트 할 때마다 포식상태 체크 및 업데이트
        m_canPredation = CanPredationCheck();
        m_predationIcon.IconEnable(m_canPredation);

        // 포식 가능한 상태가 아니였다가 포식 가능 상태가 되었을 때
        if (isPredation == false && m_canPredation == true)
        {
            m_buffHandler.OnBuff(this.gameObject, m_data.PredationStun);
        }

    }
    public void Predation()
    {

    }

    protected virtual void OnRestore()
    {

    }

    protected virtual void OnDamage()
    {
        //ResetAttackTimer();
        if (m_is2D == false)
            m_hitEffect.Hit();
    }



    protected virtual void Die()
    {
        m_canPredation = false;
        m_predationIcon.IconEnable(m_canPredation);
        // 버프 모두 지워주기
        m_buffHandler.RemoveAllBuff();

        transform.GetComponent<CapsuleCollider>().enabled = false;


        if (m_is2D == false)
            m_hitEffect.Die();

        UIManager.Instance.Debug.KillCountText(1);
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
    }
    public FoodPower GetFoodPower()
    {
        return m_FoodPower;
    }

    protected void ResetAttackTimer()
    {
        if (m_data.IgnoreAttack)
            return;
        m_attackTimer = 0;
    }


    protected virtual bool CanAttackCheck()
    {
        return m_attackCoolDown <= 0;
    }


    protected virtual bool HitStateCheck()
    {
        return m_buffHandler.Status.IsStun || m_isKnockback;
    }


    ///  <summary> 플레이어와의 공격 가능한 거리를 계속 체크한다.</summary>
    /// <returns>TRUE : 공격 가능 / False : 공격 불가</returns>
    protected bool AttackRangeCheck()
    {
        if (m_target == null)
            return false;

        float targetDistance = Vector3.Distance(transform.position, m_target.transform.position);

        // 타겟과의 거리가 도망가는 거리보다 크고, 공격 가능거리보다 짧다.
        if (m_data.EscapeRange < targetDistance && targetDistance <= m_data.AttackRange)
            return true;

        return false;
    }

    /// <summary>
    /// 플레이어를 피해 도망갈 포지션을 찾는다.
    /// </summary>
    /// <returns></returns>
    protected Vector3 FindChasePos()
    {

        // 현재 적의 위치
        Vector3 currentPosition = transform.position;

        // 적에서 플레이어까지의 방향 벡터 (플레이어 방향)
        Vector3 directionToPlayer = m_target.position - currentPosition;

        // 플레이어로부터 반대 방향 (벡터의 반대 방향)
        Vector3 oppositeDirection = -directionToPlayer.normalized;

        // 도망가는 위치 = 현재 위치에서 반대 방향으로 일정 거리만큼 이동한 위치
        Vector3 escapePosition = currentPosition + oppositeDirection * m_data.EscapeRange;

        escapePosition = GFunc.FindNavPos(transform, escapePosition, m_data.EscapeRange * 2);
        return escapePosition;


        //// 이동가능한 곳인지 체크한다.

    }


    public void OnKnockback(Vector3 hitPosition, float force, float duration)
    {
        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
        }
        knockbackRoutine = StartCoroutine(KnockBackRoutine(hitPosition, force, duration));

    }
    Coroutine knockbackRoutine;

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


}
