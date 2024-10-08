using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class CollisionProjectile : ProjectileBase
    {
        [Header("Collision")]
        [SerializeField] bool m_isStopDestroy;

        [Header("Particles")]
        [SerializeField] private ParticleSystem[] m_collisionParticles;
        [SerializeField] private VisualEffect[] m_collisionEffect;

        public override void ActiveProjectile()
        {

            Collision();
        }


        public void Collision()
        {
            // 캐스터 타겟이면 사용하지 않는다.
            if (m_skill.Data.SkillTarget == TargetTag.Caster)
                return;

            Collider[] colls = Physics.OverlapSphere(transform.position + transform.forward * m_skill.Data.ProjectileOffset, m_skill.Data.SkillRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }


                if (colls[i].CompareTag(m_skill.Data.SkillTarget.ToString()))
                {
                    // 180도만 제한한다.
                    if (GFunc.TargetAngleCheck(transform, colls[i].transform, m_skill.Data.SkillArc) == false)
                        continue;


                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_skill.Data.SkillDamage);
                    }

                    var knockBackForce = m_skill.Data.TryGetBuffValue(0);
                    var knockBackDuration = m_skill.Data.TryGetBuffValue(1);


                    // 버프도 같이 보낸다.
                    m_skill.OnBuff(colls[i].transform);
                    m_skill.RemoveBuff(colls[i].transform);
                }
            }

            foreach (var effect in m_collisionParticles)
            {
                effect.Stop();
                effect.Play();
            }
            foreach (var effect in m_collisionEffect)
            {
                effect.Stop();
                effect.Play();
            }

            Invoke(nameof(StopEffect), m_skill.Data.SkillLifeTime);
        }

        private void StopEffect()
        {
            foreach (var effect in m_collisionParticles)
            {
                effect.Stop();
            }            
            foreach (var effect in m_collisionEffect)
            {
                effect.Stop();
            }

            this.gameObject.SetActive(false);

            if (m_isStopDestroy)
                Destroy(gameObject);

        }
    }

}
