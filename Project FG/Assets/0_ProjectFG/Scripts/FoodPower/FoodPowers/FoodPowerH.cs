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
        [SerializeField] private FoodPowerSkill m_skill;



        public override void Active()
        {
            Vector3 position = m_casterPosition.position;

            Quaternion direction = GetDirection();
            

            var skill = Instantiate(m_skill.gameObject, position, direction, GameManager.Instance.ProjectileParent).GetComponent<FoodPowerSkill>();
            skill.SkillInit(m_caster.gameObject, m_casterPosition);
            skill.SetFoodPowerData(m_data.GetLevelData(m_powerLevel));

            skill.ActiveSkill();
        }

    }
}