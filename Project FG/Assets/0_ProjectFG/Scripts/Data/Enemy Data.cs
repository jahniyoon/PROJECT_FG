using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObjects/EnemyDefault", order = 0) ]

    public class EnemyData : ScriptableObject
    {
        [field: Header("에네미 정보")]
        [field: SerializeField] public string Name { get; private set; }
        [field: Tooltip("최대 체력 및 체력")]
        [field: SerializeField] public int Health { get; private set; }
        [field: Tooltip("이동 속도")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: Tooltip("회전 속도")]
        [field: SerializeField] public float RotateSpeed { get; private set; }



        [field: Header("에네미 범위")]
        [field: Tooltip("공격 범위\n해당 범위 내에 타겟이 있으면 공격 상태로 전환한다.")]
        [field: SerializeField] public float AttackRange { get; private set; }
        [field: Tooltip("회피 범위\n해당 범위 내에 타겟이 있으면 회피한다.")]
        [field: SerializeField] public float EscapeRange { get; private set; }



        [field: Header("포식 가능 상태")]
        [field : Tooltip("포식 가능한 상태의 체력 비율")]
        [field: Range(1,100)]
        [field: SerializeField] public float PredationHealthRatio { get; private set; }
        [field: Tooltip("포식상태 돌입 시 쿨다운")]

        [field: SerializeField] public float PredationStunCoolDown { get; private set; }

        [field: Header("푸드 파워")]
        [field: SerializeField] public FoodPower FoodPower { get; private set; }


    }
}