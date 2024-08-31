using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy F Data", menuName = "ScriptableObjects/EnemyF", order = 6)]
    public class EnemyFData : EnemyData
    {
        [field: Header("에네미 F")]
        [field: SerializeField] public ParticleSystem SkillEffect { get; private set; }
        [Tooltip("틱당 데미지")]
        [field: SerializeField] public float SkillTickDamage { get; private set; }
        [Tooltip("틱 간격")]
        [field: SerializeField] public float SkillTickDuration { get; private set; }
        [Tooltip("스킬 범위")]
        [field: SerializeField] public float SkillAreaRadius { get; private set; }
    }
}