using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;


namespace JH
{
    [CreateAssetMenu(fileName = "Food Power Data", menuName = "ScriptableObjects/Food Power/Food Power")]

    public class FoodPowerData : ScriptableObject
    {
        [field: Header("푸드파워 데이터")]
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string FoodPowerName { get; private set; }
        [field: Tooltip("푸드 파워 아이콘\n푸드 파워 UI에 표시할 아이콘")]
        [field: SerializeField] public Sprite Icon { get; private set; }   // 체력
        [field: Header("푸드파워 레벨 데이터")]
        [field: SerializeField] public FoodPowerLevelData[] LevelData { get; private set; }

        [field: Header("스캔 범위")]
        [field: Tooltip("TargetNearest 방식의 스캔 시 스캔할 범위")]

        [field: SerializeField] public float TargetNearestScanRadius { get; private set; }
        [field: Tooltip("TargetNearest 타겟 검출 실패 시, 발사 여부")]
        [field: SerializeField] public bool AlwaysShoot { get; private set; }


        public FoodPowerLevelData GetLevelData(int level)
        {
            if (LevelData.Length < level)
            {
                Debug.Log($"{FoodPowerName}의 '{level}'번째 레벨데이터를 찾을 수 없습니다.");
                return LevelData[LevelData.Length];
            }
            return LevelData[level];
        }

    }

    [System.Serializable]
    public class FoodPowerLevelData
    {
        public string Name;
        public float CoolDown;
        public float Damage;
        public float Range;
        public float Radius;
        public float Duration;
        public int Count;
        public FoodPowerAimType AimType;
        public float[] Values;


        public string Json()
        {
            return JsonUtility.ToJson(this);
        }
        public float GetValue(int num)
        {
            if (Values.Length < num - 1)
            {
                Debug.Log($"{Name}의 '{num}'번째 값을 찾을 수 없습니다.");
                return 0;
            }

            return Values[num];
        }
    }
}