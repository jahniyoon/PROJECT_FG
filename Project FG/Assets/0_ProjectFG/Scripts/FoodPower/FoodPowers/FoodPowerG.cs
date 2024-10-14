using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace JH
{
    [System.Serializable]

    public class FoodPowerG : FoodPower
    {
        [Header("Skill")]
        [SerializeField] private FoodPowerSkill m_healSkill;


        public override void Init(bool isMain = false)
        {
            base.Init(isMain);
            LevelUpEvent.AddListener(UpdateLevel);
        }
        private void UpdateLevel()
        {
            if(m_skill)
            m_skill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));
        }

        public override void Active()
        {
            if (m_skill == null)
                m_skill = Instantiate(m_healSkill.gameObject, this.transform).GetComponent<FoodPowerSkill>();

            m_skill.SkillInit(Caster);
            m_skill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));

        }
        public override void Inactive()
        {
            if (m_skill)
            {
                m_healSkill.InactiveSkill();
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