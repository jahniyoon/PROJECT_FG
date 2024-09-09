using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Stun Buff", menuName = "ScriptableObjects/Buff/Stun")]

    public class StunBuff : BuffBase
    {
        [Header("Stun Buff")]
        [SerializeField] private float m_stunDuration;

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            StatusInit();
            handler.BuffStatus(Status);
        }

        public override void StatusInit()
        {
            Status status = new Status();
            status.Init(StunTimer: m_stunDuration);
            m_status = status;
        }
    }
}