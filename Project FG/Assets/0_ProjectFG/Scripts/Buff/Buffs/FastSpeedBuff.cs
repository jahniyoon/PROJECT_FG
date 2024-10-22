using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    // 이동속도 관련 버프
    [CreateAssetMenu(fileName = "FastSpeed Buff", menuName = "ScriptableObjects/Buff/FastSpeed Buff")]
    public class FastSpeedBuff : BuffBase
    {
        [SerializeField] private float m_fastSpeed;
        [SerializeField] private float m_duration;



        public FastSpeedBuff(BuffData data) : base(data) 
        {
            m_data = data;
        }
        public override void SetBuffValue(float[] values)
        {
            base.SetBuffValue(values);
            m_fastSpeed = GetBuffValue();     // 스피드           
            m_duration = GetBuffValue(1);     //지속시간           
        }
        public override float GetDuration()
        {
            return m_duration;
        }
        // 버프를 비교해 높은 값을 가져온다.
        public override BuffBase ComparisonBuff(BuffBase targetBuff)
        {
            m_fastSpeed = Mathf.Max(this.GetBuffValue(), targetBuff.GetBuffValue());

            return this;
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(m_fastSpeed * -1);
        }

        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);

            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(m_fastSpeed);

            m_fastSpeed = GetBuffValue();
        }


    }
}