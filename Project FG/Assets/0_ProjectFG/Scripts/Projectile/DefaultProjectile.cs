using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public class DefaultProjectile : ProjectileBase
    {
        SphereCollider m_collider;
        Rigidbody m_rigid;



        protected override void AwakeInit()
        {
            base.AwakeInit();
            m_collider = GetComponent<SphereCollider>();
            m_rigid = GetComponent<Rigidbody>();
            m_collider.radius = m_data.ProjectileScale * 0.5f;

        }
        public override void SetSkill(SkillBase skill)
        {
            base.SetSkill(skill);
        }


        // 위치
        protected void FixedUpdate()
        {
            Vector3 velocity = transform.forward * (m_projectileSpeed * Time.deltaTime);
            m_rigid.MovePosition(m_rigid.position + velocity);
        }


        // 투사체 충돌 시
        protected virtual void Collision()
        {
            if (m_penetrateCount <= 0)
                InActiveProjectile();

            m_penetrateCount--;
        }


        // 무시할 콜라이더
        protected virtual bool IgnoreCollider(Collider other)
        {

            // 자기 자신은 무시
            if (m_skill.Caster.GameObject == other.gameObject)
                return true;

            if (other.CompareTag("Projectile"))
                return true;

            return other.isTrigger;
        }

        protected virtual void TriggerEnter(Collider other)
        {
            if (IgnoreCollider(other))
                return;

            // 태그 체크 먼저 하고
            if (TagCheck(other))
            {
                if(other.TryGetComponent<Damageable>(out Damageable damageable))
                    damageable.OnDamage(m_skill.LevelData.Damage);

                if (other.TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
                    OnBuff(other.transform);
            }

            Collision();
        }


        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter(other);
        }


        private bool TagCheck(Collider target)
        {
            return target.CompareTag(m_skill.Data.SkillTarget.ToString());
        }
    }


}
