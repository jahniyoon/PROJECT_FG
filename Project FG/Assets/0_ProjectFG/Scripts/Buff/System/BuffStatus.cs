using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class BuffStatus
    {
        private BuffHandler m_handler;

        [Header("주는 데미지")]
        [SerializeField] private float m_attackDamageIncrease = 0;   // 주는 데미지 증가


        [Header("냉기")]
        [SerializeField] private bool m_hasFrozenBuff;
        [SerializeField] private float m_frozenTimer = 0;   // 프로즌 타이머
        [SerializeField] private int m_frozenCurStack = 0;   // 동결 스택
        [SerializeField] private float m_frozenStackTimer = 0;   // 프로즌 타이머
        private FrozenBuff m_frozenBuff;

        [Header("냉기 데이터")]
        private int m_frozenStack = 0;   // 동결 스택
        private float m_frozenStackUpCoolDown = 0;   // 냉기 데미지 쿨타임
        private float m_frozenStackDownCoolDown = 0;   // 냉기 데미지 쿨타임


        [Header("힐")]
        [SerializeField] private bool m_hasHealBuff;
        private Dictionary<HealBuff, float> m_healBuffs = new Dictionary<HealBuff, float>();


        [Header("스턴")]
        [SerializeField] private float m_stunTimer = 0;

        #region Property
        public float StunTimer => m_stunTimer;         // 이동속도
        public bool IsStun => m_stunTimer > 0;
        public bool IsFrozen => m_frozenTimer > 0;
        #endregion

        public void Init(BuffHandler handler)
        {
            m_handler = handler;
        }

        //  상태이상을 체크
        public bool IsStatusState()
        {
            return IsStun || IsFrozen;
        }

        #region AttackDamage
        public void SetAttackDamage(float attackDamageIncrease)
        {
            m_attackDamageIncrease += attackDamageIncrease;
        }
        public float FinalAttackDamage(float damage)
        {
            return damage * (100 - m_attackDamageIncrease) * 0.01f;
        }

        #endregion

        #region Frozen

        private void FrozenUpdate(float deltaTime)
        {
            m_hasFrozenBuff = m_frozenBuff != null;
            m_frozenTimer = 0 < m_frozenTimer - deltaTime ? m_frozenTimer - deltaTime : 0;

            // 버프가 없으면 시간을 줄여준다.
            if (m_hasFrozenBuff == false)
            {
                m_frozenStackTimer = 0 < m_frozenStackTimer - deltaTime ? m_frozenStackTimer - deltaTime : 0;

                // 스택 내리기
                if (m_frozenStackTimer <= 0 && 0 < m_frozenCurStack)
                {
                    m_frozenCurStack--;
                    m_frozenStackTimer = m_frozenStackDownCoolDown;
                }
                return;
            }

            // 타이머 업
            m_frozenStackTimer += deltaTime;

            // 스택 업
            if (m_frozenStackUpCoolDown < m_frozenStackTimer)
            {
                m_frozenCurStack++;
                m_frozenBuff.StackUpBuff(m_handler);

                if (m_frozenStack <= m_frozenCurStack)
                {
                    m_frozenBuff.StackBuff(m_handler);
                    m_frozenCurStack = 0;
                }
                m_frozenStackTimer = 0;
            }
        }
        public void AddFrozenBuff(FrozenBuff buff)
        {
            m_frozenBuff = buff;
        }
        public void RemoveFrozenBuff(FrozenBuff buff)
        {
            m_frozenBuff = null;
        }

        public void SetFrozen(float stackUpCoolDown, int stack, float stackDownCoolDown)
        {
            m_frozenStackUpCoolDown = stackUpCoolDown;
            m_frozenStack = stack;
            m_frozenStackDownCoolDown = stackDownCoolDown;

        }
        public void OnFrozen(float frozneTimer)
        {
            m_frozenTimer += frozneTimer;
        }


        #endregion


        #region Heal
        public void AddHealBuff(HealBuff healBuff)
        {
            if (m_healBuffs.ContainsKey(healBuff)) return;

            m_healBuffs.Add(healBuff, 0);
        }
        public void RemoveHealBuff(HealBuff healBuff)
        {
            if(m_healBuffs.ContainsKey(healBuff))
            m_healBuffs.Remove(healBuff);
        }

        private void HealBuffUpdate(float deltaTime)
        {
            m_hasHealBuff = 0 < m_healBuffs.Count;

            if (m_hasHealBuff == false)
                return;

            var keys = new List<HealBuff>(m_healBuffs.Keys);
            foreach (var key in keys)
            {
                m_healBuffs[key] += deltaTime;
                if (key.Data.TryGetValue1() < m_healBuffs[key])
                {
                    key.ActiveBuff(m_handler);
                    m_healBuffs[key] = 0;
                }
            }
        }
        #endregion


        #region Stun
        public void SetStunTimer(float stunTimer = 0)
        {
            this.m_stunTimer = stunTimer;
        }

        // 버프되는 부분 (이곳에 버프가 누적되면 합연산인지 곱연산인지 정리하면 된다.)
        public void Stun(BuffStatus other)
        {
            this.m_stunTimer = other.m_stunTimer;
        }

        #endregion



        // 타이머를 계속 업데이트한다.
        public void UpdateTimer(float deltaTime)
        {
            m_stunTimer = 0 < m_stunTimer - deltaTime ? m_stunTimer - deltaTime : 0;
            FrozenUpdate(deltaTime);
            HealBuffUpdate(deltaTime);
        }
    }
}