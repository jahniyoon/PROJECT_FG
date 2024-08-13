using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "Food Power Data", menuName = "ScriptableObjects/Food Power")]

    public class FoodPowerData : ScriptableObject
    {
        [field: Header("푸드파워 데이터")]
        [field: SerializeField] public string FoodPowerName { get; private set; }
        [field: Tooltip("푸드 파워 아이콘\n푸드 파워 UI에 표시할 아이콘")]
        [field: SerializeField] public Sprite Icon { get; private set; }   // 체력
        [field: Tooltip("푸드 파워 재사용 대기 시간")]
        [field: SerializeField] public float CoolDown { get; private set; }

        [field: Header("스캔 범위")]
        [field: Tooltip("TargetNearest 방식의 스캔 시 스캔할 범위")]

        [field: SerializeField] public float TargetNearestScanRadius { get; private set; }
        [field: Tooltip("TargetNearest 타겟 검출 실패 시, 발사 여부")]
        [field: SerializeField] public bool AlwaysShoot { get; private set; }


    }
}