using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Stun Buff", menuName = "ScriptableObjects/Buff/Stun")]

    public class StunBuff : BuffBase
    {
        public StunBuff(BuffData data):base(data) 
        {
            m_data = data;
        }

        [Header("Stun Buff")]
        [SerializeField] private float m_stunDuration;
        public override void StatusInit()
        {
            BuffStatus status = new BuffStatus();
            status.SetStunTimer(stunTimer: m_stunDuration);
            m_status = status;
        }
        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            StatusInit();
            handler.BuffStatus(m_status);
        }

     
    }
}