using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class Grenade : DefaultProjectile
    {
        [Header("Grenade")]
        public ParticleSystem ExplosionEffect;
        Vector3 m_targetPos;

        // 아무것도 안한다.
        protected override void TriggerEnter(Collider other)
        {
            // DO NOTHING
        }
        float timer;
        protected override void UpdatePosition()
        {
            m_targetPos = this.transform.position;
            if(m_skill.Target != null)
            {
                m_targetPos = m_skill.Target.position;
                m_targetPos.y = transform.position.y;
            }

            transform.position = Vector3.Lerp(m_spawnPos, m_targetPos, timer / m_skill.Data.SkillLifeTime);
            timer += Time.deltaTime;
        }

        protected override void DestroyProjectile()
        {
            Explosion();
            base.DestroyProjectile();
        }
        private void Explosion()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_skill.Data.SkillRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag(m_skill.Data.SkillTarget.ToString()))
                {
                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_skill.Data.SkillDamage);
                    }
                }
            }



            var explosion = Instantiate(ExplosionEffect.gameObject, transform.position, transform.rotation, GameManager.Instance.ProjectileParent).GetComponent<ParticleSystem>();
            explosion.transform.localScale = Vector3.one * m_data.ProjectileScale;
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
