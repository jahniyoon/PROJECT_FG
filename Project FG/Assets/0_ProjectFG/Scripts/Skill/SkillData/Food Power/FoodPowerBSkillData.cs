using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power B Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power B Skill", order = 2)]

    public class FoodPowerBSkillData : SkillData
	{

        [field: Header("Slash Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }

        [field: SerializeField] public BuffBase KnockbackBuff { get; private set; }
        [field: SerializeField] public BuffBase DamageReductionBuff { get; private set; }
      


    }
}
