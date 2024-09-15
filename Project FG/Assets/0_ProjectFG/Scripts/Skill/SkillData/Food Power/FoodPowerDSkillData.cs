using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power D Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power D Skill", order = 4)]

    public class FoodPowerDSkillData : SkillData
	{

        [field: Header("Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }
        [field: SerializeField] public GameObject GrenadePrefab { get; private set; }

      


    }
}
