using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyG : EnemyController
    {
        EnemyGData m_subData;
        [Header("Enemy G")]
        [SerializeField] private float m_conversionTime;
        private SkillBase m_healSkill;

        private float m_conversionTimer = -1;

        protected override void StartInit()
        {
            base.StartInit();
            TryGetData();
        }
        private bool TryGetData()
        {
            EnemyGData m_childData = m_data as EnemyGData;
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

        private bool CanConversion()
        {
            return  m_conversionTime < m_conversionTimer;
        }

      


        void OnDrawGizmosSelected()
        {
            bool canGizmo = TryGetData();

            if (canGizmo)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(transform.position, 1);
            }
        }
    }
}