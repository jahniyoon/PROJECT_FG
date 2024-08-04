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
        [field: SerializeField] public Sprite Icon { get; private set; }   // 체력
        [field: SerializeField] public float CoolDown { get; private set; }

      


    }
}