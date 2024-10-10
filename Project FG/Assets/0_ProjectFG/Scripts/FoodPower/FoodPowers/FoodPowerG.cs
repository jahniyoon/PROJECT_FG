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
        [SerializeField] private FoodPowerSkill m_skill;
        private FoodPowerSkill m_activeSkill;


        public override void Init(bool isMain = false)
        {
            base.Init(isMain);
            LevelUpEvent.AddListener(UpdateLevel);
        }
        private void UpdateLevel()
        {
            if(m_activeSkill)
            m_activeSkill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));
        }

        public override void Active()
        {
            Vector3 position = Caster.Transform.position;

            Quaternion direction = GetDirection();

            m_activeSkill = Instantiate(m_skill.gameObject, position, direction, Caster.Transform).GetComponent<FoodPowerSkill>();
            m_activeSkill.SkillInit(Caster);
            m_activeSkill.SetFoodPowerData(m_data.GetFoodPowerLevelData(m_powerLevel));

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