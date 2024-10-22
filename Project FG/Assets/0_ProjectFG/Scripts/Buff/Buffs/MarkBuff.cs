using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace JH
{
    // 표식 버프
    public class MarkBuff : BuffBase
	{
        public MarkBuff(BuffData data) : base(data) 
        {           
            m_data = data;
        }

        public override float GetDuration()
        {
            return TryGetValue1();
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if (handler.TryGetComponent<IMarkable>(out IMarkable markable))
            {
                markable.SetCaster(m_skillCaster);
                markable.OnMarkStack(GetBuffValue(), TryGetValue1(), Mathf.FloorToInt(TryGetValue1(1)));
            }
        }
        
     
    }
}
