using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace JH
{
    [System.Serializable]

    public class FoodPowerH : FoodPower
    {
        [Header("Skill")]
        [SerializeField] private FoodPowerSkill m_mineSkill;
        private FoodPowerSkill m_skill;



        public override void Active()
        {
            if (m_skill == null)
                m_skill = Instantiate(m_mineSkill.gameObject, this.transform).GetComponent<FoodPowerSkill>();
            m_skill.SkillInit(Caster);
            m_skill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));

        }
        public override void Inactive()
        {
            if (m_skill != null)
            {
                m_skill.InactiveSkill();
            }
            base.Inactive();
        }
        public override void Remove()
        {
            base.Remove();
            if (m_skill != null)
            {
                m_skill.RemoveSkill();
            }
        }
    }
}