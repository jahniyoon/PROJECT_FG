using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public partial class TestEnemyE
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

            if (AttackRangeCheck() && TargetAngleCheck())
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
            float targetDistance = Vector3.Distance(transform.position, m_target.transform.position);

            // 공격 거리보다 가까우면
            if (targetDistance < m_data.EscapeRange)
            {
                Vector3 destination = FindChasePos();

                m_agent.SetDestination(destination);
                ModelRotate(destination);
                return;
            }

            m_agent.SetDestination(m_target.position);
            ModelRotate(m_target.position);
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
            // 공격 범위 밖일 때 이동상태가 된다.
            // + 조준중일 때 이동상태가 되지 않는다.
            //

            if (m_data.AttackRange < m_targetDistance && m_isAttackReady
                || TargetAngleCheck() == false && m_isAttackReady
            || m_isAttackReady == false && m_data.AttackRange < m_targetDistance)
                return new MoveState();

            return null;
        }

        private bool m_isAttackReady;
        private float m_aimTimer;
        private float m_shootTimer;
        protected override void AttackStateEnter()
        {
            ModelRotate(m_target.position, false, true);
            ShootOver();
        }

        protected override void AttackStateStay()
        {
            AimEnable(m_isAttackReady);
            m_targetPoint.gameObject.SetActive(m_isAttackReady == false);


            if (m_isAttackReady == false)
            { AimBehavior(); }

            else
            { AttackBehavior(); }
        }

        private void AimBehavior()
        {
            Vector3 position = m_target.position;
            position.y = m_targetPoint.transform.position.y;
            position.z += 1;
            m_targetPoint.transform.position = position;
            m_targetPoint.transform.localScale = (Vector3.one * (0.5f * (m_aimTimer - m_subData.AimSpeed)));
            // 타이머만큼 조준 준비
            if (m_subData.AimSpeed < m_aimTimer)
            {
                m_isAttackReady = true;
                m_aimTimer = 0;
                return;
            }
            ModelRotate(m_target.position);
            m_aimTimer += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            // 사격 지속시간이 지나면 사격 종료
            if (m_subData.ShootDuration < m_attackTimer)
            {
                ShootOver();
                return;
            }
            // 바로 발사하도록 하기위해, 슛 타이머가 0이 될 때마다 발사
            if (1 / m_subData.FireRate <= m_shootTimer && TargetAngleCheck() && m_targetDistance < m_data.AttackRange)
            {
                Shoot();
                m_shootTimer = 0;
            }
            AimSlider(m_attackTimer / m_subData.ShootDuration);

            m_shootTimer += Time.deltaTime;
            m_attackTimer += Time.deltaTime;
        }
        private void ShootOver()
        {
            m_attackTimer = 0;
            m_aimTimer = 0;
            m_shootTimer = 1 / m_subData.FireRate;
            m_isAttackReady = false;
            m_targetPoint.gameObject.SetActive(false);
        }

        protected override void AttackStateExit()
        {
            ShootOver();
            AimEnable(false);
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
            //m_stunCoolDown -= Time.deltaTime;
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