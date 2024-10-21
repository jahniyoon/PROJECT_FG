using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace JH
{
    // 데미지 무시 관련 버프
    public class KnockbackBuff : BuffBase
	{
        public KnockbackBuff(BuffData data) : base(data) 
        {           
            m_data = data;
        }

        public override float GetDuration()
        {
            return GetBuffValue(1);
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if (handler.TryGetComponent<IKnockbackable>(out IKnockbackable knockbackable))
            {
                knockbackable.OnKnockback(Caster.position, GetBuffValue(0), GetBuffValue(1));
            }
        }
        
     
    }
}
