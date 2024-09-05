using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class Status
    {
        [Header("상태이상 관련 버프")]
        [SerializeField] private float m_stunTimer = 0;

        #region Property
        public float StunTimer => m_stunTimer;         // 이동속도
        public bool IsStun => m_stunTimer > 0;
        #endregion

        public void Init(float StunTimer = 0)
        {

            this.m_stunTimer = StunTimer;
        }

        // 버프되는 부분 (이곳에 버프가 누적되면 합연산인지 곱연산인지 정리하면 된다.)
        public void Buff(Status other)
        {
            this.m_stunTimer = other.m_stunTimer;
        }

        // 디버프
        public void DeBuff(Status other)
        {

        }

        // 타이머를 계속 업데이트한다.
        public void UpdateTimer(float deltaTime)
        {
            m_stunTimer = 0 < m_stunTimer - deltaTime ? m_stunTimer - deltaTime : 0;
        }
    }
}