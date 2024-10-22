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
        public override float GetDuration()
        {
            return GetBuffValue();
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            handler.Status.OnStun(GetBuffValue());
        }

     
    }
}