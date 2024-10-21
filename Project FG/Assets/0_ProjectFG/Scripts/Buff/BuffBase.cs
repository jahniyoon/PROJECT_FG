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
        public float[] Value1;
        public float[] Value2;
        protected Transform Caster;


        public BuffBase(BuffData data)
        {
            m_data = data;
        }


        // 버프를 사용하는 캐스터를 세팅한다.
        // 넉백같은 경우 위치를 스스로가 체크해야하기 때문에 각자 캐스터가 필요
        public void SetCaster(Transform caster)
        {
            Caster = caster;
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
        public virtual float GetDuration()
        {
            return 0;
        }

        // 버프 활성화
        public virtual void ActiveBuff(BuffHandler handler) { }

        // 조건에 맞으면 활성화되는 버프
        public virtual void ConditionBuff(BuffHandler handler) { }

        public virtual void StackBuff(BuffHandler handler) { }

        // 버프를 쌓을 때 호출한다.
        public virtual void StackUpBuff(BuffHandler handler) { }


        // 버프 비활성화
        public virtual void InactiveBuff(BuffHandler handler) { }

        // 상태이상 업데이트
        public virtual void StatusInit()
        {
            m_status = new BuffStatus(); 
        }

        // 버프를 서로 비교해서 기준에 맞는 값을 보내준다.
        public virtual BuffBase ComparisonBuff(BuffBase otherBuff)
        {
            Debug.Log("버프베이스 들어오나?");

            return this;
        }

        // 외부에서 버프의 값을 수정하고싶을 때
        public virtual void SetBuffValue(float[] values) 
        {
            BuffValue = values;
        }

        public virtual void SetValue1(float[] values)
        {
            Value1 = values;
        }

        public virtual void SetValue2(float[] values)
        {
            Value2 = values;
        }

        public float GetBuffValue(int value = 0)
        {
            if (BuffValue.Length == 0)
                return 0;

            if(BuffValue.Length < value)
                return BuffValue[BuffValue.Length - 1];

            return BuffValue[value];
        }
        public float GetValue1(int value = 0)
        {
            if (Value1.Length == 0)
                return 0;

            if (Value1.Length - 1 < value)
                return Value1[Value1.Length - 1];

            return Value1[value];
        }

        public float GetValue2(int value = 0)
        {
            if (Value2.Length == 0)
                return 0;

            if (Value2.Length - 1 < value)
                return Value2[Value2.Length - 1];

            return Value2[value];
        }
    }
}
