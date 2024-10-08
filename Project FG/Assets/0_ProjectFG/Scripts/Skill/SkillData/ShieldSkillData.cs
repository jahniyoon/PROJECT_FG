using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "ShieldSkill", menuName = "ScriptableObjects/Skill/Shield Skill")]

    public class ShieldSkillData : SkillData
	{
        [field: Header("Shield Skill")]
        [field: SerializeField] public BuffData ShieldBuff { get; private set; }
        [field: SerializeField] public GameObject ShieldPrefab { get; private set; }


    }
}
