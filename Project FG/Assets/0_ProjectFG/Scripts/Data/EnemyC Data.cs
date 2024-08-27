using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy C Data", menuName = "ScriptableObjects/EnemyC", order = 3)]
    public class EnemyCData : EnemyData
    {
        [field:Header("에네미 C")]
        [field: Header("공격")]
        [field: SerializeField] public float AttackCoolDown { get; private set; }
        [field: Header("투사체")]
        [field: SerializeField] public GameObject Projectile { get; private set; }


    }
}