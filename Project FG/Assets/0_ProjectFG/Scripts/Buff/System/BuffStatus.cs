using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class BuffStatus
    {
        private BuffHandler m_handler;
        [Header("상태이상 관련 버프")]
        [SerializeField] private float m_stunTimer = 0;

        [Header("냉기 상태이상")]
        [SerializeField] private bool m_hasFrozenBuff;
        [SerializeField] private float m_frozenTimer = 0;   // 프로즌 타이머
        [SerializeField] private int m_frozenCurStack = 0;   // 동결 스택
        [SerializeField] private float m_frozenStackTimer = 0;   // 프로즌 타이머
        private FrozenBuff m_frozenBuff;

        [Header("냉기 버프 데이터")]
        private int m_frozenStack = 0;   // 동결 스택
        private float m_frozenStackUpCoolDown = 0;   // 냉기 데미지 쿨타임
        private float m_frozenStackDownCoolDown = 0;   // 냉기 데미지 쿨타임


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
        }
    }
}