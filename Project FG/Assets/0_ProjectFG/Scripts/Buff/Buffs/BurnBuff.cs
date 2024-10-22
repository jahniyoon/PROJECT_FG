using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    // 이동속도 관련 버프
    [CreateAssetMenu(fileName = "Burn Buff", menuName = "ScriptableObjects/Buff/Burn Buff")]
    public class BurnBuff : BuffBase
    {


        public BurnBuff(BuffData data) : base(data) 
        {
            m_data = data;
        }
        public override float GetDuration()
        {
            return GetBuffValue(1);
        }

        public void Burn(BuffHandler handler)
        {
            if (handler.TryGetComponent<IDamageable>(out IDamageable damageable))
                damageable.OnDamage(FinalDamage(m_skillCaster.FinalDamage(GetBuffValue(0))), new Color(1,0.5f,1,1));
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            handler.Status.AddBurnBuff(this);
        }

        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            handler.Status.RemoveBurnBuff(this);

        }


    }
}