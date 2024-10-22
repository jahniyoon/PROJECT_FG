using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class BuffBase 
    {
        [SerializeField] protected BuffData m_data;
        protected BuffStatus m_status = new BuffStatus();
        protected Transform m_caster;
        protected ISkillCaster m_skillCaster;

        public BuffStatus Status
        {
            get
            {
                if (m_status == null)
                {
                    StatusInit();
                }
                return m_status;
            }
            protected set
            {
                m_status = value;
            }
        }

        public BuffType Type => m_data.Type;
        public int ID => m_data.ID;
        public int Priority => m_data.Priority;
        public float DecreaseTime => m_data.DecreaseTime;
        public BuffData Data => m_data;
        public float[] BuffValue;

        public Transform Caster => m_caster;


        public BuffBase(BuffData data)
        {
            m_data = data;
        }


        // 버프를 사용하는 캐스터를 세팅한다.
        // 넉백같은 경우 위치를 스스로가 체크해야하기 때문에 각자 캐스터가 필요
        public void SetCaster(ISkillCaster skill, Transform caster)
        {
            m_skillCaster = skill;
            m_caster = caster;
        }

        // 버프 활성화가 가능한지 체크하는 조건식
        public virtual bool CanActive(float timer)
        {
            return true;
        }

        public virtual float GetDuration()
        {
            return 0;
        }

        /// <summary>
        /// 버프가 활성화 될 때 실행되는 버프
        /// </summary>
        /// <param name="handler"></param>
        public virtual void ActiveBuff(BuffHandler handler) { }

        /// <summary>
        /// 스택이 목표 스택에 도달했을 때 실행되는 버프
        /// </summary>
        /// <param name="handler"></param>
        public virtual void StackBuff(BuffHandler handler) { }

        /// <summary>
        /// 스택을 쌓을 때 실행되는 버프
        /// </summary>
        /// <param name="handler"></param>
        public virtual void StackUpBuff(BuffHandler handler) { }


        /// <summary>
        /// 버프 비활성화
        /// </summary>
        /// <param name="handler"></param>
        public virtual void InactiveBuff(BuffHandler handler) { }

       /// <summary>
       /// 상태이상 초기화
       /// </summary>
        public virtual void StatusInit()
        {
            m_status = new BuffStatus(); 
        }

        /// <summary>
        /// 버프를 서로 비교해서 기준에 맞는 값을 보내준다.
        /// </summary>
        /// <param name="otherBuff">비교 대상의 버프</param>
        /// <returns></returns>
        public virtual BuffBase ComparisonBuff(BuffBase otherBuff)
        {
            Debug.Log("버프베이스 들어오나?");

            return this;
        }

        /// <summary>
        /// 스킬 등 외부에서 버프의 값을 수정해야 할 경우
        /// </summary>
        /// <param name="values"></param>
        public virtual void SetBuffValue(float[] values) 
        {
            BuffValue = values;
        }

        /// <summary>
        /// 버프의 추가 값을 가져온다.
        /// </summary>
        /// <param name="index">배열 인덱스</param>
        /// <returns></returns>
        public float GetBuffValue(int index = 0)
        {
            if (BuffValue.Length == 0)
                return 0;

            if(BuffValue.Length < index)
                return BuffValue[BuffValue.Length - 1];

            return BuffValue[index];
        }
        public float TryGetValue1(int value = 0)
        {
            return m_data.TryGetValue1(value);
        
        }

        public float GetValue2(int value = 0)
        {
            return m_data.TryGetValue2(value);

        }

        public float FinalDamage(float damage, DamageType type)
        {
            if (m_skillCaster == null)
                return damage;
            return m_skillCaster.FinalDamage(damage, type);
        }
    }
}
