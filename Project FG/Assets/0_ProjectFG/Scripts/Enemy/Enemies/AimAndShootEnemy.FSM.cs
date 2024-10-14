using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public partial class AimAndShootEnemy
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

            if (AttackRangeCheck())
                return new AttackState();

            return null;
        }
        protected override void MoveStateEnter()
        {
            // 정지를 시킨경우 켜지 않는다.
            m_agent.isStopped = m_isStop;
        }

        protected override void MoveStateStay()
        {
            // 스킬 타이머동안은 움직이지 않는다.
            if (0 < m_skillTimer)
                return;

            Vector3 destination = m_target.position;

            // 회피거리보다 가까우면
            if (m_targetDistance < m_data.EscapeRange)
                destination = FindChasePos();

            // 공격범위보다 가까우면 멈춘다.
            else if (m_targetDistance < m_data.ChaseRange)
                destination = this.transform.position;

            ModelRotate(destination, false, true);


            if (m_isStop == false)
                m_agent.SetDestination(destination);
        }
        protected override void MoveStateExit()
        {
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

            if (m_target == null)
                return new IdleState();

            if (AttackAimCheck() == false)
                return new MoveState();

            return null;
        }

        protected override void AttackStateEnter()
        {
            base.AttackStateEnter();
            ResetTimer();
            SetAim(false);
        }

        protected override void AttackStateStay()
        {
            base.AttackStateStay();

            if (m_aimSkill == null)
                return;


            if (m_isAim == false)
            {
                ModelRotate(m_target.position, false, true);
                AimBehavior();
            }
            else
                ShootBehavior();
        }

        private void AimBehavior()
        {
            m_aimState = AimState.Aim;

            // 조준 완료
            if (m_aimSkill.Data.TryGetValue1(1) <= m_aimTimer)
            {
                SetAim(true);
                ResetTimer();
                return;
            }

            m_aimTimer += Time.deltaTime;

        }
        private void ShootBehavior()
        {
            m_aimState = AimState.Shoot;

            // 사격 종료
            if (m_aimSkill.Data.Duration <= m_shootingTimer)
            {
                m_aimState = AimState.Reload;

                SetAim(false);
                ResetTimer();
                 return;
            }

            m_shootingTimer += Time.deltaTime;
        }
        protected override void AttackStateExit()
        {
            base.AttackStateExit();
            m_aimState = AimState.Idle;

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
        }

        #endregion
    }


}