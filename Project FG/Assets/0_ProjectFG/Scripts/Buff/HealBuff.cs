using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{

    public class HealBuff : BuffBase
    {
        public HealBuff(BuffData data) : base(data) 
        {
            m_data = data;
        }
        [Header("Heal Buff")]
        [SerializeField] private float m_healValue;

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            if(handler.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.RestoreHealth(m_healValue);
            }

        }

      
    }
}