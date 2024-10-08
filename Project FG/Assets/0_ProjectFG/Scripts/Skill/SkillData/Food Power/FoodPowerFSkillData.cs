using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power F Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power F Skill", order = 6)]

    public class FoodPowerFSkillData : SkillData
	{

        [field: Header("Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }
        [field: SerializeField] public BuffData SlowDebuff { get; private set; }






    }
}
