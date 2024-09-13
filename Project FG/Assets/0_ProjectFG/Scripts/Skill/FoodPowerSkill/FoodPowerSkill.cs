using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public class FoodPowerSkill : SkillBase
	{
        [Header("푸드파워 레벨 데이터")]
        [SerializeField] protected FoodPowerLevelData m_levelData;

        public void SetLevel(FoodPowerLevelData levelData)
        {
            m_levelData = levelData;
        }
        protected virtual IEnumerator SkillRoutine()
        {
            float timer = 0;
            while (timer < m_levelData.Duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            InactiveSkill();
            yield break;
        }


    }
}
