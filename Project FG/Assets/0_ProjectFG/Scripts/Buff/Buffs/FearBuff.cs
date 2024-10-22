using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace JH
{
    // 데미지 무시 관련 버프
    public class FearBuff : BuffBase
	{
        public FearBuff(BuffData data) : base(data) 
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

            if (handler.TryGetComponent<IFearable>(out IFearable fearable))
            {
                fearable.OnFear(Caster.position, GetValue1(), GetDuration(), GetValue1(1));
            }
        }
        
     
    }
}
