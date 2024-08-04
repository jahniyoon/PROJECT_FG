using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{

    public class Damageable : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] private int m_maxHealth;
        [SerializeField] private int m_health;
        [SerializeField] private bool m_isDie;

        [SerializeField] private bool m_invincible;

        public UnityEvent DamageEvent;
        public UnityEvent DieEvent;
        public UnityEvent UpdateHealthEvent;
    
    public int MaxHealth => m_maxHealth;
        public int Health => m_health;

        public bool IsDie => m_isDie;

        public void SetMaxHealth(int maxHealth)
        {
            m_maxHealth = maxHealth;
            m_health = maxHealth;

            UpdateHealthEvent?.Invoke();
        }

        public void RestoreHealth(int addHealth)
        {
            if (IsDie)
                return;

            m_health += addHealth;

            if(m_maxHealth <= m_health)
            {
                m_health = m_maxHealth;
            }

            UpdateHealthEvent?.Invoke();
        }


        public void OnDamage(int damage)
        {
            if (IsDie)
                return;
            if(m_invincible == false)
            m_health -= damage;

            if (m_health <= 0)
            {
                m_health = 0;
                Die();
            }

            DamageEvent?.Invoke();
            UpdateHealthEvent?.Invoke();
        }
        public void Die()
        {
            if (IsDie)
                return;

            m_isDie = true;
            DieEvent?.Invoke();
        }

        public void InvincibleMode(bool enable)
        {
            m_invincible = enable;
        }

    }
}