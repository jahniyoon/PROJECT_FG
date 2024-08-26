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




        //void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = new Color(1, 0, 0, 0.5f);
        //    Gizmos.DrawSphere(transform.position, m_AttackDamageRadius);
        //}
    }
}