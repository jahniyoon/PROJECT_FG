using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public partial class TestEnemyB
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
            ModelRotate(m_target.position, navDir: false);

            // 공격 거리보다 가까우면
            if (targetDistance < m_data.EscapeRange)
            {
                Vector3 destination = FindChasePos();

                m_agent.SetDestination(destination);
                return;
            }

            m_agent.SetDestination(m_target.position);
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

            if (AttackRangeCheck() == false)
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


            if(m_shildSkill.CanActiveSkill())
            {
                m_shildSkill.LeagcyActiveSkill();
            }

            // 버프중이 아니고, 공격 가능한지 체크
            else if(m_shildSkill.IsActive == false && m_subbData.AttackSpeed < m_attackTimer && CanAttackCheck())
            {
                // 공격 속도 타이머와 쿨타임 초기화
                ResetAttackTimer();
                m_attackCoolDown = m_subbData.AttackCoolDown;

                MeleeAttack();
            }

            ModelRotate(m_target.position, navDir: false);

            m_attackTimer += Time.deltaTime;
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