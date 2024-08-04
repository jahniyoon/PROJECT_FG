using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable 
{
    public int Health { get; }
    public bool IsDie {  get; }
    public void OnDamage(int damage);

    public void Die();
}
