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

            m_stackUpCoolDown = TryGetValue1(0);
            m_stack = Mathf.FloorToInt(m_data.TryGetValue1(1));
            m_frozenDuration = TryGetValue1(2);
            m_stackDownCoolDown = TryGetValue1(3);
        }
        // 버프를 비교해 높은 값을 가져온다.
        public override BuffBase ComparisonBuff(BuffBase targetBuff)
        {
            m_slowSpeed = Mathf.Max(this.GetBuffValue(), targetBuff.GetBuffValue());
            m_dotDamage = Mathf.Max(this.GetBuffValue(1), targetBuff.GetBuffValue(1));

            return this;
        }
        // 스택이 목표 스택에 도달했을 경우
        public override void StackBuff(BuffHandler handler)
        {
            base.StackBuff(handler);
            handler.Status.OnFrozen(m_frozenDuration);

        }
        // 스택이 오를 경우의 버프
        public override void StackUpBuff(BuffHandler handler)
        {
            base.StackUpBuff(handler);
            if (handler.TryGetComponent<IDamageable>(out IDamageable damageable))
                damageable.OnDamage(FinalDamage(m_dotDamage, DamageType.Attribute), Color.cyan);
        }


        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            handler.Status.AddFrozenBuff(this);
            handler.Status.SetFrozen(m_stackUpCoolDown, m_stack, m_stackDownCoolDown);

            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(m_slowSpeed);
        }

        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);

            handler.Status.RemoveFrozenBuff(this);


            if (handler.TryGetComponent<ISlowable>(out ISlowable slowable))
                slowable.SetMoveSpeed(m_slowSpeed * -1);


            m_slowSpeed = GetBuffValue();
            m_dotDamage = GetBuffValue(1);
        }


    }
}