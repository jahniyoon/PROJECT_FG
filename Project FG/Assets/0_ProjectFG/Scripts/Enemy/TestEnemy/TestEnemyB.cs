using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyB : EnemyController
    {
        EnemyBData m_bData;
        [Header("Enemy B")]
        [Header("Attack")]
        [SerializeField] private VisualEffect m_attackEffect;

        [Header("Buff")]
        [SerializeField] private bool m_isBuff;
        [SerializeField] private float m_buffCoolDown;
        [SerializeField] private GameObject m_buffShild;
        Coroutine m_buffRoutine;


        protected override void StartInit()
        {
            base.StartInit();
            TryGetData();
        }
        private bool TryGetData()
        {
            var m_childData = m_data as EnemyBData;
            if (m_childData != null)
            {
                m_bData = m_childData;
                return true;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
                return false;
            }
        }

        protected override void UpdateBehaviour()
        {
            base.UpdateBehaviour();
            if(0 <m_buffCoolDown)
            {
                m_buffCoolDown -= Time.deltaTime;               
            }
        }



        private void ActiveBuff()
        {
            m_isBuff = true;
            m_buffCoolDown += m_bData.BuffCoolDown;
            m_buffShild.SetActive(true);
            Invoke(nameof(DeActiveBuff), m_bData.BuffDuration);
        }
        private void DeActiveBuff()
        {
            m_isBuff = false;
            m_buffShild.SetActive(false);
        }


        private void MeleeAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position + m_model.forward * m_bData.AttackOffset, m_bData.AttackRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag("Player"))
                {

                    if (colls[i].TryGetComponent<IDamageable>(out IDamageable damageable))
                    {
                        damageable.OnDamage(m_data.AttackDamage);
                    }
                    if(colls[i].TryGetComponent<IKnockbackable>(out IKnockbackable knockbackable))
                    {
                        knockbackable.OnKnockback(transform.position, m_bData.KnockBackDistance, m_bData.KnockBackDuration);
                    }
                }
            }
            if (m_attackEffect)
                m_attackEffect.Play();

        }


        void OnDrawGizmosSelected()
        {
            bool canGizmo = TryGetData();

            if (canGizmo)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                //Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_bData.AttackOffset, m_bData.AttackRadius);
            }
        }
    }
}