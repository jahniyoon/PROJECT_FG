using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy G Data", menuName = "ScriptableObjects/Enemy/EnemyG", order = 7)]
    public class EnemyGData : EnemyData
    {
        [field: Header("에네미 E")]
        [field: SerializeField] public float ConversionMinTime { get; private set; }
        [field: SerializeField] public float ConversionMaxTime { get; private set; }
        [field: SerializeField] public SkillBase HealSkill { get; private set; }




    }
}