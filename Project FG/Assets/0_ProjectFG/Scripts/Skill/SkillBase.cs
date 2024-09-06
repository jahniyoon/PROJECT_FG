using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{
    public class SkillBase : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] protected SkillData m_skillData;

        [Header("Skill State")]
        [SerializeField] private GameObject m_caster;   // 스킬 캐스터
        [SerializeField] private bool m_isActive;       // 스킬 활성화 여부
        private Transform m_casterPosition;
        private float m_duration;

        [Header("Event")]
        [HideInInspector] public UnityEvent ActiveEvent = new UnityEvent();
        [HideInInspector] public UnityEvent InactiveEvent = new UnityEvent();

        Coroutine m_skillRoutine;

        #region Property
        public bool IsActive => m_isActive;
        public SkillData Data => m_skillData;
        public GameObject Caster => m_caster;
        public Transform Position => m_casterPosition;
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
            Init();
        }

        // 스킬 초기화
        protected virtual void Init() { }


        // 스킬에 업데이트가 필요한 부분
        protected virtual void UpdateBehavior() { }

        // 실행 가능 확인
        public virtual bool CanActiveSkill(bool enable = true) 
        { return enable; }

        // 스킬 실행
        public virtual void ActiveSkill() 
        {
            m_isActive = true;
            ActiveEvent?.Invoke();

            // 길이가 0이면 즉시 발동
            if (m_duration <= 0)
                return;

            // 스킬 루틴 시작
            if (m_skillRoutine != null)
                StopCoroutine(m_skillRoutine);
            m_skillRoutine = StartCoroutine(SkillRoutine(m_duration));
        }

        // 스킬 비활성화
        public virtual void InactiveSkill() 
        {
            m_isActive = false;
            InactiveEvent?.Invoke();
        }

        // 스킬의 지속시간을 정한다.
        protected void SetDuration(float duration)
        {
            m_duration = duration;
        }


        // 스킬 지속시간동안 이뤄지는 루틴
        public IEnumerator SkillRoutine(float duration)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            InactiveSkill();
            yield break;
        }

    }
}
