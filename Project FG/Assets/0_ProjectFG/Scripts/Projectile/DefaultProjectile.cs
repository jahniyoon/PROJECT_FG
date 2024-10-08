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



        [SerializeField] private int m_penetrateCount;


        protected Vector3 m_spawnPos;

        protected override void AwakeInit()
        {
            m_collider = GetComponent<SphereCollider>();
            m_rigid = GetComponent<Rigidbody>();
            m_spawnPos = transform.position;
            m_penetrateCount = m_data.Penetrate;
            m_collider.radius = m_data.ProjectileScale * 0.5f;

        }
        public override void SetSkill(SkillBase skill)
        {
            base.SetSkill(skill);
            Invoke(nameof(DestroyProjectile), m_skill.Data.SkillLifeTime);
        }


        void Update()
        {
            UpdatePosition();
        }
        // 위치
        protected virtual void UpdatePosition()
        {
            Vector3 velocity = transform.forward * (m_data.ProjectileSpeed * Time.deltaTime);
            m_rigid.MovePosition(m_rigid.position + velocity);
        }


        // 투사체 충돌 시
        protected virtual void Collision()
        {
            if (m_penetrateCount <= 0)
                DestroyProjectile();

            m_penetrateCount--;
        }

        // 투사체 파괴
        protected virtual void DestroyProjectile()
        {
            Destroy(gameObject);
        }

        // 무시할 콜라이더
        protected virtual bool IgnoreCollider(Collider other)
        {
            return other.isTrigger;
        }

        protected virtual void TriggerEnter(Collider other)
        {
            if (IgnoreCollider(other))
                return;

            // 태그 체크 먼저 하고
            if (TagCheck(other))
            {
                Damageable target = other.transform.GetComponent<Damageable>();

                if (target)
                    target.OnDamage(m_skill.Data.SkillDamage);
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
