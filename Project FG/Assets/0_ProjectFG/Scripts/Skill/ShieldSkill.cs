using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class ShieldSkill : SkillBase
    {
        private ShieldSkillData m_subData;
        private GameObject m_shield;
        [Header("Shield Skill")]
        [SerializeField] private float m_coolDownTimer;

        protected override void Init()
        {
            m_subData = m_skillData as ShieldSkillData;
            if (m_subData == null)
            { 
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

            m_shield = Instantiate(m_subData.ShieldPrefab, Position);
            m_shield.transform.localEulerAngles = Vector3.zero;
            m_shield.SetActive(false);

            SetDuration(m_subData.SkillDuration);
        }

        protected override void UpdateBehavior()
        {
            m_coolDownTimer = 0 < m_coolDownTimer ? m_coolDownTimer - Time.deltaTime : 0;
        }

        public override bool CanActiveSkill(bool enable = true)
        {
            return m_coolDownTimer <= 0;
        }


        public override void ActiveSkill()
        {
            base.ActiveSkill();
            if (Caster.TryGetComponent<BuffHandler>(out BuffHandler buff))
                buff.OnBuff(Caster, m_subData.ShieldBuff);

            m_shield.SetActive(true);
            m_coolDownTimer = m_subData.SkillCoolDown;
        }



        public override void InactiveSkill()
        {
            base.InactiveSkill();
            m_shield.SetActive(false);
        }
    }
}
