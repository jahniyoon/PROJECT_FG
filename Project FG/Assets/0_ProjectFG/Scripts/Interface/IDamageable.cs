using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    public float Health { get; }
    public bool IsDie {  get; }
    public void OnDamage(float damage);

    public void Die();
}
