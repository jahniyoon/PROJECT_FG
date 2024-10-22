using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    // 이동속도 관련 버프
    [CreateAssetMenu(fileName = "SlowSpeed Buff", menuName = "ScriptableObjects/Buff/SlowSpeed Buff")]
    public class SlowSpeedBuff : BuffBase
    {
        [SerializeField] private float m_slowSpeed;
        [SerializeField] private float m_duration;



        public SlowSpeedBuff(BuffData data) : base(data) 
        {
            m_data = data;
        }
        public override void SetBuffValue(float[] values)
        {
            base.SetBuffValue(values);
            m_slowSpeed = GetBuffValue();     // 스피드           
            m_duration = GetBuffValue(1);     //지속시간           
        }
        public override float GetDuration()
        {
            return m_duration;
        }
        // 버프를 비교해 높은 값을 가져온다.
        public override BuffBase ComparisonBuff(BuffBase targetBuff)
        {
            m_slowSpeed = Mathf.Max(this.GetBuffValue(), targetBuff.GetBuffValue());

            return this;
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(m_slowSpeed);
        }

        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);

            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(m_slowSpeed * -1);

            m_slowSpeed = GetBuffValue();
        }


    }
}