using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public partial class TestEnemy 
    {
        #region IDLE STATE
        protected override FSM<EnemyController> IdleStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (0 < m_groggyCoolDown)
                return new GroggyState();


            if (m_target)
                return new MoveState();

            return null;
        }

        protected override void IdleStateEnter()
        {
            m_agent.enabled = false;
        }
        protected override void IdleStateExit()
        {
            m_agent.enabled = true;
        }

        #endregion

        #region MOVE STATE
        protected override FSM<EnemyController> MoveStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (0 < m_groggyCoolDown)
                return new GroggyState();

            if (m_target == null)
                return new IdleState();

            if (m_targetDistance <= m_data.AttackRange)
            {
                return new AttackState();
            }

            return null;
        }
        protected override void MoveStateEnter()
        {
            m_agent.isStopped = false;
        }

        protected override void MoveStateStay()
        {
            m_agent.SetDestination(m_target.position);
            ModelRotate(m_target.position);
        }
        protected override void MoveStateExit()
        {
            m_agent.isStopped = true;
        }
        #endregion

        #region ATTACK STATE
        protected override FSM<EnemyController> AttackStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (0 < m_groggyCoolDown)
                return new GroggyState();

            if (m_target == null)
                return new IdleState();

            if (m_data.AttackRange < m_targetDistance)
                return new MoveState();

                return null;
        }

        float timer = 0;

        protected override void AttackStateStay()
        {
            if(m_data.AttackSpeed < timer && CanAttackCheck())
            {
                // 공격 속도 타이머와 쿨타임 초기화
                timer = 0;
                m_attackCoolDown = m_data.AttackCoolDown;

                if(m_attackCoolDownRoutine != null)
                {
                    StopCoroutine(m_attackCoolDownRoutine);
                    m_attackCoolDownRoutine = null;
                }

                m_attackCoolDownRoutine = StartCoroutine(AttackCoolDownRoutine());

                MeleeAttack();
                ModelRotate(m_target.position, true);
            }
            ModelRotate(m_target.position);

            timer += Time.deltaTime;
        }
        #endregion

        #region GROGGY STATE
        protected override FSM<EnemyController> GroggyStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (m_groggyCoolDown <= 0)
                return new IdleState();

            return null;
        }
        protected override void GroggyStateEnter()
        {
            m_groggyEffect.gameObject.SetActive(true);
            m_groggyEffect.Stop();
            m_groggyEffect.Play();
        }

        protected override void GroggyStateStay()
        {
            m_groggyCoolDown -= Time.deltaTime;
        }

        protected override void GroggyStateExit()
        {
            m_groggyEffect.gameObject.SetActive(false);
            m_groggyEffect.Stop();
        }
        #endregion



        #region DIE STATE
        protected override FSM<EnemyController> DieStateConditional()
        {
            if (m_damageable.IsDie == false)
                return new IdleState();

            return null;
        }
        float scale = 1;
        Vector3 rotation = Vector3.zero;

        protected override void DieStateStay()
        {
            transform.localScale = Vector3.one * scale;


            transform.eulerAngles = rotation;


            rotation.y += Time.deltaTime * 5;
            scale -= Time.deltaTime;
        }

        #endregion
    }


}