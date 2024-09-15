using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public class Projectile : MonoBehaviour
    {
        SphereCollider m_collider;
        Rigidbody m_rigid;


        [SerializeField] ProjectileData m_data;
        [Header("Projectile Info")]
        [SerializeField] protected int m_penetrate = 0;           // 관통
        [SerializeField] protected TargetTag m_targetTag;
        [SerializeField] protected float m_damage = 1;
        [SerializeField] protected float m_projectileSpeed = 5;   // 스피드
        [SerializeField] protected float m_projectileScale = 1;   // 스피드

        [Header("Projectile Target")]
        [SerializeField] protected Transform m_target;
        [SerializeField] protected Vector3 m_targetPosition;


        [Header("Destroy Setting")]
        [SerializeField] protected DestroyType m_destroyType;
        [SerializeField] protected float m_destroyTime = 10;      // 자동 삭제 시간
        Coroutine m_destroyRoutine;

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
            GetData();


        }
        private void GetData()
        {
            if (m_data)
            {
                m_targetTag = m_data.TargetTag;
                m_damage = m_data.Damage;
                m_projectileSpeed = m_data.ProjectileSpeed;
                m_penetrate = m_data.Penetrate;
                m_destroyType = m_data.DestroyType;
                m_destroyTime = m_data.DestroyValue;
                transform.localScale = Vector3.one * m_data.ProjectileScale;

                m_destroyRoutine = StartCoroutine(DestroyRoutine());
            }
        }

        public virtual void ProjectileInit(float Damage, float ProjectileSpeed, int Penetrate, float DestroyValue, TargetTag TargetTag, float Scale = 1)
        {
            m_damage = Damage;
            m_projectileSpeed = ProjectileSpeed;
            m_penetrate = Penetrate;
            m_destroyTime = DestroyValue;
            m_targetTag = TargetTag;
            m_projectileScale = Scale;

            if(m_destroyRoutine != null)
            {
                StopCoroutine(m_destroyRoutine);
                m_destroyRoutine = null;
            }
            m_destroyRoutine = StartCoroutine(DestroyRoutine());
        }
        public virtual void SetTarget(Transform target)
        {
            m_target = target;
        }
        public virtual void SetTarget(Vector3 target)
        {
            m_targetPosition = target;
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
            if (m_destroyTime < Vector3.Distance(m_spawnPos, transform.position))
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

        protected virtual void TriggerEnter(Collider other)
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


        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter(other);
        }


        private bool TagCheck(Collider target)
        {
            return target.CompareTag(m_targetTag.ToString());
        }


        IEnumerator DestroyRoutine()
        {
            if (m_destroyType != DestroyType.Time)
                yield break;

            float timer = 0;
            while (timer < m_destroyTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            DestroyProjectile();
            yield break;
        }


    }


    public enum DestroyType
    {
        Distance,
        Time
    }
}
