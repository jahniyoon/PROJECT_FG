using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy B Data", menuName = "ScriptableObjects/Enemy/EnemyB", order = 2)]
    public class EnemyBData : EnemyData
    {
        [field: Header("에네미 B")]
        [field: Header("공격")]
        [field: SerializeField] public int AttackDamage { get; private set; }
        [field: SerializeField] public int AttackSpeed { get; private set; }
        [field: SerializeField] public int AttackCoolDown { get; private set; }
        [field: Header("근접 공격 범위")]
        [field: SerializeField] public float AttackOffset { get; private set; }
        [field: SerializeField] public float AttackRadius { get; private set; }

        [field: Header("공격 막기 버프")]
        [field: SerializeField] public SkillBase ShieldSkill { get; private set; }
        [field: Header("넉백")]
        [field: SerializeField] public float KnockBackDistance { get; private set; }
        [field: SerializeField] public float KnockBackDuration { get; private set; }
    }
}