using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class Projectile : MonoBehaviour
    {
        Rigidbody m_rigid;
        SphereCollider m_collider;

        [SerializeField] ProjectileData m_data;
        [Header("Projectile Setting")]
        [SerializeField] private TargetTag m_targetTag;
        [SerializeField] private int m_damage = 1;
        [SerializeField] private float m_projectileSpeed = 5;   // 스피드
        [SerializeField] private float m_destroyTime = 10;      // 자동 삭제 시간
        [SerializeField] private int m_penetrate = 0;           // 관통

        private void Awake()
        {
            m_rigid = GetComponent<Rigidbody>();
            m_collider = GetComponent<SphereCollider>();
            
            if(m_data)
            {
                m_targetTag = m_data.TargetTag;
                m_damage = m_data.Damage;
                m_projectileSpeed = m_data.ProjectildSpeed;
                m_destroyTime = m_data.DestroyTime;
                m_penetrate = m_data.Penetrate;
            }
        }


        void Update()
        {
            UpdatePosition();
        }

        public void UpdatePosition()
        {
            Vector3 velocity = transform.forward * (m_projectileSpeed * Time.deltaTime);
            m_rigid.MovePosition(m_rigid.position + velocity);

            m_destroyTime -= Time.deltaTime;

            if (m_destroyTime <= 0)
                Collision();
        }

        // 충돌 시
        protected virtual void Collision()
        {

            if (m_penetrate <= 0)
            {
                Destroy(gameObject);
            }

            m_penetrate--;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger || other.CompareTag("Player"))
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

    public enum TargetTag
    {
        Enemy,
        Player
    }
}
