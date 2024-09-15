using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class Grenade : Projectile
    {
        [Header("Grenade")]
        public ParticleSystem ExplosionEffect;

        // 아무것도 안한다.
        protected override void TriggerEnter(Collider other)
        {
            // DO NOTHING
        }
        float timer;
        protected override void UpdatePosition()
        {
            transform.position = Vector3.Lerp(m_spawnPos, m_targetPosition, timer / m_destroyTime);
            timer += Time.deltaTime;
        }

        protected override void DestroyProjectile()
        {
            Explosion();
            base.DestroyProjectile();
        }
        private void Explosion()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_projectileScale);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag(m_targetTag.ToString()))
                {
                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_damage);
                    }
                }
            }



            var explosion = Instantiate(ExplosionEffect.gameObject, transform.position, transform.rotation, GameManager.Instance.ProjectileParent).GetComponent<ParticleSystem>();
            explosion.transform.localScale = Vector3.one * m_projectileScale;
            explosion.Play();
            Destroy(explosion.gameObject, 1f);
        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}
