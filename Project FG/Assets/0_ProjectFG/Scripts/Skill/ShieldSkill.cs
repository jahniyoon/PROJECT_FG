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
        BuffBase m_shieldBuff;

        protected override void Init()
        {
            m_subData = m_data as ShieldSkillData;
            if (m_subData == null)
            { 
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

            m_shield = Instantiate(m_subData.ShieldPrefab, Caster.Model);
            m_shield.transform.localEulerAngles = Vector3.zero;
            m_shield.SetActive(false);

            m_shieldBuff = BuffFactory.CreateBuff(m_subData.ShieldBuff);

            SetDuration(m_subData.Duration);
        }

        protected override void UpdateBehavior()
        {
            m_coolDownTimer = 0 < m_coolDownTimer ? m_coolDownTimer - Time.deltaTime : 0;
        }


        public override void ActiveSkill()
        {
            base.ActiveSkill();
            if (Caster.Transform.TryGetComponent<BuffHandler>(out BuffHandler buff))
                buff.OnBuff(Caster.GameObject, m_shieldBuff);

            m_shield.SetActive(true);
            m_coolDownTimer = m_subData.LevelData.CoolDown;
        }



        public override void InactiveSkill()
        {
            base.InactiveSkill();
            m_shield.SetActive(false);
        }
    }
}
