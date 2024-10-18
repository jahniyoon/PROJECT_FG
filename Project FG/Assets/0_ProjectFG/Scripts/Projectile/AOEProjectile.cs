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
        [SerializeField] private List<Damageable> m_aoeTargets = new List<Damageable>();





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
        }

        public override void SetRadius(float radius)
        {
            base.SetRadius(radius);
            m_collider.radius = radius;
        }

        public override void ActiveProjectile()
        {
            PlayEffect();
        }
        public override ProjectileBase InActiveProjectile()
        {
            StopEffect();
            RemoveAllTarget();
            return base.InActiveProjectile();
        }

        public void AddTarget(Transform target)
        {
            Damageable damageable;
            target.TryGetComponent(out damageable);

            if (damageable == null)
                return;
            if (damageable.IsDie)
                return;

            if (m_aoeTargets.Contains(damageable))
                return;


            OnAreaBuff(target);
            m_aoeTargets.Add(damageable);
        }

        public void RemoveTarget(Transform target)
        {
            Damageable damageable;
            target.TryGetComponent(out damageable);

            if (damageable == null)
                return;


            if (m_aoeTargets.Contains(damageable) == false)
                return;


            for (int i = 0; i < m_aoeTargets.Count; i++)
            {
                if (m_aoeTargets[i] == damageable)
                {
                    RemoveAreaBuff(target);
                    m_aoeTargets.RemoveAt(i);
                    break;
                }
            }

        }
        public void RemoveAllTarget()
        {

            for (int i = 0; i < m_aoeTargets.Count; i++)
            {
                RemoveAreaBuff(m_aoeTargets[i].transform);
            }
            m_aoeTargets.Clear();
        }

        private void OnTriggerStay(Collider other)
        {
            if (m_skill == null)
                return;

            if (other.isTrigger || m_skill.IsActive == false)
                return;
            if (other.CompareTag(m_skill.Data.SkillTarget.ToString()))
                AddTarget(other.transform);
        }
        private void OnTriggerExit(Collider other)
        {
            if (m_skill == null)
                return;

            if (other.isTrigger)
                return;

            if (other.CompareTag(m_skill.Data.SkillTarget.ToString()))
            {
                RemoveTarget(other.transform);
            }
        }

    }

}
