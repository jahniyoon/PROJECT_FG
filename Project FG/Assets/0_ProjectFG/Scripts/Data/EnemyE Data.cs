using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Enemy E Data", menuName = "ScriptableObjects/Enemy/EnemyE", order = 5)]
    public class EnemyEData : EnemyData
    {
        [field: Header("에네미 E")]
        [field: Header("조준")]
        [field: SerializeField] public float AimSpeed { get; private set; }
        [field: SerializeField] public float AimAngle { get; private set; }
        [field: Tooltip("에임 배경 색")]
        [field: SerializeField] public Color OuterColor { get; private set; }
        [field: Tooltip("에임 슬라이더 색")]
        [field: SerializeField] public Color SliderColor { get; private set; }
        [field: Header("사격")]
        [field: SerializeField] public float ShootDamage { get; private set; }
        [field: SerializeField] public float ShootDuration { get; private set; }
        [field: SerializeField] public float FireRate { get; private set; }


    }
}