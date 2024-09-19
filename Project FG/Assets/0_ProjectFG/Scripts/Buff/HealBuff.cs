using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Heal Buff", menuName = "ScriptableObjects/Buff/HealBuff")]

    public class HealBuff : BuffBase
    {
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