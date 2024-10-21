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
        [SerializeField] private float m_HitdamageDecrease;   // 피해 감소
        [SerializeField] private Color m_damageEffectColor = Color.white;
        [SerializeField] private bool m_execution;

        public UnityEvent DamageEvent;
        public UnityEvent DieEvent;
        public UnityEvent<Damageable> DieDamageableEvent;
        public UnityEvent UpdateHealthEvent;


        public float MaxHealth => m_maxHealth;
        public float Health => m_health;

        public bool IsDie => m_isDie;
        public bool Excution => m_execution;
        public float HitDamageDecrease => m_HitdamageDecrease;

        public void SetMaxHealth(float maxHealth)
        {
            m_maxHealth = maxHealth;
            m_health = maxHealth;

            UpdateHealthEvent?.Invoke();
        }
        public void SetHealth(float health)
        {
            m_health = health;

            UpdateHealthEvent?.Invoke();
        }


        public void RestoreHealth(float addHealth)
        {
            if (IsDie)
                return;

            if (m_maxHealth <= m_health)
                return;

            float beforeHealth = m_health;

            m_health += addHealth;

            if (m_maxHealth < m_health)
            {
                m_health = m_maxHealth;
            }

            UIManager.Instance.Debug.OnDamage((m_health - beforeHealth), transform, Color.green);

            UpdateHealthEvent?.Invoke();

        }
        public void OnDamage(float damage)
        {
            OnDamage(damage, false);
        }
        public void OnDamage(float damage, Color color)
        {
            OnDamage(damage, false, color);
        }


        public void OnDamage(float damage, bool Execution = false, Color color = default)
        {
            if (color == default)
                color = m_damageEffectColor;

            if (IsDie)
                return;

            float finalDamage = FinalDamage(damage);

            if (m_invincible == false)
            {
                if (Execution)
                    finalDamage = damage;

                m_health -= finalDamage;
            }

            if(Excution)
            m_execution = Execution;

            UIManager.Instance.Debug.OnDamage(finalDamage, transform, color);
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
            float finalDamage = damage * (100 - m_HitdamageDecrease) * 0.01f;

            return finalDamage;
        }
        public void Die()
        {
            if (IsDie)
                return;
            DieEvent?.Invoke();
            UpdateHealthEvent?.Invoke();

            if (m_execution == false)
            {
                DieDamageableEvent?.Invoke(this);
            }

            m_health = 0;
            m_isDie = true;
            UpdateHealthEvent?.Invoke();

        }

        public void InvincibleMode()
        {
            m_invincible = !m_invincible;
        }

        public void SetDamageReduction(float value)
        {
            m_HitdamageDecrease += value;
        }

    }
}