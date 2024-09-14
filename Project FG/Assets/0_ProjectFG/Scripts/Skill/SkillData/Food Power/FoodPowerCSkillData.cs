using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Food Power C Skill", menuName = "ScriptableObjects/Skill/Food Power/Food Power C Skill")]

    public class FoodPowerCSkillData : SkillData
	{

        [field: Header("Slash Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }


      


    }
}
