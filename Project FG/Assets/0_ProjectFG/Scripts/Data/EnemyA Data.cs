using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy A Data", menuName = "ScriptableObjects/EnemyA", order = 1)]
    public class EnemyAData : EnemyData
    {
        [field: Header("에네미 A")]
        [field: SerializeField] public float AttackOffset { get; private set; }
        [field: SerializeField] public float AttackRadius { get; private set; }
    }
}