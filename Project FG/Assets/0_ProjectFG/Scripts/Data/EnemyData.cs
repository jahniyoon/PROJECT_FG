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
        [field: Tooltip("최대 체력 및 체력")]
        [field: SerializeField] public int Health { get; private set; }
        [field: Tooltip("이동 속도")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: Tooltip("회전 속도")]
        [field: SerializeField] public float RotateSpeed { get; private set; }

        [field: Header("에네미 공격")]
        [field: Tooltip("공격 데미지")]
        [field: SerializeField] public float AttackSpeed { get; private set; }
        [field: Tooltip("공격 범위\n해당 범위 내에 타겟이 있으면 공격 상태로 전환한다.")]
        [field: SerializeField] public int AttackDamage { get; private set; }
        [field: Tooltip("공격 시작까지의 속도\n공격 상태에 돌입 후 해당 시간만큼 시간이 지나야 공격이 실행. 만약 공격 시작까지의 속도 중 데미지를 입으면 타이머를 초기화합니다.")]
        [field: SerializeField] public float AttackRange { get; private set; }
        [field: Tooltip("공격이 끝난 뒤 간격")]

        [field: SerializeField] public float AttackCoolDown { get; private set; }

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