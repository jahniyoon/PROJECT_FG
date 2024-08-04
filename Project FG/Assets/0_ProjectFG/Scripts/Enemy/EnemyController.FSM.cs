using JH;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public partial class EnemyController
{
    // 상태에 맞는 행동을 실행 및 업데이트
    private void FSMHandler()
    {
        // 이전 상태 저장을 하고
        FSM<EnemyController> prevState = m_fsm.StateTransition(this);
        m_fsm.action(this); // 상태 실행

        //  이전 상태와 현재 상태가 같지 않다면 전이한다.
        if (m_fsm.Equals(prevState) == false)
        {
            m_fsm.Exit(this);
            m_fsm = prevState;
        }
    }
    private void StateHandler(FSMState nextState)
    {
        m_state = nextState;
    }

    #region ▶ 상태 조건식

    // Freeze 상태 조건식
    protected virtual FSM<EnemyController> FreezeStateConditional()
    {
        return null;
    }
    // Idle 상태 조건식
    protected virtual FSM<EnemyController> IdleStateConditional()
    {
        return null;
    }
    // Move 상태 조건식
    protected virtual FSM<EnemyController> MoveStateConditional()
    {
        return null;
    }
    // Attack 상태 조건식
    protected virtual FSM<EnemyController> AttackStateConditional()
    {
        return null;
    }
    // Hit 상태 조건식
    protected virtual FSM<EnemyController> HitStateConditional()
    {
        return null;
    }    
    protected virtual FSM<EnemyController> GroggyStateConditional()
    {
        return null;
    }
    // Hit 상태 조건식
    protected virtual FSM<EnemyController> DieStateConditional()
    {
        return null;
    }
    #endregion


    #region ▶ STATE IDLE : 대기 상태
    public class IdleState : FSM<EnemyController>
    {
        // 상태 전이 조건을 넣는 메서드
        public override FSM<EnemyController> StateTransition(EnemyController t)
        {
            FSM<EnemyController> nextState = t.IdleStateConditional();
            if (nextState == null)
                return this;

            return nextState;
        }

        public override void Enter(EnemyController t)
        {
            base.Enter(t);
            t.StateHandler(FSMState.Idle);
        }
        public override void Stay(EnemyController t)
        {
            base.Stay(t);
            t.IdleStateStay();
        }
        public override void Exit(EnemyController t)
        {
            base.Exit(t);
            t.IdleStateExit();
        }
    }
    protected virtual void IdleStateEnter()
    {
        // 가상함수
    }
    protected virtual void IdleStateStay()
    {
        // 가상함수
    }
    protected virtual void IdleStateExit()
    {
        // 가상함수
    }
    #endregion

    #region ▶ STATE MOVE : 이동 상태
    public class MoveState : FSM<EnemyController>
    {
        // 상태 전이 조건을 넣는 메서드
        public override FSM<EnemyController> StateTransition(EnemyController t)
        {
            FSM<EnemyController> nextState = t.MoveStateConditional();
            if (nextState == null)
                return this;

            return nextState;
        }

        public override void Enter(EnemyController t)
        {
            base.Enter(t);
            t.StateHandler(FSMState.Move);
            t.MoveStateEnter();
        }
        public override void Stay(EnemyController t)
        {
            base.Stay(t);
            t.MoveStateStay();
        }

        public override void Exit(EnemyController t)
        {
            base.Exit(t);
            t.MoveStateExit();
        }
    }

    protected virtual void MoveStateEnter()
    {
        // 가상함수
    }
    protected virtual void MoveStateStay()
    {
        // 가상함수
    }
    protected virtual void MoveStateExit()
    {
        // 가상함수
    }
    #endregion

    #region ▶ STATE ATTACK : 공격 상태
    public class AttackState : FSM<EnemyController>
    {
        // 상태 전이 조건을 넣는 메서드
        public override FSM<EnemyController> StateTransition(EnemyController t)
        {
            FSM<EnemyController> nextState = t.AttackStateConditional();
            if (nextState == null)
                return this;

            return nextState;

        }

        public override void Enter(EnemyController t)
        {
            base.Enter(t);
            t.StateHandler(FSMState.Attack);
            t.AttackStateEnter();
        }
        public override void Stay(EnemyController t)
        {
            base.Stay(t);
            t.AttackStateStay();
        }
        public override void Exit(EnemyController t)
        {
            base.Exit(t);
            t.AttackStateExit();
        }
    }

    protected virtual void AttackStateEnter()
    {
        // 가상함수
    }
    protected virtual void AttackStateStay()
    {
        // 가상함수
    }
    protected virtual void AttackStateExit()
    {
        // 가상함수
    }
    #endregion

    #region ▶ STATE Hit : 피격 상태
    public class HitState : FSM<EnemyController>
    {
        // 상태 전이 조건을 넣는 메서드
        public override FSM<EnemyController> StateTransition(EnemyController t)
        {
            FSM<EnemyController> nextState = t.HitStateConditional();
            if (nextState == null)
                return this;

            return nextState;
        }

        public override void Enter(EnemyController t)
        {
            base.Enter(t);
            t.StateHandler(FSMState.Hit);
            t.HitStateEnter();
        }

        public override void Stay(EnemyController t)
        {
            base.Stay(t);
            t.HitStateStay();
        }
        public override void Exit(EnemyController t)
        {
            base.Exit(t);
            t.HitStateExit();
        }
    }

    protected virtual void HitStateEnter()
    {
        // 가상함수
    }
    protected virtual void HitStateStay()
    {
        // 가상함수
    }
    protected virtual void HitStateExit()
    {
        // 가상함수
    }
    #endregion


    #region ▶ STATE GROGGY : 그로기 상태
    public class GroggyState : FSM<EnemyController>
    {
        // 상태 전이 조건을 넣는 메서드
        public override FSM<EnemyController> StateTransition(EnemyController t)
        {
            FSM<EnemyController> nextState = t.GroggyStateConditional();
            if (nextState == null)
                return this;

            return nextState;
        }

        public override void Enter(EnemyController t)
        {
            base.Enter(t);
            t.StateHandler(FSMState.Groggy);
            t.GroggyStateEnter();
        }

        public override void Stay(EnemyController t)
        {
            base.Stay(t);
            t.GroggyStateStay();
        }
        public override void Exit(EnemyController t)
        {
            base.Exit(t);
            t.GroggyStateExit();
        }
    }

    protected virtual void GroggyStateEnter()
    {
        // 가상함수
    }
    protected virtual void GroggyStateStay()
    {
        // 가상함수
    }
    protected virtual void GroggyStateExit()
    {
        // 가상함수
    }
    #endregion

    #region ▶ STATE DIE : 사망 상태
    public class DieState : FSM<EnemyController>
    {
        // 상태 전이 조건을 넣는 메서드
        public override FSM<EnemyController> StateTransition(EnemyController t)
        {
            FSM<EnemyController> nextState = t.DieStateConditional();
            if (nextState == null)
                return this;

            return nextState;
        }

        public override void Enter(EnemyController t)
        {
            base.Enter(t);
            t.StateHandler(FSMState.Die);
            t.DieStateEnter();
        }
        public override void Stay(EnemyController t)
        {
            base.Stay(t);
            t.DieStateStay();
        }
        public override void Exit(EnemyController t)
        {
            base.Exit(t);
            t.DieStateExit();
        }
    }

    protected virtual void DieStateEnter()
    {
        // 가상함수
    }
    protected virtual void DieStateStay()
    {
        // 가상함수
    }
    protected virtual void DieStateExit()
    {
        // 가상함수
    }
    #endregion
}
