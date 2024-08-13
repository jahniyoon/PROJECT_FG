using System;

namespace JH
{
    // 유한 상태 머신
    public abstract class FSM<T>
    {
        public Action<T> action;  // State에 따라 동작하는 델리게이트
        public abstract FSM<T> StateTransition(T t); // 상태 전이를 하는 구조체. 다음 상태를 반환

        // State 클래스의 생성자
        public FSM()
        {
            action = Enter;
        }

        // 상태에 진입 했을 때
        public virtual void Enter(T t)
        {
            action = Stay;
        }

        // 상태가 진행 중일 때
        public virtual void Stay(T t)
        {
        }

        // 상태가 종료되었을 때
        public virtual void Exit(T t)
        {
        }
    }

    public enum FSMState
    {
        Freeze,       // 멈춤
        Idle,       // 대기
        Move,       // 이동
        Attack,     // 공격
        Groggy,     // 그로기
        Predation,  // 포식
        Hit,        // 피격
        Die         // 사망
    }
}