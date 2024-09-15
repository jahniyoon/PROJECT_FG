using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "GameSeggings", menuName = "ScriptableObjects/Setting/GameSettings")]

    public class GameSettings : ScriptableObject
    {
        [field: Header("게임 세팅")]
        [field: Tooltip("게임 오버 이후 리셋까지의 시간")]
        [field: SerializeField] public float ResetDuration { get; private set; }   // 사망 후 리셋 시간


        [field: Header("스테이지 사이즈")]
        [field: Tooltip("스테이지 가로 사이즈")]
        [field: SerializeField] public float StageWidth { get; private set; }   // 스테이지 가로
        [field: Tooltip("스테이지 세로 사이즈")]
        [field: SerializeField] public float StageLength { get; private set; }    // 스테이지 세로


        [field: Header("에네미 스폰 세팅")]
        [field: Tooltip("에네미 스폰 시작까지의 시간")]
        [field: SerializeField] public int StartEnemySpawnTime { get; private set; }    // 에네미 스폰 시간
        [field: Tooltip("에네미 스폰 웨이브의 간격")]
        [field: SerializeField] public int EnemySpawnInterval { get; private set; }   // 에네미 스폰 간격
        [field: Tooltip("한 웨이브에서 스폰할 에네미의 수")]
        [field: SerializeField] public int EnemySpawnCount { get; private set; }   // 에네미 스폰 수

        [field: Header("플레이어 체력")]
        [field: SerializeField] public float PlayerMaxHealth { get; private set; }   // 플레이어 체력

        [field: Header("플레이어 이동")]
        [field: Tooltip("플레이어의 이동 속도")]
        [field: SerializeField] public float PlayerMoveSpeed { get; private set; }   // 플레이어 이동속도
        [field: Tooltip("플레이어의 회전 속도")]
        [field: SerializeField] public float PlayerRotateSpeed { get; private set; }   // 플레이어 회전속도

        [field: Header("플레이어 공격")]
        [field: Tooltip("플레이어의 기본 공격 데미지")]
        [field: SerializeField] public float PlayerAttackDamage { get; private set; }   // 플레이어 공격력
        [field: Tooltip("플레이어의 공격 쿨타임")]
        [field: SerializeField] public float PlayerAttackCoolDown { get; private set; }
        [field: Tooltip("플레이어의 기본 공격 경직 유발시간")]
        [field: SerializeField] public float PlayerAttackStunDuration { get; private set; }   // 경직 유발 시간

        [field: Header("플레이어 포식")]
        [field: Tooltip("플레이어 포식 회복")]
        [field: SerializeField] public float PredationRestoreHealth { get; private set; }
        [field: Tooltip("플레이어 기준 검사 범위")]
        [field: SerializeField] public float PredationPlayerRange { get; private set; }
        [field: Tooltip("마우스 조준 기준 검사 범위")]
        [field: SerializeField] public float PredationAimRange { get; private set; }
        [field: Tooltip("포식 재사용 대기 시간")]
        [field: SerializeField] public float PredationCoolDown { get; private set; }
        [field: Tooltip("포식 대시\n포식할 대상의 위치까지 이동하는 시간")]
        [field: SerializeField] public float DashDuration { get; private set; }

        [field: Tooltip("포식 처형\n해당 시간동안 플레이어는 처형 애니메이션을 진행합니다.")]
        [field: SerializeField] public float PredationFatalityDuration { get; private set; }    // 처형 시간
        [field: Tooltip("포식 아이콘 사이즈\n에네미 위의 포식 아이콘 사이즈")]
        [field: SerializeField] public float PredationIconScale { get; private set; }

        [field: Header("플레이어 포만감")]
        [field: Tooltip("Max 포만감 게이지")]
        [field: SerializeField] public int MaxHunger { get; private set; }
        [field: Tooltip("포만감 소모기 범위")]
        [field: SerializeField] public float HungerSkillRange { get; private set; }
        [field: Tooltip("포만감 소모기 데미지")]
        [field: SerializeField] public float HungerSkillDamage { get; private set; }
        [field: Tooltip("포만감 소모기 스턴")]
        [field: SerializeField] public float HungerSkillStun { get; private set; }

        [field: Tooltip("포만감 소모기 딜레이")]
        [field: SerializeField] public float HungerSkillDelay { get; private set; }


        [field: Header("푸드파워")]
        [field: SerializeField] public FoodPower DefaultFoodPower { get; private set; }

        [field: Tooltip("푸드파워 딜레이")]
        [field: SerializeField] public float FoodPowerDelay { get; private set; }
        [field: Tooltip("푸드파워 투사체 조준 시스템")]
        [field: SerializeField] public FoodPowerAimType FoodPowerAimType { get; private set; }


    }

    public enum FoodPowerAimType
    {
        None,
        MoveDirection,      //  이동 방향
        TargetNearest,      // 가까운 타겟 방향
        PointerDirection,   // 포인터 방향
        RandomDirection,     // 랜덤 방향
        RandomEnemyDirection,    // 랜덤 적 방향
        Hit,                    // 피격시
        PcPosition,             // PC 위치에 소환
        PcRadius                // PC 주변

    }
}