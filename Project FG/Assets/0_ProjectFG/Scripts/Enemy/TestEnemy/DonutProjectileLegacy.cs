using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class DonutProjectilLegacy : MonoBehaviour
    {
        [Header("Projectile Setting")]
        
        [SerializeField] float m_projectileDamage = 1;                // 데미지

        [Tooltip("투사체 크기")]
        [SerializeField] float m_projectileScale = 5;                // 크기
        [Tooltip("투사체 전체 크기")]
        [SerializeField] float m_AttackDamageRadius = 5;    // 전체 크기
        [Tooltip("투사체 공격을 안받는 사이즈")]
        [SerializeField] float m_AttackInnerRadius = 2.5f;  // 공격 받는 사이즈
        [Tooltip("제거 시간")]
        [SerializeField] float m_DestroyTime = 1;  // 공격 받는 사이즈


        [Header("Donut Move Setting")]
        [Tooltip("투사체가 떨어지는 시간")]
        [SerializeField] float m_projectileDropDuration = 1;    // 떨어지는 시간         
        [Tooltip("투사체 목표의 높이\n보통은 플레이어의 높이입니다.")]
        [SerializeField] float m_targetHeight = 0;    // 목표 높이       
        [SerializeField] ParticleSystem m_dropParticle;

        public void ProjectileInit(float damage, float speed = 5)
        {
            m_projectileDamage = damage;
            //m_projectileSpeed = speed;
            transform.localScale = Vector3.one * m_projectileScale;
        }

        public void Shoot(Vector3 targetPos)
        {
            m_targetHeight = targetPos.y;

            StartCoroutine(ShootRoutine());
        }


        IEnumerator ShootRoutine()
        {
            float timer = 0;

            Vector3 startPos = transform.position;
            Vector3 endPos = startPos;
            endPos.y = m_targetHeight;

            while(timer < m_projectileDropDuration)
            {
                transform.position = Vector3.Lerp(startPos, endPos, timer / m_projectileDropDuration);
                timer += Time.deltaTime ;
                yield return null;
            }

            DonutDrop();

            yield break;
        }

   

        // 도넛이 떨어졌을 때
        private void DonutDrop()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_AttackDamageRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

             


                if (colls[i].CompareTag("Player"))
                {
                    // 원의 안쪽은 데미지를 안입는다.
                    Vector3 targetPos = colls[i].transform.position;
                    targetPos.y = transform.position.y;

                    float distance = Vector3.Distance(transform.position, targetPos);

                    if (distance < m_AttackInnerRadius)
                    continue;


                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_projectileDamage);
                    }
                }
            }

            m_dropParticle?.Stop();     
            m_dropParticle?.Play();     
            Destroy(gameObject, m_DestroyTime);
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, m_AttackDamageRadius);
        }
    }
}