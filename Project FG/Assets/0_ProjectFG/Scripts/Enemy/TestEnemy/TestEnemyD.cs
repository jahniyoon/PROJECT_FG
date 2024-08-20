using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;
using Unity.VisualScripting;


namespace JH
{
    public partial class TestEnemyD : EnemyController
    {
        private EnemyDData m_dData;

        [Header("Enemy D")]
        [SerializeField] private DonutProjectile m_projectile;
        [Tooltip("투사체가 떨어지는 높이")]
        [SerializeField] private float m_projectileYOffset = 5;    // 투사체가발사되는 곳

        //[Header("공격 / 도망 반경")]
        //[Tooltip("공격 가능 거리")]
        //[SerializeField] private float m_AttackRadius = 2; // 도망가는 거리
        //[Tooltip("도망 거리")]
        //[SerializeField] private float m_escapeRadius = 2; // 도망가는 거리

        [Header("Attack CoolDown")]
        private float m_attackCoolDown = 0;
        Coroutine m_attackCoolDownRoutine;

        protected override void StartInit()
        {
            base.StartInit();

            EnemyDData m_childData = m_data as EnemyDData;
            if(m_childData != null)
            {
                m_dData = m_childData;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
            }
        }


        ///  <summary> 플레이어와의 공격 가능한 거리를 계속 체크한다.</summary>
        /// <returns>TRUE : 공격 가능 / False : 공격 불가</returns>
        private bool TargetAttackDistanceCheck()
        {
            if (m_target == null)
                return false;

            float targetDistance = Vector3.Distance(transform.position, m_target.transform.position);

            // 타겟과의 거리가 도망가는 거리보다 크고, 공격 가능거리보다 짧다.
            if (m_dData.EscapeRadius < targetDistance && targetDistance <= m_dData.AttackRadius)
                return true;

            return false;
        }


        /// <summary>
        /// 플레이어를 피해 도망갈 포지션을 찾는다.
        /// </summary>
        /// <returns></returns>
        private Vector3 FindChasePos()
        {

            // 현재 적의 위치
            Vector3 currentPosition = transform.position;

            // 적에서 플레이어까지의 방향 벡터 (플레이어 방향)
            Vector3 directionToPlayer = m_target.position - currentPosition;

            // 플레이어로부터 반대 방향 (벡터의 반대 방향)
            Vector3 oppositeDirection = -directionToPlayer.normalized;

            // 도망가는 위치 = 현재 위치에서 반대 방향으로 일정 거리만큼 이동한 위치
            Vector3 escapePosition = currentPosition + oppositeDirection * m_dData.EscapeRadius;

            escapePosition = GFunc.FindNavPos(transform, escapePosition, m_dData.EscapeRadius * 2);
            return escapePosition;


            //// 이동가능한 곳인지 체크한다.

        }

       


        private bool CanAttackCheck()
        {
            return m_attackCoolDown <= 0;
        }

        private void Shootdonut(Vector3 targetPos)
        {
            Vector3 projectilePos = targetPos;
            projectilePos.y += m_projectileYOffset;

            var projectile = Instantiate(m_projectile, projectilePos, Quaternion.identity, GameManager.Instance.ProjectileParent);
            // 투사체 초기화하고
            projectile.ProjectileInit(m_data.AttackDamage);
            // 발사
            projectile.Shoot(targetPos);
        }




        IEnumerator AttackCoolDownRoutine()
        {
            while (0 < m_attackCoolDown)
            {
                m_attackCoolDown -= Time.deltaTime;
                yield return null;
            }

            m_attackCoolDown = 0;
            yield break;
        }

        //void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = new Color(1, 0, 0, 0.5f);
        //    Gizmos.DrawSphere(transform.position, m_AttackDamageRadius);
        //}
    }
}