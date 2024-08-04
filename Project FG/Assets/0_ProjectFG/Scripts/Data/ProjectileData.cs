using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "Projectile Data", menuName = "ScriptableObjects/Projectile")]

    public class ProjectileData : ScriptableObject
    {
        [field: Header("투사체 데이터")]
        [field: SerializeField] public string ProjectileName { get; private set; }   
        [field: SerializeField] public TargetTag TargetTag { get; private set; }
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public float ProjectildSpeed { get; private set; }
        [field: SerializeField] public float DestroyTime { get; private set; }
        [field: SerializeField] public int Penetrate { get; private set; }

      


    }
}