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

    [Header("Groggy")]
    [SerializeField] protected ParticleSystem m_groggyEffect;
    [SerializeField] protected float m_groggyCoolDown;

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

    protected bool GroggyCheck()
    {
        float curRatio = m_damageable.Health / (float)m_damageable.MaxHealth;

        return curRatio <= m_data.GroggyHealthRatio * 0.01f;        
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
    }

    protected virtual void OnDamage()
    {
        m_hitEffect.Hit();
        if(GroggyCheck())
        {
            m_groggyCoolDown = m_data.GroggyStunTime;
        }
    }

    protected virtual void OnGroggy()
    {
        if(m_groggyEffect)
            m_groggyEffect.Stop();
            m_groggyEffect.Play();
    }

    protected virtual void Die()
    {
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
        return m_data.FoodPower;
    }

}
