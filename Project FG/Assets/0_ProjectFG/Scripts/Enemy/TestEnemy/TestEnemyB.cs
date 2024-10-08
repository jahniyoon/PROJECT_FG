using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyB : EnemyController
    {
        EnemyBData m_subbData;
        [Header("Enemy B")]
        [Header("Attack")]
        [SerializeField] private VisualEffect m_attackEffect;

        [Header("Skill")]
        [SerializeField] private SkillBase m_shildSkill;

        //[Header("OnBuff")]
        //[SerializeField] private float m_buffCoolDown;
        //[SerializeField] private GameObject m_buffShild;
        //Coroutine m_buffRoutine;


        protected override void StartInit()
        {
            base.StartInit();
            TryGetData();

            // 스킬을 초기화한다.
            m_shildSkill = Instantiate(m_subbData.ShieldSkill, transform).GetComponent<SkillBase>();
            m_shildSkill.SkillInit(this.gameObject, m_model);
        }
        private bool TryGetData()
        {
            var m_childData = m_data as EnemyBData;
            if (m_childData != null)
            {
                m_subbData = m_childData;
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
        }
        protected override void Die()
        {
            base.Die();
            m_shildSkill.gameObject.SetActive(false);
        }

        private void MeleeAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position + m_model.forward * m_subbData.AttackOffset, m_subbData.AttackRadius);
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
                        damageable.OnDamage(m_subbData.AttackDamage);
                    }
                    if(colls[i].TryGetComponent<IKnockbackable>(out IKnockbackable knockbackable))
                    {
                        knockbackable.OnKnockback(transform.position, m_subbData.KnockBackDistance, m_subbData.KnockBackDuration);
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
                //Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_subbData.AttackOffset, m_subbData.AttackRadius);
            }
        }
    }
}