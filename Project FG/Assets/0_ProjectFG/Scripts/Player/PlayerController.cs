using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.Security.Claims;
using System.Threading;

namespace JH
{
    public partial class PlayerController : MonoBehaviour, IKnockbackable
    {
        private FSM<PlayerController> m_fsm;
        private Transform m_model;

        private Damageable m_damageable;

        private PlayerInput m_input;
        private PlayerMovement m_movement;
        private PlayerPredation m_predation;
        private PlayerAttack m_attack;
        private PlayerAim m_aim;

        private AnimationController m_animation;
        private CinemachineImpulseSource m_impulse;

        [Header("Game Settings")]
        [SerializeField] private GameSettings m_gameSettings;

        [Header("Player")]
        [SerializeField] private FSMState m_playerState;
        [SerializeField] private bool m_isFreeze;

        #region 프로퍼티
        public PlayerInput Input => m_input;
        public GameSettings Setting => m_gameSettings;
        public AnimationController Animation => m_animation;
        public Transform Model => m_model;
        public Transform Aim => m_aim.Aim;
        public FSMState State => m_playerState;
        #endregion

        private void Awake()
        {
            m_model = transform.GetChild(0);

            m_damageable = GetComponent<Damageable>();

            m_input = GetComponent<PlayerInput>();
            m_movement = GetComponent<PlayerMovement>();
            m_predation = GetComponent<PlayerPredation>();
            m_attack = GetComponent<PlayerAttack>();
            m_aim = GetComponent<PlayerAim>();

            m_animation = GetComponent<AnimationController>();
            m_impulse = GetComponent<CinemachineImpulseSource>();
        }

        private void Start()
        {
            m_damageable.SetMaxHealth(m_gameSettings.PlayerMaxHealth);
            m_fsm = new IdleState();
        }

        private void OnEnable()
        {
            m_damageable.UpdateHealthEvent.AddListener(SetHealthUI);
            m_damageable.DamageEvent.AddListener(OnDamage);
            m_damageable.DieEvent.AddListener(Die);
        }

        private void OnDisable()
        {
            m_damageable.UpdateHealthEvent.RemoveListener(SetHealthUI);
            m_damageable.DamageEvent.RemoveListener(OnDamage);
            m_damageable.DieEvent.RemoveListener(Die);
        }

        private void Update()
        {
            FSMHandler();
            AnimationHandler();
        }


        private void AnimationHandler()
        {
            m_animation.SetBool(AnimationID.isMove, m_playerState == FSMState.Move);
        }


        // 체력 UI 업데이트
        public void SetHealthUI()
        {
            UIManager.Instance.MainUI.HealthUI.SetSlider(m_damageable.MaxHealth, m_damageable.Health);
        }

        public void OnDamage()
        {
            m_animation.SetTrigger(AnimationID.isHit);
            m_impulse?.GenerateImpulse();
        }


        // 게임오버
        public void Die()
        {
            m_animation.SetLayer("Upper Layer", 0);
            m_animation.SetBool(AnimationID.isDie, true);
            GameManager.Instance.GameOver();
        }

        public void LookAt(Vector3 position)
        {
            m_movement.LookAt(position);
        }

        public void GodMode()
        {
            m_damageable.InvincibleMode();
        }


        private bool IsFixedFrame()
        {
            Debug.Log(Time.time + " / " + Time.fixedTime + " / " + Time.deltaTime + " / " + Time.fixedDeltaTime);
            return Time.time == Time.fixedTime;
        }

        public void SetFreeze(bool enable)
        {
            m_isFreeze = enable;
        }


        public void OnKnockback(Vector3 hitPosition, float force, float duration)
        {
            SetFreeze(true);

            if(knockbackRoutine != null)
            {
                StopCoroutine(knockbackRoutine);
                knockbackRoutine = null;
            }
            knockbackRoutine = StartCoroutine(KnockBackRoutine(hitPosition, force, duration));

        }
        Coroutine knockbackRoutine;

        IEnumerator KnockBackRoutine(Vector3 hitPos,float force, float duration)
        {
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
                transform.position = Vector3.Lerp(startPos, endPos, timer/duration);
                timer += Time.deltaTime;
                yield return null;
            }
            SetFreeze(false);
            yield break;
        }


    }



}