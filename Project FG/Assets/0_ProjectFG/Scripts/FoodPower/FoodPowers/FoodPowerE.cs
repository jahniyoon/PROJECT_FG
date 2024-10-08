using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace JH
{
    [System.Serializable]

    public class FoodPowerE : FoodPower
    {
        [Header("Skill")]
        [SerializeField] private FoodPowerSkill m_skill;
        [Header("Ignore ID")]
        private FoodPowerSkill m_activeSkill;

        public override void LevelUp()
        {
            base.LevelUp();
            Inactive();
            Active();
        }

        public override void Active()
        {
            Vector3 position = m_casterPosition.position;

            Quaternion direction = GetDirection();

            m_activeSkill = Instantiate(m_skill.gameObject, position, direction, m_caster.transform).GetComponent<FoodPowerSkill>();
            m_activeSkill.SkillInit(m_caster.gameObject, m_casterPosition);
            m_activeSkill.SetFoodPowerData(m_data.GetLevelData(m_powerLevel));

            m_activeSkill.LeagcyActiveSkill();
        }
        public override void Inactive()
        {
            if (m_activeSkill)
            {
                m_activeSkill.InactiveSkill();
                Destroy(m_activeSkill.gameObject);
            }
            base.Inactive();    
        }

    }
}