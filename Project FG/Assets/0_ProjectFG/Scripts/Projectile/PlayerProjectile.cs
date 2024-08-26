using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PlayerProjectile : Projectile
    {
        protected override bool IgnoreCollider(Collider other)
        {
            // 적이면 적 관통하도록 하기
            if (m_targetTag == TargetTag.Enemy)
                return other.isTrigger || other.CompareTag("Player");

            return other.isTrigger || other.CompareTag("Player");
        }

    }
}