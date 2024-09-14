using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public enum BuffType
    {
        Immediately,    // 즉시
        TimeCondition,      // 조건
        Stack,           // 스택
        Stay,           // 계속 붙어있을 때 까지
        Attachment           // 뗄 때까지
    }
    [System.Serializable]
    public class BuffBase : ScriptableObject
    {
        [SerializeField] protected int m_buffID; // 버프 InstanceID
        [SerializeField] protected string m_buffName; 
        [SerializeField] [TextArea]protected string m_buffDescription; 
        [field: Header("Buff Info")]
        [field: SerializeField] public BuffType Type { get; private set; } // 버프 지속시간
        [SerializeField] protected float m_buffDuration; // 버프 지속시간
        [Header("One And Only Buff")]
        [SerializeField] protected bool m_isOneAndOnlyBuff; // 단 하나만 활성화 된다.
        [SerializeField] protected int m_Priority = 0;    // 우선도
        [Header("Stack Buff")]
        [SerializeField] protected int m_activeStack; 
        [SerializeField] protected float m_stackUpTime; 
        [SerializeField] protected float m_decreaseTime; 

        protected Status m_status;
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


        #region Property
        public int ID => m_buffID;
        public float Duration => m_buffDuration;
        public bool IsOneAndOnly => m_isOneAndOnlyBuff;
        public int Priority => m_Priority;
        public int ActiveStack => m_activeStack;
        public float StackUpTime => m_stackUpTime;  
        public float DecreaseTime => m_decreaseTime;
        #endregion




        // 버프 활성화가 가능한지 체크하는 조건식
        public virtual bool CanActive(float timer)
        {
            return true;
        }

        public virtual bool CanActive(int Count)
        {
            return ActiveStack <= Count;
        }
        // 버프 활성화
        public virtual void ActiveBuff(BuffHandler handler)
        {

        }

        // 조건에 맞으면 활성화되는 버프
        public virtual void ConditionBuff(BuffHandler handler)
        {

        }

        // 버프를 쌓을 때 호출한다.
        public virtual void StackBuff(BuffHandler handler)
        {

        }

        // 버프 비활성화
        public virtual void InactiveBuff(BuffHandler handler)
        {

        }

        // 상태이상 업데이트
        public virtual void StatusInit()
        {
            Status status = new Status();
            m_status = status;
        }
    }
}
