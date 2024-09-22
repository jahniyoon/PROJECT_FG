using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;


namespace JH
{
    [System.Serializable]

    public class FoodPowerA : FoodPower
    {
        [Header("Skill")]
        [SerializeField] private FoodPowerSkill m_slashSkill;
        [SerializeField] private float m_activeDelay;
        private FoodPowerSkill m_activeSkill;



        Quaternion m_direction;
        public override void Active()
        {

            m_direction = GetDirection();
            if (m_direction == Quaternion.identity)
                return;

            base.Active();
            Invoke(nameof(ActiveSkill), m_activeDelay);



        }

        private void ActiveSkill()
        {
            if (this.gameObject.activeInHierarchy == false)
                return;

            Vector3 position = m_casterPosition.position;

            m_activeSkill = Instantiate(m_slashSkill.gameObject, position, m_direction, m_caster.transform).GetComponent<FoodPowerSkill>();
            m_activeSkill.SkillInit(m_caster.gameObject, m_casterPosition);
            m_activeSkill.SetFoodPowerData(m_data.GetLevelData(m_powerLevel));

            m_activeSkill.ActiveSkill();
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