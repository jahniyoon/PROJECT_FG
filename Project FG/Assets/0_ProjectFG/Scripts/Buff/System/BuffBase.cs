using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class BuffBase : ScriptableObject
    {
        [HideInInspector][SerializeField] protected int m_instanceID; // 버프 고유 InstanceID
        [Header("Buff Base")]
        [SerializeField] protected int m_buffID; // 버프 InstanceID
        [SerializeField] protected string m_buffName; // 버프 InstanceID
        [SerializeField] protected string m_buffDescription; // 버프 InstanceID
        [Header("Buff Info")]
        [SerializeField] protected float m_buffDuration; // 버프 지속시간
        [Header("One And Only Buff")]
        [SerializeField] protected bool m_isOneAndOnlyBuff; // 단 하나만 활성화 된다.
        [SerializeField] protected int m_Priority = 0;    // 우선도

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
        public int InstanceID => m_instanceID;
        public int ID => m_buffID;
        public float Duration => m_buffDuration;
        public bool IsOneAndOnly => m_isOneAndOnlyBuff;
        public int Priority => m_Priority;
        #endregion


        // 고유 InstanceID 가져오기
        public virtual void SetID(GameObject obj)
        {
            //m_instanceID = this.GetInstanceID();
            m_instanceID = obj.GetInstanceID();
        }

        // 버프 활성화가 가능한지 체크하는 조건식
        public virtual bool CanActive(float timer)
        {
            return true;
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
