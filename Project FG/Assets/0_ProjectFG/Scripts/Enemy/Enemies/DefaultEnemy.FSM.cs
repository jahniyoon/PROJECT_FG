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

            if (HitStateCheck())
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

            if (HitStateCheck())
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

            if (HitStateCheck())
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
            m_agent.isStopped = m_isStop;
            m_agent.avoidancePriority = 50;
        }

        protected override void MoveStateStay()
        {
            m_agent.avoidancePriority = 50;

            Vector3 destination = m_target.position;

            // 스킬 타이머동안은 움직이지 않는다.
            if (0 < m_skillTimer)
            {
                m_agent.avoidancePriority = 49;
                destination = this.transform.position;
            }

            // 회피거리보다 가까우면
            else if (m_targetDistance < m_data.EscapeRange)
                destination = FindChasePos();

            // 공격범위보다 가까우면 멈춘다.
            else if (m_targetDistance <= m_data.ChaseRange)
            {
                m_agent.avoidancePriority = 49;
                destination = this.transform.position;
            }

            ModelRotate(destination, false, true);


            if (m_isStop == false)
                m_agent.SetDestination(destination);
        }
        protected override void MoveStateExit()
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

            if (HitStateCheck())
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
            scale -= Time.deltaTime * m_dieSpeed;
            scale = 0 <= scale ? scale : 0;
        }

        #endregion
    }


}