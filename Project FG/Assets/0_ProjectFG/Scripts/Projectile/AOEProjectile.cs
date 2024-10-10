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
        [SerializeField] private Transform m_effectTransform;





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
            m_collider.radius = m_skill.LevelData.Radius;
            m_effectTransform.transform.localScale = Vector3.one * m_skill.LevelData.Radius;
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
            if (m_aoeTargets.Contains(target))
                return;


            OnBuff(target);
            m_aoeTargets.Add(target);
        }

        public void RemoveTarget(Transform target)
        {
            if (m_aoeTargets.Contains(target) == false)
                return;


            for (int i = 0; i < m_aoeTargets.Count; i++)
            {
                if (m_aoeTargets[i] == target)
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
