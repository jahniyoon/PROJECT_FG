using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static UnityEngine.EventSystems.EventTrigger;


namespace JH
{
    public class PlayerAttack : MonoBehaviour
    {
        private PlayerController m_player;

        [Header("Attack State")]
        [SerializeField] private bool m_isAttack;
        [SerializeField] private float m_attackCoolDown;


        [Header("Attack Setting")]
        [SerializeField] private float m_attackOffset;
        [SerializeField] private float m_attackRadius;
        [SerializeField] private VisualEffect m_attackEffect;


        public bool isAttack => m_isAttack;

        private void Awake()
        {
            m_player = GetComponent<PlayerController>();
        }


        private void Update()
        {
            if (m_player.Input.AttackDown)
            {
                Attack();
            }

            AttackStateHandler();
        }

        private void AttackStateHandler()
        {
            if (0 < m_attackCoolDown)
            {
                m_attackCoolDown -= Time.deltaTime;
            }

            m_isAttack = 0 < m_attackCoolDown;
        }

        public void Attack()
        {
            if (m_isAttack || m_player.State == FSMState.Die || m_player.State == FSMState.Predation || m_player.State == FSMState.Freeze)
                return;

            m_player.Animation.SetTrigger(AnimationID.isAttack);


            m_attackCoolDown = m_player.Setting.PlayerAttackCoolDown;
            MeleeAttack();
        }

        private void MeleeAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position + m_player.Model.forward * m_attackOffset, m_attackRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag("Enemy"))
                {
                    Damageable damageable = colls[i].GetComponent<Damageable>();

                    if (damageable)
                    {
                        float distance = Vector3.Distance(transform.position, colls[i].transform.position);

                        Vector3 hitPoint = colls[i].ClosestPoint(transform.position);

                        damageable.OnDamage(m_player.Setting.PlayerAttackDamage);
                        colls[i].GetComponent<EnemyController>().OnStun(m_player.Setting.PlayerAttackStunDuration);
                    }
                }
            }

            if (m_attackEffect)
                m_attackEffect.Play();
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1,0,0,0.5f);
            Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_attackOffset, m_attackRadius);
        }
    }
}