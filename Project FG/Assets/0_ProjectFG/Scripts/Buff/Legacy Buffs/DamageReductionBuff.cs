using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace JH
{
    // 데미지 무시 관련 버프
    public class DamageReductionBuff : BuffBase
	{
        public DamageReductionBuff(BuffData data) : base(data) 
        {           
            m_data = data;
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if (handler.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.SetHitDamageIncrease(GetBuffValue());
            }
        }
        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            if (handler.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.SetHitDamageIncrease(GetBuffValue() * -1);
            }
        }
     
    }
}