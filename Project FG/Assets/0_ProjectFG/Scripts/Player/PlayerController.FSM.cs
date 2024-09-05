using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public partial class PlayerController
    {
        // 상태에 맞는 행동을 실행 및 업데이트
        private void FSMHandler()
        {
            // 이전 상태 저장을 하고
            FSM<PlayerController> prevState = m_fsm.StateTransition(this);
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
            m_playerState = nextState;
        }


        #region ▶ STATE FREEZE : 프리즈 상태
        public class FreezeState : FSM<PlayerController>
        {
            // 상태 전이 조건을 넣는 메서드
            public override FSM<PlayerController> StateTransition(PlayerController t)
            {

                if (t.m_isFreeze == false)
                    return new IdleState();

                return this;
            }

            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t.StateHandler(FSMState.Freeze);
            }
            // 상태 중일 때 실행될 메서드
            public override void Stay(PlayerController t)
            {
                base.Stay(t);
            }

            // 상태를 빠져 나갈 때 실행될 메서드
            public override void Exit(PlayerController t)
            {
                base.Exit(t);
            }
        }
        #endregion


        #region ▶ STATE IDLE : 대기 상태
        public class IdleState : FSM<PlayerController>
        {
            // 상태 전이 조건을 넣는 메서드
            public override FSM<PlayerController> StateTransition(PlayerController t)
            {
                if (t.m_damageable.IsDie)
                    return new DieState();

                if (t.m_isFreeze)
                    return new FreezeState();

                if (t.m_predation.IsPredation)
                    return new PredationState();

                if(t.m_attack.isAttack)
                    return new AttackState();

                // 입력이 있으면 이동 상태
                if (t.Input.Move != Vector2.zero)
                    return new MoveState();
                

                return this;
            }

            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t.StateHandler(FSMState.Idle);
            }
            // 상태 중일 때 실행될 메서드
            public override void Stay(PlayerController t)
            {
                base.Stay(t);
            }

            // 상태를 빠져 나갈 때 실행될 메서드
            public override void Exit(PlayerController t)
            {
                base.Exit(t);
            }
        }
        #endregion

        #region ▶ STATE MOVE : 이동 상태
        public class MoveState : FSM<PlayerController>
        {
            // 상태 전이 조건을 넣는 메서드
            public override FSM<PlayerController> StateTransition(PlayerController t)
            {
                if (t.m_damageable.IsDie)
                    return new DieState();

                if (t.m_isFreeze)
                    return new FreezeState();

                if (t.m_predation.PredationTarget != null)
                    return new PredationState();

                if (t.m_attack.isAttack)
                    return new AttackState();

                // 입력이 없으면 대기 상태
                if (t.Input.Move == Vector2.zero)
                    return new IdleState();

                return this;
            }

            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t.StateHandler(FSMState.Move);
            }
            // 상태 중일 때 실행될 메서드
            public override void Stay(PlayerController t)
            {
                base.Stay(t);

                UpdateDirection(t);
                Movement(t);
            }

            // 상태를 빠져 나갈 때 실행될 메서드
            public override void Exit(PlayerController t)
            {
                base.Exit(t);
            }

            private Vector3 m_lookDir;


            // 방향 업데이트
            private void UpdateDirection(PlayerController t)
            {     
                m_lookDir.x = t.m_model.forward.x;
                m_lookDir.z = t.m_model.forward.z;

                if (t.Input.Move != Vector2.zero)
                {
                    m_lookDir.x = t.Input.Move.x;
                    m_lookDir.z = t.Input.Move.y;
                }
            }

            private void Movement(PlayerController t)
            {
                // 카메라의 방향과 이동 방향을 곱하여 플레이어의 이동 속도 벡터 계산
                Vector3 cameraForward = Camera.main.transform.forward;
                cameraForward.y = 0f; // 수평 방향 벡터로 설정

                Vector3 moveDirWorld = Quaternion.FromToRotation(Vector3.forward, cameraForward) * m_lookDir;

                // 방향에 맞게 이동시킨다.
                Vector3 velocity = moveDirWorld * (t.m_movement.FinalSpeed(t.m_gameSettings.PlayerMoveSpeed) * Time.deltaTime);


                t.m_movement.Movement(velocity);
                t.m_movement.LookRotation(moveDirWorld);
            }
        }
        #endregion

        #region ▶ STATE ATTACK : 공격 상태
        public class AttackState : FSM<PlayerController>
        {
            // 상태 전이 조건을 넣는 메서드
            public override FSM<PlayerController> StateTransition(PlayerController t)
            {
                if (t.m_damageable.IsDie)
                    return new DieState();

                if (t.m_isFreeze)
                    return new FreezeState();

                if (t.m_predation.PredationTarget != null)
                    return new PredationState();

                if (t.m_attack.isAttack)
                    return this;

                return new IdleState();
            }

            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t.StateHandler(FSMState.Attack);
            }
            // 상태 중일 때 실행될 메서드
            public override void Stay(PlayerController t)
            {
                base.Stay(t);
            }

            // 상태를 빠져 나갈 때 실행될 메서드
            public override void Exit(PlayerController t)
            {
                base.Exit(t);
            }
        }
        #endregion

        #region ▶ STATE Predation : 포식 상태
        public class PredationState : FSM<PlayerController>
        {
            // 상태 전이 조건을 넣는 메서드
            public override FSM<PlayerController> StateTransition(PlayerController t)
            {
                if (t.m_damageable.IsDie)
                    return new DieState();

                if (t.m_isFreeze)
                    return new FreezeState();

                if (t.m_predation.IsPredation == false)
                    return new IdleState();

                return this;
            }

            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t.StateHandler(FSMState.Predation);
                t.m_predation.PredationDash();
            }
            // 상태 중일 때 실행될 메서드
            public override void Stay(PlayerController t)
            {
                base.Stay(t);
            }

            // 상태를 빠져 나갈 때 실행될 메서드
            public override void Exit(PlayerController t)
            {
                base.Exit(t);
            }


          
        }
        #endregion

        #region ▶ STATE DIE : 사망 상태
        public class DieState : FSM<PlayerController>
        {
            // 상태 전이 조건을 넣는 메서드
            public override FSM<PlayerController> StateTransition(PlayerController t)
            {
             
                if(t.m_damageable.IsDie == false)
                    return new IdleState();

                return this;
            }

            public override void Enter(PlayerController t)
            {
                base.Enter(t);
                t.StateHandler(FSMState.Die);
            }
            // 상태 중일 때 실행될 메서드
            public override void Stay(PlayerController t)
            {
                base.Stay(t);
            }

            // 상태를 빠져 나갈 때 실행될 메서드
            public override void Exit(PlayerController t)
            {
                base.Exit(t);
            }
        }
        #endregion
    }
}