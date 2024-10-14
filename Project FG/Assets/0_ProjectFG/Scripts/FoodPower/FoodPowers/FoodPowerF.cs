using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace JH
{
    [System.Serializable]

    public class FoodPowerF : FoodPower
    {
        [Header("Skill")]
        [SerializeField] private FoodPowerSkill m_areaSkill;


        // 레벨업 한 경우에 스킬을 다시 켜줘야한다.
        public override void LevelUp()
        {
            base.LevelUp();
            Inactive();
            Active();
        }


        public override void Active()
        {
            if (m_skill == null)
                m_skill = Instantiate(m_areaSkill.gameObject, this.transform).GetComponent<FoodPowerSkill>();
            m_skill.SkillInit(Caster);
            m_skill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));
        }
        public override void Inactive()
        {
            if (m_skill != null)
                m_skill.InactiveSkill();
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