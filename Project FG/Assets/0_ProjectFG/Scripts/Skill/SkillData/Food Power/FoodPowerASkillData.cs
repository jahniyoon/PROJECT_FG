using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power A Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power A Skill", order = 1)]

    public class FoodPowerASkillData : SkillData
	{

        [field: Header("Slash Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }


      


    }
}
