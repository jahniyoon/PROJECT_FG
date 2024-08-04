using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "GameSeggings", menuName = "ScriptableObjects/GameSettings")]

    public class GameSettings : ScriptableObject
    {
        [field: Header("게임 세팅")]
        [field: SerializeField] public float ResetDuration { get; private set; }   // 사망 후 리셋 시간

        [field: Header("스테이지 사이즈")]
        [field: SerializeField] public float StageWidth { get; private set; }   // 스테이지 가로
        [field: SerializeField] public float StageLength { get; private set; }    // 스테이지 세로


        [field: Header("에네미 스폰 세팅")]
        [field: SerializeField] public int StartEnemySpawnTime { get; private set; }    // 에네미 스폰 시간
        [field: SerializeField] public int EnemySpawnInterval { get; private set; }   // 에네미 스폰 간격
        [field: SerializeField] public int EnemySpawnCount { get; private set; }   // 에네미 스폰 수

        [field: Header("플레이어 체력")]
        [field: SerializeField] public int PlayerMaxHealth { get; private set; }   // 플레이어 체력

        [field: Header("플레이어 이동")]
        [field: SerializeField] public float PlayerMoveSpeed { get; private set; }   // 플레이어 이동속도
        [field: SerializeField] public float PlayerRotateSpeed { get; private set; }   // 플레이어 회전속도
        
        [field: Header("플레이어 공격")]
        [field: SerializeField] public int PlayerAttackDamage { get; private set; }   // 플레이어 공격력
        [field: SerializeField] public float PlayerAttackCoolDown { get; private set; }   // 플레이어 공격력
        [field: SerializeField] public float PlayerAttackStunDuration { get; private set; }   // 경직 유발 시간

        [field: Header("플레이어 포식")]

        [field: SerializeField] public float PredationRange { get; private set; }
        [field: SerializeField] public float PredationCoolDown { get; private set; }
        [field: Header("포식 : 돌진")]
        [field: SerializeField] public float DashDuration { get; private set; }

        [field: Header("포식 : 처형")]
        [field: SerializeField] public float PredationDuration { get; private set; }    // 처형 시간

        [field: Header("플레이어 포만감")]
        [field: SerializeField] public int MaxHunger { get; private set; }   // 플레이어 공격력


    }
}