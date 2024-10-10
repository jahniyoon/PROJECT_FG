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
        [SerializeField] private FoodPowerSkill m_aimShootSkill;
        private FoodPowerSkill m_skill;

        public override void LevelUp()
        {
            base.LevelUp();
            Inactive();
            Active();
        }

        public override void Active()
        {

            if (m_skill == null)
                m_skill = Instantiate(m_aimShootSkill.gameObject, this.transform).GetComponent<FoodPowerSkill>();
            m_skill.SkillInit(Caster);
            m_skill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));

        }
        public override void Inactive()
        {
            if (m_skill != null)
                m_skill.InactiveSkill();
            base.Inactive();
        }

    }
}