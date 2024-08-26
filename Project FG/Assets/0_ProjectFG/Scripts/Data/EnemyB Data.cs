using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy B Data", menuName = "ScriptableObjects/EnemyB", order = 2)]
    public class EnemyBData : EnemyData
    {
        [field: Header("에네미 B")]
        [field: Header("공격")]
        [field: SerializeField] public float AttackOffset { get; private set; }
        [field: SerializeField] public float AttackRadius { get; private set; }

        [field: Header("공격 막기 버프")]
        [field: SerializeField] public float BuffCoolDown { get; private set; }
        [field: SerializeField] public float BuffDuration { get; private set; }
        [field: SerializeField] public float DamageReduction { get; private set; }
        [field: Header("넉백")]
        [field: SerializeField] public float KnockBackDistance { get; private set; }
        [field: SerializeField] public float KnockBackDuration { get; private set; }
    }
}