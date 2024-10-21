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
        [SerializeField] private float m_duration;

        public override float GetDuration()
        {
            return m_duration;
        }
        public override void SetBuffValue(float[] values)
        {
            base.SetBuffValue(values);
            m_duration = GetBuffValue();
            m_healValue = GetBuffValue(1);
        }

        public override BuffBase ComparisonBuff(BuffBase otherBuff)
        {
            m_healValue = Mathf.Max(this.GetBuffValue(1), otherBuff.GetBuffValue(1));
            return this;
        }


        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            if(handler.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.RestoreHealth(m_healValue);

            handler.Status.AddHealBuff(this);
        }

        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            handler.Status.RemoveHealBuff(this);
            m_healValue = GetBuffValue(1);

        }


    }
}