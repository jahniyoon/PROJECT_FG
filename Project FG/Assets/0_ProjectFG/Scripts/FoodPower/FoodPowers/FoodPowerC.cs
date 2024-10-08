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
        private FoodPowerSkill m_activeSkill;

        public override void Active()
        {

            Vector3 position = m_casterPosition.position;
            position.y += m_offset;

            Quaternion direction = GetDirection();

            m_activeSkill = Instantiate(m_projectileSkill.gameObject, position, direction, m_caster.transform).GetComponent<FoodPowerSkill>();
            m_activeSkill.SkillInit(m_caster.gameObject, m_casterPosition);
            m_activeSkill.SetFoodPowerData(m_data.GetLevelData(m_powerLevel));
            m_activeSkill.LeagcyActiveSkill();

            // 투사체를 발사하고 사라진다.
            Inactive();

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