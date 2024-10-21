using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    // 이동속도 관련 버프
    [CreateAssetMenu(fileName = "Frozen Buff", menuName = "ScriptableObjects/Buff/Frozen Buff")]
    public class FrozenBuff : BuffBase
    {
        [SerializeField] private float m_slowSpeed;
        [SerializeField] private float m_dotDamage;

        [SerializeField] private float m_stackUpCoolDown;
        [SerializeField] private int m_stack;
        [SerializeField] private float m_frozenDuration;
        [SerializeField] private float m_stackDownCoolDown;


        public FrozenBuff(BuffData data) : base(data) 
        {
            m_data = data;
        }
        public override void SetBuffValue(float[] values)
        {
            base.SetBuffValue(values);
            m_slowSpeed = GetBuffValue();     // 스피드
            m_dotDamage = GetBuffValue(1);    // 데미지

            m_stackUpCoolDown = m_data.TryGetValue1(0);
            m_stack = Mathf.FloorToInt(m_data.TryGetValue1(1));
            m_frozenDuration = m_data.TryGetValue1(2);
            m_stackDownCoolDown = m_data.TryGetValue1(3);
        }

        public override BuffBase ComparisonBuff(BuffBase targetBuff)
        {
            m_slowSpeed = Mathf.Max(this.GetBuffValue(), targetBuff.GetBuffValue());
            m_dotDamage = Mathf.Max(this.GetBuffValue(1), targetBuff.GetBuffValue(1));

            return this;
        }
        public override void StackBuff(BuffHandler handler)
        {
            base.StackBuff(handler);
            handler.Status.OnFrozen(m_frozenDuration);

        }
        public override void StackUpBuff(BuffHandler handler)
        {
            base.StackUpBuff(handler);
            if (handler.TryGetComponent<IDamageable>(out IDamageable damageable))
                damageable.OnDamage(m_dotDamage, Color.cyan);
        }


        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            handler.Status.AddFrozenBuff(this);
            handler.Status.SetFrozen(m_stackUpCoolDown, m_stack, m_stackDownCoolDown);

            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetSlowSpeed(m_slowSpeed);
        }

        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);

            handler.Status.RemoveFrozenBuff(this);


            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetSlowSpeed(m_slowSpeed * -1);


            m_slowSpeed = GetBuffValue();
            m_dotDamage = GetBuffValue(1);
        }


    }
}