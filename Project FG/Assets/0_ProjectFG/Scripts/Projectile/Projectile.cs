using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class Projectile : MonoBehaviour
    {
        SphereCollider m_collider;
        Rigidbody m_rigid;


        [SerializeField] ProjectileData m_data;
        [Header("Projectile Setting")]
        [SerializeField] protected int m_penetrate = 0;           // 관통
        protected TargetTag m_targetTag;
        protected float m_damage = 1;
        protected float m_projectileSpeed = 5;   // 스피드

        [Header("Destroy Setting")]
        protected DestroyType m_destroyType;
        protected float m_destroyValue = 10;      // 자동 삭제 거리

        protected Vector3 m_spawnPos;

        private void Awake()
        {
            AwakeInit();

        }
        protected virtual void AwakeInit()
        {
            m_collider = GetComponent<SphereCollider>();
            m_rigid = GetComponent<Rigidbody>();


            m_spawnPos = transform.position;
            if (m_data)
            {
                m_targetTag = m_data.TargetTag;
                m_damage = m_data.Damage;
                m_projectileSpeed = m_data.ProjectileSpeed;
                m_penetrate = m_data.Penetrate;
                m_destroyType = m_data.DestroyType;
                m_destroyValue = m_data.DestroyValue;
                transform.localScale = Vector3.one * m_data.ProjectileScale;
            }
            if (m_destroyType == DestroyType.Time)
            {
                Invoke(nameof(DestroyProjectile), m_destroyValue);
            }
        }


        void Update()
        {
            UpdatePosition();
            DestroyDistanceCheck();
        }
        // 위치
        protected virtual void UpdatePosition()
        {
            Vector3 velocity = transform.forward * (m_projectileSpeed * Time.deltaTime);
            m_rigid.MovePosition(m_rigid.position + velocity);
        }
        private void DestroyDistanceCheck()
        {
            if (m_destroyType != DestroyType.Distance)
                return;
            if (m_destroyValue < Vector3.Distance(m_spawnPos, transform.position))
                DestroyProjectile();
        }

        // 투사체 충돌 시
        protected virtual void Collision()
        {
            if (m_penetrate < 0)
            {
                DestroyProjectile();
            }

            m_penetrate--;
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



        private void OnTriggerEnter(Collider other)
        {
            if (IgnoreCollider(other))
                return;

            // 태그 체크 먼저 하고
            if (TagCheck(other))
            {
                Damageable target = other.transform.GetComponent<Damageable>();

                if (target)
                    target.OnDamage(m_damage);
            }

            Collision();
        }


        private bool TagCheck(Collider target)
        {
            return target.CompareTag(m_targetTag.ToString());
        }

    }


    public enum DestroyType
    {
        Distance,
        Time
    }
}
