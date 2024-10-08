using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class AOEProjectile : ProjectileBase
    {

        private Rigidbody m_rigid;
        private SphereCollider m_collider;
        [Header("AOEProjectile")]
        [SerializeField] private List<Transform> m_aoeTargets = new List<Transform>();


        [SerializeField] bool m_isDestroy;

        [Header("Particles")]
        [SerializeField] private ParticleSystem[] m_collisionParticles;
        [SerializeField] private VisualEffect[] m_collisionEffect;


        protected override void AwakeInit()
        {
            base.AwakeInit();
            m_rigid = transform.AddComponent<Rigidbody>();
            m_rigid.isKinematic = true;

            m_collider = transform.AddComponent<SphereCollider>();
            m_collider.isTrigger = true;
            m_aoeTargets.Clear();
        }
        public override void SetSkill(SkillBase skill)
        {
            base.SetSkill(skill);
            m_collider.radius = m_skill.Data.SkillRadius;


        }
        public override void ActiveProjectile()
        {

            foreach (var effect in m_collisionParticles)
            {
                effect.Stop();
                effect.Play();
            }
        }
        public override ProjectileBase InActiveProjectile()
        {
            StopEffect();
            return  this;
        }

        public void AddTarget(Transform target)
        {
            if (m_aoeTargets.Contains(target))
                return;
            OnBuff(target);
            m_aoeTargets.Add(target);
        }

        public void RemoveTarget(Transform target)
        {
            if(m_aoeTargets.Contains(target) == false)
                    return;


            for(int i = 0; i < m_aoeTargets.Count; i++)
            {
                if(m_aoeTargets[i] == target)
                {
                    RemoveBuff(target);
                    m_aoeTargets.RemoveAt(i);
                    break;
                }
            }

        }
        public void RemoveAllTarget()
        {
            for (int i = 0; i < m_aoeTargets.Count; i++)
            {
                RemoveBuff(m_aoeTargets[i]);
                m_aoeTargets.RemoveAt(i);
            }



        }

        private void StopEffect()
        {
            foreach (var effect in m_collisionParticles)
            {
                effect.Stop();
            }            


        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger || m_skill.IsActive == false)
                return;
            if (other.CompareTag(m_skill.Data.SkillTarget.ToString()))
            {
                AddTarget(other.transform);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger)
                return;

            if (other.CompareTag(m_skill.Data.SkillTarget.ToString()))
            {
                RemoveTarget(other.transform);
            }
        }

    }

}
