using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    // 이동속도 관련 버프
    [CreateAssetMenu(fileName = "Slow Debuff", menuName = "ScriptableObjects/Buff/Slow Debuff")]
    public class SlowDebuff : BuffBase
    {
        [Header("Slow Debuff")]
        [SerializeField] private float m_moveSpeedValue;
        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetSlowSpeed(m_moveSpeedValue);
        }




        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetSlowSpeed(m_moveSpeedValue*-1);
        }


    }
}