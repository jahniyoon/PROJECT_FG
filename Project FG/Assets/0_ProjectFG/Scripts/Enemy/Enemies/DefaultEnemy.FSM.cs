using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public partial class DefaultEnemy
    {
        #region FREEZE STATE
        protected override FSM<EnemyController> FreezeStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (CCStateCheck())
                return new HitState();

            if (CheckFreezeState() == false)
                return new IdleState();

            return null;
        }

        protected override void FreezeStateStay()
        {
            base.FreezeStateStay();
            ModelRotate(m_target.position, false, true);
        }
        #endregion

        #region IDLE STATE
        protected override FSM<EnemyController> IdleStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (CCStateCheck())
                return new HitState();

            if (CheckFreezeState())
                return new FreezeState();


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

            if (CCStateCheck())
                return new HitState();

            if (CheckFreezeState())
                return new FreezeState();

            if (TargetCheck() == false)
                return new IdleState();


            if (m_target == null)
                return new IdleState();

            if (AttackRangeCheck())
                return new AttackState();

            return null;
        }
        protected override void MoveStateEnter()
        {
            // 정지를 시킨경우 켜지 않는다.
            m_agent.isStopped = false;

            m_distanceCheckDelay = 0;
        }

        protected override void MoveStateStay()
        {
            m_agent.avoidancePriority = 50;

            m_distanceCheckDelay = 0 < m_distanceCheckDelay - Time.deltaTime ? m_distanceCheckDelay - Time.deltaTime : 0;

            // 스킬 타이머동안은 움직이지 않는다.
            if (0 < m_skillTimer)
            {
                StopBehavior();
                return;
            }

            // 거리체크 딜레이동안은 이동 체크를 안한다.
            if (0 < m_distanceCheckDelay)
                return;


            if (m_data.SurroundRange < m_targetDistance)
            {
                SurroundBehavior();
                return;
            }
            // 회피거리보다 가까우면
            if (m_targetDistance < m_data.EscapeRange)
            {
                EscapeBehavior();
                return;
            }

            // 공격범위보다 가까우면 멈춘다.
            if (m_targetDistance <= m_data.ChaseRange)
            {
                StopBehavior();
                return;
            }

            ChaseBehavior();

        }


        protected void StopBehavior()
        {
            SetMoveState(EnemyMoveState.Stop);

            m_agent.avoidancePriority = 49;

            SetMoveDestination(transform.position);
        }

        // 포위 행동
        protected void SurroundBehavior()
        {
            SetMoveState(EnemyMoveState.Surround);
            m_distanceCheckDelay += m_data.SurroundDelay;

            Vector3 surroundPos = FindSurroundPos();

            SetMoveDestination(surroundPos);
        }

        // 추적 행동
        protected void ChaseBehavior()
        {
            SetMoveState(EnemyMoveState.Chase);
            m_distanceCheckDelay += m_data.ChaseDelay;

            SetMoveDestination(m_target.position);


        }

        // 회피 행동
        protected void EscapeBehavior()
        {
            SetMoveState(EnemyMoveState.Escape);
            m_distanceCheckDelay += m_data.EscapeDelay;

            SetMoveDestination(FindEscapePos());
        }

        protected override void MoveStateExit()
        {
            SetMoveState(EnemyMoveState.Stop);
            StopMove();
        }

        private void StopMove()
        {
            m_agent.avoidancePriority = 49;
            m_agent.SetDestination(this.transform.position);
            m_agent.isStopped = true;
        }
        #endregion

        #region ATTACK STATE
        protected override FSM<EnemyController> AttackStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (CCStateCheck())
                return new HitState();

            if (CheckFreezeState())
                return new FreezeState();

            if (TargetCheck() == false)
                return new IdleState();

            if (AttackRangeCheck() == false)
                return new MoveState();

            return null;
        }



        #endregion

        #region Hit STATE
        protected override FSM<EnemyController> HitStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (CCStateCheck() == false)
                return new IdleState();

            return null;
        }
        protected override void HitStateEnter()
        {
            m_stunEffect.gameObject.SetActive(true);
            m_stunEffect.Stop();
            m_stunEffect.Play();

            foreach (var skill in m_attackSkills)
            {
                skill.FreezeSkill(true);
            }
        }

        //우선순위
        // 스턴 > 넉백 > 
        protected override void HitStateStay()
        {
            base.HitStateStay();

            if (m_buffHandler.Status.IsStun)
                return;

            if (m_buffHandler.Status.IsFrozen)
                return;

            if (m_isKnockback)
                return;

        
            FearState();
        }

        private void FearState()
        {
            if (m_isFear == false)
                return;

            m_agent.isStopped = false;
            SetMoveDestination(FindEscapePos());
        }

        protected override void HitStateExit()
        {
            m_stunEffect.gameObject.SetActive(false);
            m_stunEffect.Stop();

            foreach (var skill in m_attackSkills)
            {
                skill.FreezeSkill(false);
            }

            StopMove();
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
            scale -= Time.deltaTime * m_dieSpeed;
            scale = 0 <= scale ? scale : 0;
        }

        #endregion
    }


}