using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyA : EnemyController
    {
        EnemyAData m_aData;
        [Header("Enemy A")]
        [SerializeField] private VisualEffect m_attackEffect;
        //[SerializeField] private float m_attackOffset = 0;
        //[SerializeField] private float m_attackRadius = 0;


        [Header("Attack CoolDown")]
        private float m_attackCoolDown = 0;
        Coroutine m_attackCoolDownRoutine;

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
                m_aData = m_childData;
                return true;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
                return false;
            }
        }


        private bool CanAttackCheck()
        {
            return m_attackCoolDown <= 0;
        }

        private void MeleeAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position + m_model.forward * m_aData.AttackOffset, m_aData.AttackRadius);
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
                        damageable.OnDamage(m_data.AttackDamage);
                    }
                }
            }

            if (m_attackEffect)
                m_attackEffect.Play();
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

        void OnDrawGizmosSelected()
        {
            bool canGizmo = TryGetData();

            if (canGizmo)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_aData.AttackOffset, m_aData.AttackRadius);
            }
        }
    }
}