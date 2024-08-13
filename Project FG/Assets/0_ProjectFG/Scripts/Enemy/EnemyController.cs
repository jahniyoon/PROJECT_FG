using JH;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public partial class EnemyController : MonoBehaviour
{
    private FSM<EnemyController> m_fsm;
    protected PlayerController m_player;
    protected NavMeshAgent m_agent;
    protected Damageable m_damageable;
    protected Transform m_model;
    protected HitEffect m_hitEffect;

    [Header("Enemy Data")]
    [SerializeField] protected EnemyData m_data;

    [Header("State")]

    [SerializeField] protected FSMState m_state;

    [Header("Target")]
    [SerializeField] protected Transform m_target;
    [SerializeField] protected float m_targetDistance;

    [Header("HealthBar")]
    [SerializeField] private MiniHealthBar m_healthBar;
    [SerializeField] private FoodPower m_FoodPower;

    [Header("Predation")]
    [SerializeField] private bool m_canPredation;
    [SerializeField] private WorldSpaceIcon m_predationIcon;

    [Header("Stun")]
    [SerializeField] protected ParticleSystem m_stunEffect;
    [SerializeField] protected float m_stunCoolDown;

    // 공격 타이머 : 공격 상태 돌입 후 해당 타이머가 공격속도에 해당하는 값이 되면 공격이 실행된다.
    protected float m_attackTimer = 0;

    public bool CanPredation => m_canPredation;

    public FSMState State => m_state;

# region Lifecycle
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
        m_healthBar = Instantiate(m_healthBar.gameObject, this.transform).GetComponent<MiniHealthBar>();
        m_hitEffect = GetComponent<HitEffect>();

        m_FoodPower = m_data.FoodPower;

        // 포식 아이콘 미리 꺼둔다.
        m_predationIcon = Instantiate(m_predationIcon.gameObject, this.transform).GetComponent<WorldSpaceIcon>();
        m_predationIcon.IconEnable(false);
    }

    protected virtual void StartInit()
    {
        m_agent.speed = m_data.MoveSpeed;
        m_damageable.SetMaxHealth(m_data.Health);

        m_fsm = new IdleState();

        if (m_player)
            SetTarget(m_player.transform);
    }



    protected virtual void UpdateBehaviour()
    {
        TargetDistanceUpdate();
        FSMHandler();
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
    protected void ModelRotate(Vector3 position, bool instant = false)
    {

        Vector3 navRotation = m_agent.velocity.normalized;

        //  네비게이션 있으면 네비게이션 우선으로 방향 보기
        if (navRotation != Vector3.zero)
        {
            m_model.rotation = Quaternion.LookRotation(m_agent.velocity.normalized, Vector3.up);
            return;
        }


        Vector3 targetPos = position;
        targetPos.y = 0;

        Vector3 targetDir = targetPos - this.transform.position;



        Quaternion targetRotation = Quaternion.LookRotation(targetDir.normalized);
        Quaternion newRotation = Quaternion.Slerp(m_model.rotation, targetRotation, (m_data.RotateSpeed * Time.deltaTime));

        // 즉시 바라본다.
        if (instant)
        {
            newRotation = Quaternion.Slerp(m_model.rotation, targetRotation, ((m_data.RotateSpeed * 10f) * Time.deltaTime));
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
            m_stunCoolDown = m_data.PredationStunCoolDown;
        }

    }


    protected virtual void OnRestore()
    {

    }

    protected virtual void OnDamage()
    {
        m_hitEffect.Hit();
        ResetAttackTimer();
    }

    public virtual void OnStun(float stunDuration)
    {
        m_stunCoolDown += stunDuration;
    }

    protected virtual void Die()
    {
        m_canPredation = false;
        m_predationIcon.IconEnable(m_canPredation);

        m_hitEffect.Die();
        Destroy(gameObject, 1);
        UIManager.Instance.Debug.KillCountText(1);
    }

    // 처형
    public virtual void Execution(int damage)
    {
        m_damageable.OnDamage(damage);
    }
    public FoodPower GetFoodPower()
    {
        return m_FoodPower;
    }

    protected void ResetAttackTimer()
    {
        m_attackTimer = 0;
    }

}
