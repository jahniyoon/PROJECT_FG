using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public class FoodPowerSkill : SkillBase
	{
        [Header("푸드파워 스킬")]
        [SerializeField] protected FoodPowerLevelData m_levelData;

        public void SetLevel(FoodPowerLevelData levelData)
        {
            m_levelData = levelData;
        }

	}
}
