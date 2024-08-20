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

            if (0 < m_stunCoolDown)
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

            if (0 < m_stunCoolDown)
                return new HitState();

            if (m_target == null)
                return new IdleState();

            if (TargetAttackDistanceCheck())
            {
                return new AttackState();
            }

            return null;
        }

        float m_posCheckTimer = 0;

        protected override void MoveStateEnter()
        {
            m_posCheckTimer = 0;
            m_agent.isStopped = false;
        }

        protected override void MoveStateStay()
        {
            PositionCheck();
        }


        protected override void MoveStateExit()
        {
            m_agent.isStopped = true;
        }

        private void PositionCheck()
        {
            //  타이머
            //m_posCheckTimer += Time.deltaTime;

            //if (m_posCheckTimer < 0.5f)
            //    return;


            //m_posCheckTimer = 0;


            float targetDistance = Vector3.Distance(transform.position, m_target.transform.position);

            // 공격 거리보다 가까우면
            if (targetDistance < m_dData.EscapeRadius)
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

        #region ATTACK STATE
        protected override FSM<EnemyController> AttackStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (0 < m_stunCoolDown)
                return new HitState();

            if (m_target == null)
                return new IdleState();

            if (TargetAttackDistanceCheck() == false)
                return new MoveState();

            return null;
        }

        protected override void AttackStateEnter()
        {
            // 돌입시 타이머를 리셋한다.
            ResetAttackTimer();
        }

        protected override void AttackStateStay()
        {
            if (m_data.AttackSpeed < m_attackTimer && CanAttackCheck())
            {
                // 공격 속도 타이머와 쿨타임 초기화
                m_attackTimer = 0;
                m_attackCoolDown = m_data.AttackCoolDown;

                if (m_attackCoolDownRoutine != null)
                {
                    StopCoroutine(m_attackCoolDownRoutine);
                    m_attackCoolDownRoutine = null;
                }

                m_attackCoolDownRoutine = StartCoroutine(AttackCoolDownRoutine());

                Shootdonut(m_target.position);
                ModelRotate(m_target.position, true);
            }
            ModelRotate(m_target.position);

            m_attackTimer += Time.deltaTime;
        }
        #endregion

        #region Hit STATE
        protected override FSM<EnemyController> HitStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (m_stunCoolDown <= 0)
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