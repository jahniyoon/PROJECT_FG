using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Projectile Skill", menuName = "ScriptableObjects/Skill/Projectile Skill")]

    public class ProjectileSkillData : SkillData
	{

        [field: Header("Slash Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }
        [field: SerializeField] public GameObject ProjectilePrefab { get; private set; }


      


    }
}
