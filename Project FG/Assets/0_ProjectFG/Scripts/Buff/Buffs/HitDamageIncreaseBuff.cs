using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace JH
{
    // 받는 데미지 증가
    public class HitDamageIncrease : BuffBase
	{
        public HitDamageIncrease(BuffData data) : base(data) 
        {           
            m_data = data;
        }
        public override float GetDuration()
        {
            return GetBuffValue(1); // 지속 시간
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if (handler.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.SetHitDamageIncrease(GetBuffValue() * -1);
            }
        }
        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            if (handler.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.SetHitDamageIncrease(GetBuffValue());
            }
        }
     
    }
}
