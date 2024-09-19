using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power G Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power G Skill", order = 7)]

    public class FoodPowerGSkillData : SkillData
	{

        [field: Header("Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }



      


    }
}
