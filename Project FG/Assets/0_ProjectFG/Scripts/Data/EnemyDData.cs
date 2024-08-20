using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy D Data", menuName = "ScriptableObjects/EnemyD", order = 4)]
    public class EnemyDData : EnemyData
    {
        [field: Header("에네미 D")]
        [field: SerializeField] public float AttackRadius { get; private set; }
        [field: SerializeField] public float EscapeRadius { get; private set; }
    }
}