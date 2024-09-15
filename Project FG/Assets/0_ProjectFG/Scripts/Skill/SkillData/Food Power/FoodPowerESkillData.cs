using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power E Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power E Skill", order = 5)]

    public class FoodPowerESkillData : SkillData
	{

        [field: Header("Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }


      


    }
}
