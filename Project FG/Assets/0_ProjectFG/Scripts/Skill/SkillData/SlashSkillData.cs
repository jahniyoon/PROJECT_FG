using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Slash Skill", menuName = "ScriptableObjects/Skill/Slash Skill")]

    public class SlashSkillData : SkillData
	{

        [field: Header("Slash Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }


      


    }
}
