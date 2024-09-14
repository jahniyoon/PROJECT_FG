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
        [SerializeField] private FoodPowerSkill m_slashSkill;



        public override void Active()
        {
            Vector3 position = m_casterPosition.position;

            Quaternion direction = GetDirection();

            var skill = Instantiate(m_slashSkill.gameObject, position, direction, m_caster.transform).GetComponent<FoodPowerSkill>();
            skill.SkillInit(m_caster.gameObject, m_casterPosition);
            skill.SetLevel(m_data.GetLevelData(m_powerLevel));

            skill.ActiveSkill();
        }

    }
}