using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power H Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power H Skill", order = 8)]

    public class FoodPowerHSkillData : SkillData
	{

        [field: Header("Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }


      


    }
}
