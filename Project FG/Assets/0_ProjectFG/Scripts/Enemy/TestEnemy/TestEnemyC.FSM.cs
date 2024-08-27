using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public partial class TestEnemyC
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
        protected override void MoveStateEnter()
        {
            m_agent.isStopped = false;
        }

        protected override void MoveStateStay()
        {
            ModelRotate(m_target.position, navDir: false);

            // 에네미 C는 무브 스테이트 내에서 이동과 공격 상태를 갖는다.
            MoveBehavior();
            AttackBehavior();
        }

        private void MoveBehavior()
        {
            Vector3 destination = m_target.position;

            // 회피 범위보다 가까우면 가까우면 
            if (m_targetDistance < m_data.EscapeRange)
                destination = FindChasePos();
            // 브레이크를 위해
            else if (m_targetDistance < m_data.EscapeRange + 0.25f)
                destination = transform.position;

            m_agent.SetDestination(destination);
        }

        private void AttackBehavior()
        {
            if (m_targetDistance < m_data.AttackRange && CanAttackCheck())
            {
                m_attackCoolDown = m_subData.AttackCoolDown;
                ShootProjectile();
            }
        }

        protected override void MoveStateExit()
        {
            m_agent.SetDestination(this.transform.position);
            m_agent.isStopped = true;
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