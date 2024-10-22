using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    // 이동속도 관련 버프
    [CreateAssetMenu(fileName = "Slow Debuff", menuName = "ScriptableObjects/Buff/Slow Debuff")]
    public class SlowDebuff : BuffBase
    {
        public SlowDebuff(BuffData data) : base(data) 
        {
            m_data = data;
        }


        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(GetBuffValue());
        }




        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(GetBuffValue() * -1);
        }


    }
}