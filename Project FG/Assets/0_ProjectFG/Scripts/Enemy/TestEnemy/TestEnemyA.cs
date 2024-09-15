using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyA : EnemyController
    {
        EnemyAData m_subData;
        [Header("Enemy A")]
        [SerializeField] private VisualEffect m_attackEffect;

        protected override void StartInit()
        {
            base.StartInit();
            TryGetData();
        }
        private bool TryGetData()
        {
            EnemyAData m_childData = m_data as EnemyAData;
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

        private void MeleeAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position + m_model.forward * m_subData.AttackOffset, m_subData.AttackRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag("Player"))
                {
                    Damageable damageable = colls[i].GetComponent<Damageable>();

                    if (damageable)
                    {
                        damageable.OnDamage(m_subData.AttackDamage);
                    }
                }
            }
            Debug.Log("때린다");
            if (m_attackEffect)
                m_attackEffect.Play();
        }


        void OnDrawGizmosSelected()
        {
            bool canGizmo = TryGetData();

            if (canGizmo)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_subData.AttackOffset, m_subData.AttackRadius);
            }
        }
    }
}