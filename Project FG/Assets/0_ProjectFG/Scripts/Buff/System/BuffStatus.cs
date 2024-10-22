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
        [SerializeField] private float m_attributeDamageIncrease = 0;   // 주는 속성 데미지 증가


        [Header("냉기")]
        [SerializeField] private bool m_isFrozen;
        [SerializeField] private float m_frozenTimer = 0;   // 프로즌 타이머
        [SerializeField] private int m_frozenCurStack = 0;   // 동결 스택
        [SerializeField] private float m_frozenStackTimer = 0;   // 프로즌 타이머
        private FrozenBuff m_frozenBuff;

        [Header("냉기 데이터")]
        private int m_frozenStack = 0;   // 동결 스택
        private float m_frozenStackUpCoolDown = 0;   // 냉기 데미지 쿨타임
        private float m_frozenStackDownCoolDown = 0;   // 냉기 데미지 쿨타임


        [Header("화상")]
        [SerializeField] private bool m_isBurn;
        private Dictionary<BurnBuff, float> m_burnBuffs = new Dictionary<BurnBuff, float>();

        [Header("부패")]
        [SerializeField] private bool m_isPutrefaction;
        private List<Putrefaction> m_putrefactions = new List<Putrefaction>();


        [Header("힐")]
        [SerializeField] private bool m_isHeal;
        private Dictionary<HealBuff, float> m_healBuffs = new Dictionary<HealBuff, float>();

        [Header("스턴")]
        [SerializeField] private float m_stunTimer = 0;

        [Header("무적")]
        [SerializeField] private bool m_isInvincible;
        private List<InvincibleBuff> m_invincibleBuffs = new List<InvincibleBuff>();

        #region Property
        public bool IsStun => m_stunTimer > 0;
        public bool IsBurn => m_isBurn;
        public bool IsPutrefaction => m_isBurn;
        public bool IsFrozen => m_isFrozen;
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
        public float FinalAttackDamage(float damage, DamageType type)
        {
            return damage * (100 - m_attackDamageIncrease) * 0.01f;
        }

        #endregion

        #region Frozen

        private void FrozenUpdate(float deltaTime)
        {
            m_isFrozen = m_frozenBuff != null;
            m_frozenTimer = 0 < m_frozenTimer - deltaTime ? m_frozenTimer - deltaTime : 0;

            // 버프가 없으면 시간을 줄여준다.
            if (m_isFrozen == false)
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

        #region Burn
        public void AddBurnBuff(BurnBuff burnBuff)
        {
            if (m_burnBuffs.ContainsKey(burnBuff)) return;
            m_burnBuffs.Add(burnBuff, burnBuff.Data.TryGetValue1());
        }
        public void RemoveBurnBuff(BurnBuff burnBuff)
        {
            if (m_burnBuffs.ContainsKey(burnBuff))
                m_burnBuffs.Remove(burnBuff);

        }
        private void BurnBuffUpdate(float deltaTime)
        {
            m_isBurn = 0 < m_burnBuffs.Count;

            if (m_isBurn == false)
                return;

            var keys = new List<BurnBuff>(m_burnBuffs.Keys);
            foreach (var key in keys)
            {
                m_burnBuffs[key] += deltaTime;
                if (key.Data.TryGetValue1() < m_burnBuffs[key])
                {
                    key.Burn(m_handler);
                    m_burnBuffs[key] = 0;
                }
            }
        }

        #endregion

        #region Putrefaction
        public void AddPutrefactionBuff(Putrefaction putrrefaction)
        {
            if (m_putrefactions.Contains(putrrefaction)) return;
            m_putrefactions.Add(putrrefaction);
        }
        public void RemovePutrefactionBuff(Putrefaction putrrefaction)
        {
            if (m_putrefactions.Contains(putrrefaction))
                m_putrefactions.Remove(putrrefaction);
        }
        // 부패가 전이
        public void OnPutrefactionTransition()
        {
            for(int i = 0; i < m_putrefactions.Count; i++)
            {
                m_putrefactions[i].OnPutrefactionTransition(m_handler);
            }
        }
        private void PutrefactionBuffUpdate(float deltaTime)
        {
            m_isPutrefaction = 0 < m_putrefactions.Count;

            if (m_isPutrefaction == false)
                return;

            for (int i = 0; i < m_putrefactions.Count; i++)
            {
                m_putrefactions[i].Tick(deltaTime);

                // 데미지를 줄 수 있으면
                if (m_putrefactions[i].CanPutrefactionDamage)
                    m_putrefactions[i].OnPutrefactionDamage(m_handler);

                // 지속시간이 끝났으면 제거
                if (m_putrefactions[i].isPutrefactionOver)
                    RemovePutrefactionBuff(m_putrefactions[i]);
            }
        }
        #endregion

        #region Heal
        public void AddHealBuff(HealBuff healBuff)
        {
            if (m_healBuffs.ContainsKey(healBuff)) return;

            m_healBuffs.Add(healBuff, healBuff.Data.TryGetValue1());
        }
        public void RemoveHealBuff(HealBuff healBuff)
        {
            if (m_healBuffs.ContainsKey(healBuff))
                m_healBuffs.Remove(healBuff);
        }

        private void HealBuffUpdate(float deltaTime)
        {
            m_isHeal = 0 < m_healBuffs.Count;

            if (m_isHeal == false)
                return;

            var keys = new List<HealBuff>(m_healBuffs.Keys);
            foreach (var key in keys)
            {
                m_healBuffs[key] += deltaTime;
                // 주기적으로 힐을 넣는다.
                if (key.Data.TryGetValue1() < m_healBuffs[key])
                {
                    key.Heal(m_handler);
                    m_healBuffs[key] = 0;
                }
            }
        }
        #endregion


        #region Stun


        // 버프되는 부분 (이곳에 버프가 누적되면 합연산인지 곱연산인지 정리하면 된다.)
        public void OnStun(float timer)
        {
            m_stunTimer += timer;
        }
        private void StunUpdate(float deltaTime)
        {
            m_stunTimer = 0 < m_stunTimer - deltaTime ? m_stunTimer - deltaTime : 0;
        }

        #endregion

        #region Invincible
        public void AddInvincible(InvincibleBuff buff)
        {
            if (m_invincibleBuffs.Contains(buff)) return;
            m_invincibleBuffs.Add(buff);

            buff.Invincible(m_handler, true);
        }
        public void RemoveInvincible(InvincibleBuff buff)
        {
            if(m_invincibleBuffs.Contains(buff))
                m_invincibleBuffs.Remove(buff);

            // 리스트가 비어있으면 무적 상태 해제
            if(m_invincibleBuffs.Count == 0)
                buff.Invincible(m_handler, false);
        }
        #endregion

        // 타이머를 계속 업데이트한다.
        public void UpdateTimer(float deltaTime)
        {
            StunUpdate(deltaTime);
            FrozenUpdate(deltaTime);
            HealBuffUpdate(deltaTime);
            BurnBuffUpdate(deltaTime);
            PutrefactionBuffUpdate(deltaTime);
        }
    }
}