using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public interface IKnockbackable
    {
        public void OnKnockback(Vector3 hitPosition, float force, float knockBackDuration);
    }
}