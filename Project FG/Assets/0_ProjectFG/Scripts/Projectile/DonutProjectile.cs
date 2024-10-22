using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class DonutProjectile : ProjectileBase
    {
        [Header("Donut")]
        [SerializeField] private DonutShader m_donutEffect;
        [SerializeField] private ParticleSystem[] m_collisionParticles;
        [SerializeField] private float m_outerRadius;
        [SerializeField] private float m_innerRadius;
        public override void ActiveProjectile()
        {
            ShootDonut();
        }

        public void ShootDonut()
        {
            m_outerRadius = m_radius;
            m_innerRadius = m_skill.LevelData.TryGetValue1();

            m_donutEffect.SetRadius(m_outerRadius, m_innerRadius);

            float duration = m_skill.LevelData.TryGetValue1(1);
            StartCoroutine(AimRoutine(m_skill.LevelData.Damage, duration));
        }



        IEnumerator AimRoutine(float damage, float duration)
        {
            float timer = 0;
            m_donutEffect.SetActive(true);

            while (timer < duration)
            {
                m_donutEffect.SetSlider(timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }
            m_donutEffect.SetActive(false);
            Explosion(damage);


            timer = 0;

            while(timer < m_skill.LevelData.LifeTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            InActiveProjectile();
            yield break;
        }

        public void Explosion(float damage)
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_radius);
            Vector3 targetPos;

            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag(m_skill.Data.SkillTarget.ToString()))
                {
                    targetPos = colls[i].transform.position;
                    targetPos.y = transform.position.y;

                    float targetDistance = Vector3.Distance(targetPos, transform.position);

                    if (targetDistance < m_innerRadius)
                        continue;

                    if (colls[i].TryGetComponent<IDamageable>(out IDamageable damageable))
                        damageable.OnDamage(m_skill.Caster.FinalDamage(damage));

                   OnBuff(colls[i].transform);
                   RemoveBuff(colls[i].transform);
                }
            }

            foreach(var effect in m_collisionParticles)
            {
                effect.Stop();
                effect.Play();

            }
        }

        public override ProjectileBase InActiveProjectile()
        {
            Destroy(this.gameObject);
            return this;
        }
    }

}
