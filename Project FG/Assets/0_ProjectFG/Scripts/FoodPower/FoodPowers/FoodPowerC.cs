using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [System.Serializable]

    public class FoodPowerC : FoodPower
    {
        [Header("Test Projectile")]
        [SerializeField] private FoodPowerSkill m_projectileSkill;
        [SerializeField] private float m_offset = 0.5f;


        public override void Active()
        {

            Vector3 position = m_casterPosition.position;
            position.y += m_offset;

            Quaternion direction = GetDirection();



            FoodPowerSkill skill = Instantiate(m_projectileSkill.gameObject, position, direction, m_caster.transform).GetComponent<FoodPowerSkill>();
            skill.SkillInit(m_caster.gameObject, m_casterPosition);
            skill.SetFoodPowerData(m_data.GetLevelData(m_powerLevel));
            skill.ActiveSkill();

        }

    }
}