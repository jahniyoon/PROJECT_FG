using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy C Data", menuName = "ScriptableObjects/EnemyC", order = 3)]
    public class EnemyCData : EnemyData
    {
        [field:Header("Enemy C 스킬")]
        [field:Tooltip("스킬의 외부 범위")]
        [field: SerializeField] public GameObject Projectile { get; private set; }


    }
}