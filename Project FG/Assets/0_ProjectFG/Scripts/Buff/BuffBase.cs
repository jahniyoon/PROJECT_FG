using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class BuffBase 
    {
        [SerializeField] protected BuffData m_data;
        protected Status m_status = new Status();
        public Status Status
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

        public float StackUpTime => m_data.StackUpTime;
        public BuffType Type => m_data.Type;
        public bool IsOneAndOnly => m_data.IsOneAndOnly;
        public int ID => m_data.ID;
        public int Priority => m_data.Priority;
        public float DecreaseTime => m_data.DecreaseTime;
        public BuffData Data => m_data;

        public float[] Value1;
        public float[] Value2;
        protected Transform m_caster;


        public BuffBase(BuffData data)
        {
            m_data = data;
        }
  
        public void SetCaster(Transform caster)
        {
            m_caster = caster;
        }

        // 버프 활성화가 가능한지 체크하는 조건식
        public virtual bool CanActive(float timer)
        {
            return true;
        }

        public virtual bool CanActive(int Count)
        {
            return m_data.ActiveStack <= Count;
        }

        // 버프 활성화
        public virtual void ActiveBuff(BuffHandler handler) { }

        // 조건에 맞으면 활성화되는 버프
        public virtual void ConditionBuff(BuffHandler handler) { }

        // 버프를 쌓을 때 호출한다.
        public virtual void StackBuff(BuffHandler handler) { }

        // 버프 비활성화
        public virtual void InactiveBuff(BuffHandler handler) { }
        // 상태이상 업데이트
        public virtual void StatusInit()
        {
            m_status = new Status(); 
        }

        public virtual void SetBuffValue(float[] values) 
        {
            Value1 = values;
        }
        public virtual void SetValue2(float[] values)
        {
            Value2 = values;
        }
        protected float GetValue1(int value = 0)
        {
            if (Value1.Length == 0)
                return 0;

            if(Value1.Length-1 < value)
                return Value1[Value1.Length - 1];

            return Value1[value];
        }
        protected float GetValue2(int value = 0)
        {
            if (Value2.Length == 0)
                return 0;

            if (Value2.Length - 1 < value)
                return Value2[Value2.Length - 1];

            return Value2[value];
        }
    }
}
