using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyC : EnemyController
    {
        EnemyCData m_subData;
        [Header("Enemy C")]
        [SerializeField] private Transform m_shootPos;

        protected override void StartInit()
        {
            base.StartInit();
            TryGetData();
        }
        private bool TryGetData()
        {
            EnemyCData m_childData = m_data as EnemyCData;
            if (m_childData != null)
            {
                m_subData = m_childData;
                return true;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
                return false;
            }
        }

        private void ShootProjectile()
        {
            var projectile = Instantiate(m_subData.Projectile, m_shootPos.position, m_model.rotation, GameManager.Instance.ProjectileParent);
        }



    }
}