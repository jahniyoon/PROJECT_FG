using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public class FoodPowerSkill : SkillBase
	{
        [Header("푸드파워 레벨 데이터")]
        [SerializeField] protected FoodPowerLevelData m_levelData;
        [Header("푸드파워 조준 타입")]
        [SerializeField] protected AimType m_aimtype;


        public void SetFoodPowerData(FoodPowerLevelData levelData)
        {
            m_levelData = levelData;
            m_aimtype = m_levelData.AimType;
        }
        protected override IEnumerator ActiveSkillRoutine(float duration = 0)
        {
            float timer = 0;

            // 듀레이션이 0보다 작으면 끄지 않는다.
            if(m_levelData.Duration < 0)
                yield break;

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
