using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy H Data", menuName = "ScriptableObjects/Enemy/EnemyH", order = 8)]
    public class EnemyHData : EnemyData
    {
        [field: Header("에네미 H")]
        [field: SerializeField] public float AttackDamage { get; private set; }
        [field: SerializeField] public float AttackSpeed { get; private set; }

        [field: SerializeField] public float ExplosionRadius { get; private set; }
    }
}