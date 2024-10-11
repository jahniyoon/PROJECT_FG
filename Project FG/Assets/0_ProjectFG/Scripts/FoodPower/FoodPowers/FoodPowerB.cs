using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


namespace JH
{
    [System.Serializable]

    public class FoodPowerB : FoodPower
    {
        [Header("Skill")]
        [SerializeField] private FoodPowerSkill m_shieldSkill;
        private FoodPowerSkill m_skill;

        // 푸드파워 활성화
        public override void Active()
        {
            base.Active();
            if (m_skill == null)
                m_skill = Instantiate(m_shieldSkill.gameObject, this.transform).GetComponent<FoodPowerSkill>();

            m_skill.SkillInit(Caster);
            m_skill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));
        }
        // 푸드파워 비활성화

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