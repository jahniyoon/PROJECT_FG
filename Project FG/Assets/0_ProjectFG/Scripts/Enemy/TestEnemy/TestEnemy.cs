using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemy : EnemyController
    {

        [Header("Attack")]
        [SerializeField] private VisualEffect m_attackEffect;
        [SerializeField] private float m_attackOffset = 0;
        [SerializeField] private float m_attackRadius = 0;


        [Header("Attack CoolDown")]
        [SerializeField] private float m_attackCoolDown = 0;

        Coroutine m_attackCoolDownRoutine;



        private bool CanAttackCheck()
        {
            return m_attackCoolDown <= 0;
        }

        private void MeleeAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position + m_model.forward * m_attackOffset, m_attackRadius);
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
                        float distance = Vector3.Distance(transform.position, colls[i].transform.position);

                        Vector3 hitPoint = colls[i].ClosestPoint(transform.position);

                        damageable.OnDamage(m_data.AttackDamage);
                    }
                }
            }

            if (m_attackEffect)
            m_attackEffect.Play();
        }

        IEnumerator AttackCoolDownRoutine()
        {
            while(0 < m_attackCoolDown)
            {
                m_attackCoolDown -= Time.deltaTime;
                yield return null;
            }

            m_attackCoolDown = 0;
            yield break;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_attackOffset, m_attackRadius);
        }
    }
}