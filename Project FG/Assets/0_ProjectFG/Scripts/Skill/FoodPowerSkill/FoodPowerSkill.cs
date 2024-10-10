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
            SetLevelData(levelData.LevelData);


            SetDuration(LevelData.Duration);
            m_skillCoolDown = levelData.LevelData.CoolDown;
  
            LevelDataChange();
        }


        protected virtual void LevelDataChange() { }


    }
}
