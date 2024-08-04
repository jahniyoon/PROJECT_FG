using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObjects/Enemy")]

    public class EnemyData : ScriptableObject
    {
        [field: Header("에네미 정보")]
        [field: SerializeField] public string Name { get; private set; }   
        [field: SerializeField] public int Health { get; private set; }   // 체력
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: SerializeField] public float RotateSpeed { get; private set; }

        [field: Header("에네미 공격")]
        [field: SerializeField] public int AttackDamage { get; private set; }   
        [field: SerializeField] public float AttackSpeed { get; private set; }   
        [field: SerializeField] public float AttackRange { get; private set; }   
        [field: SerializeField] public float AttackCoolDown { get; private set; }

        [field: Header("그로기")]
        [field: Range(1,100)]
        [field: SerializeField] public float GroggyHealthRatio { get; private set; }
        [field: SerializeField] public float GroggyStunTime { get; private set; }

        [field: Header("푸드 파워")]
        [field: SerializeField] public FoodPower FoodPower { get; private set; }


    }
}