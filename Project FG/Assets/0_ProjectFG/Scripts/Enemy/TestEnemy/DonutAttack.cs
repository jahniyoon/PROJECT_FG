using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class DonutAttack : MonoBehaviour
    {
        RingEffect m_ring;
        [SerializeField] private ParticleSystem m_effect;
        [SerializeField] float m_outerRadius;
        float m_innerRadius;

        private void Awake()
        {
            m_ring = transform.GetComponentInChildren<RingEffect>();
        }

        public void SkillInit(float damage, float outer, float inner, float duration)
        {
            m_outerRadius = outer;
            m_innerRadius = inner;

            m_ring.SetRadius(m_outerRadius, m_innerRadius);
            StartCoroutine(ShootRoutine(damage, duration));
        }
        public void SetColor(Color outer, Color inner)
        {
            m_ring.SetColor(outer, inner);
        }

        public void Explosion(float damage)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_outerRadius * 0.5f);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag("Player"))
                {
                    if (Vector3.Distance(transform.position, colls[i].transform.position) < m_innerRadius * 0.5f)
                        continue;

                    if (colls[i].TryGetComponent<IDamageable>(out IDamageable damageable))
                        damageable.OnDamage(damage);
                }
            }
        }



        IEnumerator ShootRoutine(float duration, float damage)
        {
            float timer = 0;

            while (timer < duration)
            {
                m_ring.SetSlider(timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            Explosion(damage);

            if (m_effect)
                m_effect.Play();

            Destroy(gameObject, 1);

            yield break;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, m_outerRadius);
        }

    }
}