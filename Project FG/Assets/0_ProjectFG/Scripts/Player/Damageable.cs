using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JH
{

    public class Damageable : MonoBehaviour, IDamageable
    {
        [Header("Health")]
        [SerializeField] private float m_maxHealth;
        [SerializeField] private float m_health;
        [SerializeField] private bool m_isDie;

        [SerializeField] private bool m_invincible;
        [SerializeField] private float m_damageReduction;   // 피해 감소

        public UnityEvent DamageEvent;
        public UnityEvent DieEvent;
        public UnityEvent UpdateHealthEvent;
    
    public float MaxHealth => m_maxHealth;
        public float Health => m_health;

        public bool IsDie => m_isDie;
        public float DamageReduction=> m_damageReduction;

        public void SetMaxHealth(float maxHealth)
        {
            m_maxHealth = maxHealth;
            m_health = maxHealth;

            UpdateHealthEvent?.Invoke();
        }

        public void RestoreHealth(float addHealth)
        {
            if (IsDie)
                return;

            if (m_maxHealth <= m_health)
                return;


            float restore = addHealth;
            m_health += addHealth;

            if (m_maxHealth <= m_health)
            {
                restore -= m_health - m_maxHealth;
                m_health = m_maxHealth;
            }
            UIManager.Instance.Debug.OnDamage(FinalDamage(restore), transform, true);

            UpdateHealthEvent?.Invoke();

        }


        public void OnDamage(float damage)
        {
            if (IsDie)
                return;

            if(m_invincible == false)
            m_health -= FinalDamage(damage);

            UIManager.Instance.Debug.OnDamage(FinalDamage(damage), transform);
            if (m_health <= 0)
            {
                Die();
                return;
            }
            DamageEvent?.Invoke();
            UpdateHealthEvent?.Invoke();
        }

        // 최종데미지
        private float FinalDamage(float damage)
        {
            float finalDamage = damage * (100 - m_damageReduction) * 0.01f;

            return finalDamage;
        }
        public void Die()
        {
            if (IsDie)
                return;

            m_health = 0;
            m_isDie = true;

            UpdateHealthEvent?.Invoke();
            DieEvent?.Invoke();
        }

        public void InvincibleMode()
        {
            m_invincible = !m_invincible;
        }

        public void SetDamageReduction(float value)
        {
            m_damageReduction += value;
        }

    }
}