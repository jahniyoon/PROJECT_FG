using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public partial class TestEnemyD
    {
        #region IDLE STATE
        protected override FSM<EnemyController> IdleStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (HitStateCheck())
                return new HitState();


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

            if (HitStateCheck())
                return new HitState();

            if (m_target == null)
                return new IdleState();
        
            return null;
        }

        float m_posCheckTimer = 0;

        protected override void MoveStateEnter()
        {
            m_posCheckTimer = 0;
            m_agent.isStopped = false;
            ResetAttackTimer();
        }

        protected override void MoveStateStay()
        {
            MoveBehavior();
            AttackBehavior();
        }
        private void MoveBehavior()
        {
            PositionCheck();

        }
        private void AttackBehavior()
        {

            if (m_subData.AttackSpeed < m_attackTimer && CanAttackCheck() && AttackRangeCheck())
            {
                // 공격 속도 타이머와 쿨타임 초기화
                m_attackTimer = 0;
                m_attackCoolDown = m_subData.AttackCoolDown;

                ShootDonut(m_target.position);
                ModelRotate(m_target.position);
            }

            m_attackTimer += Time.deltaTime;
            
        }


        protected override void MoveStateExit()
        {
            m_agent.SetDestination(this.transform.position);
            m_agent.isStopped = true;
        }

        private void PositionCheck()
        {
            // 공격중엔 제자리
            if(m_attackTimer < m_subData.AttackSpeed)
            {
                m_agent.SetDestination(this.transform.position);
                ModelRotate(m_target.position);

                return;
            }


            // 공격 거리보다 가까우면
            if (m_targetDistance < m_data.EscapeRange)
            {
                Vector3 destination = FindChasePos();

                m_agent.SetDestination(destination);
                ModelRotate(destination);
                return;
            }

            m_agent.SetDestination(m_target.position);
            ModelRotate(m_target.position);

        }

        #endregion


        #region Hit STATE
        protected override FSM<EnemyController> HitStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (HitStateCheck() == false)
                return new IdleState();

            return null;
        }
        protected override void HitStateEnter()
        {
            m_stunEffect.gameObject.SetActive(true);
            m_stunEffect.Stop();
            m_stunEffect.Play();
        }

        protected override void HitStateStay()
        {
            m_stunCoolDown -= Time.deltaTime;
        }

        protected override void HitStateExit()
        {
            m_stunEffect.gameObject.SetActive(false);
            m_stunEffect.Stop();
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