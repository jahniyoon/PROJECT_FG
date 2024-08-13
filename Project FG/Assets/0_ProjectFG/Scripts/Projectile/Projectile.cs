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
        [SerializeField] private float m_damage = 1;
        [SerializeField] private float m_projectileSpeed = 5;   // 스피드
        [SerializeField] private float m_destroyDistnace = 10;      // 자동 삭제 거리
        [SerializeField] private int m_penetrate = 0;           // 관통
        private Vector3 m_spawnPos;

        private void Awake()
        {
            m_rigid = GetComponent<Rigidbody>();
            m_collider = GetComponent<SphereCollider>();
            m_spawnPos = transform.position;
            if (m_data)
            {
                m_targetTag = m_data.TargetTag;
                m_damage = m_data.Damage;
                m_projectileSpeed = m_data.ProjectileSpeed;
                m_destroyDistnace = m_data.DestroyDistance;
                m_penetrate = m_data.Penetrate;
                transform.localScale = Vector3.one * m_data.ProjectileScale;
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

            float distance = Vector3.Distance(transform.position, m_spawnPos);

            if (m_destroyDistnace <= distance)
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
