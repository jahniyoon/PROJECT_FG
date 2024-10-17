using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{
    public class FoodPowerSkill : SkillBase
    {
        [Header("푸드파워 레벨 데이터")]
        [SerializeField] protected FoodPowerLevelData m_foodPowerData;


        protected override void Init()
        {
            base.Init();
        }

        public void SetFoodPowerData(FoodPowerLevelData levelData)
        {
            m_foodPowerData = levelData;

            LevelDataChange();
        }


        protected virtual void LevelDataChange()
        {
            SetLevelData(m_foodPowerData.LevelData);
            SetDuration(m_foodPowerData.LevelData.Duration);
            m_skillCoolDown = m_foodPowerData.LevelData.CoolDown;

            for (int i = 0; i < m_buffs.Count; i++)
            {
                m_buffs[i].SetBuffValue(LevelData.TryGetBuffValues(i));
            }
        }


    }
}
