using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy A Data", menuName = "ScriptableObjects/Enemy/EnemyA", order = 1)]
    public class EnemyAData : EnemyData
    {

        [field: Header("에네미 A")]
        [field: Header("공격")]
        [field: SerializeField] public float AttackDamage { get; private set; }
        [field: SerializeField] public float AttackSpeed { get; private set; }
        [field: SerializeField] public float AttackCoolDown { get; private set; }
        [field: Header("근접 공격 범위")]
        [field: SerializeField] public float AttackOffset { get; private set; }
        [field: SerializeField] public float AttackRadius { get; private set; }
    }


}