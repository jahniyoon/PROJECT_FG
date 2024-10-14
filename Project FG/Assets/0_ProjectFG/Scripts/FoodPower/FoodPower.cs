using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;


namespace JH
{
    [System.Serializable]
    public class FoodPower : MonoBehaviour
    {
        [SerializeField] protected FoodPowerData m_data;

        [Header("Food Power")]
        [SerializeField] protected int m_powerLevel;
        [SerializeField] private float m_coolDownTimer;
        [Header("푸드파워 활성화 여부")]
        [SerializeField] protected bool m_mainPower;
        [Header("습득한 푸드파워")]
        [SerializeField] protected bool m_effectFoodPower;

        protected FoodPowerSkill m_skill;

        // 푸드파워 실행 루틴
        Coroutine foodRoutine;
        ISkillCaster m_caster;


        bool isActive;

        #region Property
        public ISkillCaster Caster => m_caster;
        public Sprite Icon => m_data.Icon;
        public float CoolDown => m_data.GetLevelData(m_powerLevel).CoolDown;
        public float Timer => m_coolDownTimer;
        public bool Main => m_mainPower;
        public int ID => m_data.ID;
        public int Level => m_powerLevel;
        public bool IsEffectFoodPower => m_effectFoodPower;
        public FoodPowerData Data => m_data;
        #endregion

        public UnityEvent ActiveEvent = new UnityEvent();
        public UnityEvent LevelUpEvent = new UnityEvent();

        private void OnDisable()
        {
            ActiveEvent.RemoveAllListeners();
        }


        public virtual void Init(bool isMain = false)
        {
            if (m_data == null)
                return;

            SetLevel(0);
            m_coolDownTimer = 0;
        }
        public void SetMain(bool enable = true)
        {
            m_mainPower = enable;
        }
        public virtual void SetCaster(ISkillCaster caster)
        {
            m_caster = caster;
        }

        public virtual SkillBase GetSkillBase()
        {
            return m_skill;
        }


        // 푸드파워의 효과
        public virtual void Active()
        {

        }
           
        // 스킬이 애니메이션을 발동한다.
        public void SkillActiveEvent()
        {
            ActiveEvent.Invoke();
        }
        public virtual void Inactive()
        {

        }

        public virtual void Remove()
        {
            Inactive();
        }

        // 레벨업하면 스킬을 비활성화한 뒤 활성화한다.
        public virtual void LevelUp()
        {
            m_powerLevel++;
            Inactive();
            Active();
            LevelUpEvent?.Invoke();
        }

        // 레벨 업
        public virtual void SetLevel(int value)
        {
            m_powerLevel = value;
            LevelUpEvent?.Invoke();

        }



        // 푸드파워 실행
        public void ActiveFoodPower()
        {
            Active();

            isActive = true;
        }
        // 푸드파워 정지
        public void InActiveFoodPower()
        {
            Inactive();
            isActive = false;
        }



        // 이 푸드파워는 계산하지 않는다.
        public void EffectFoodPower()
        {
            m_effectFoodPower = true;
        }
    }
}