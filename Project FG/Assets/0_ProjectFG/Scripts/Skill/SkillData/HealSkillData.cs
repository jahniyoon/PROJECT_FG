using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Heal Skill", menuName = "ScriptableObjects/Skill/Heal Skill")]

    public class HealSkillData : SkillData
	{
        [field: Header("Heal Skill")]
        [field: SerializeField] public TargetTag Target { get; private set; }

        [field: SerializeField] public BuffBase HealBuff { get; private set; }
        [field: SerializeField] public float SkillRadius { get; private set; }
        [field: SerializeField] public ParticleSystem HealPrefab { get; private set; }


    }
}
