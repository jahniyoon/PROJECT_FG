using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy D Data", menuName = "ScriptableObjects/EnemyD", order = 4)]
    public class EnemyDData : EnemyData
    {
        [field:Header("Enemy D 스킬")]
        [field:Tooltip("스킬의 외부 범위")]
        [field: SerializeField] public float OuterRadius { get; private set; }
        [field:Tooltip("스킬의 내부 범위")]
        [field: SerializeField] public float InnerRadius { get; private set; }
        [field:Tooltip("스킬의 속도")]
        [field: SerializeField] public float SliderDuration { get; private set; }
        [field: Header("스킬 컬러")]

        [field: Tooltip("스킬 배경 색")]
        [field: SerializeField] public Color OuterColor { get; private set; }
        [field: Tooltip("스킬 슬라이더 색")]
        [field: SerializeField] public Color SliderColor { get; private set; }


    }
}