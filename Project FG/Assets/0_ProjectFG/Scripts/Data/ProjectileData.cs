using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "Projectile Data", menuName = "ScriptableObjects/Projectile/Projectile")]

    public class ProjectileData : ScriptableObject
    {
        [field: Header("투사체 데이터")]
        [field: SerializeField] public string ProjectileName { get; private set; }
        [field: Tooltip ("효과가 적용될 대상의 태그")]
        [field: SerializeField] public TargetTag TargetTag { get; private set; }
        [field: Tooltip("투사체의 데미지")]
        [field: SerializeField] public float Damage { get; private set; }        
        [field: Tooltip("투사체의 크기")]
        [field: SerializeField] public float ProjectileScale { get; private set; }
        [field: Tooltip("투사체의 속도")]
        [field: SerializeField] public float ProjectileSpeed { get; private set; }

        [field: Tooltip("투사체의 관통 가능 횟수")]
        [field: SerializeField] public int Penetrate { get; private set; }

        [field: Header("투사체 소멸")]
        [field: SerializeField] public DestroyType DestroyType { get; private set; }

        [field: Tooltip("투사체가 소멸하는 거리 / 시간")]
        [field: SerializeField] public float DestroyValue { get; private set; }



    }
}