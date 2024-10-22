using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

namespace JH
{
    public partial class TestEnemyG 
    {
        #region IDLE STATE
        protected override FSM<EnemyController> IdleStateConditional()
        {
            if (m_damageable.IsDie)
                return new DieState();

            if (CCStateCheck())
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

            if (CCStateCheck())
                return new HitState();

            if (m_target == null || CanPredation)
                return new IdleState();

            if (CanConversion())
                return new AttackState();

            return null;
        }
        protected override void MoveStateEnter()
        {
            m_agent.isStopped = false;
            m_conversionTimer = 0;
            m_conversionTime = Random.Range(m_subData.ConversionMinTime, m_subData.ConversionMaxTime);
        }

        protected override void MoveStateStay()
        {
            float targetDistance = Vector3.Distance(transform.position, m_target.transform.position);

            // 공격 거리보다 가까우면
            if (targetDistance < m_data.EscapeRange)
            {
                Vector3 destination = FindEscapePos();

                m_agent.SetDestination(destination);
                ModelRotate(destination);
                return;
            }

            m_agent.SetDestination(m_target.position);
            ModelRotate(m_target.position);

            m_conversionTimer += Time.deltaTime;

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

            if(CanPredation)
                return new IdleState();

                return null;
        }

        protected override void AttackStateEnter()
        {
            m_healSkill = Instantiate(m_subData.HealSkill.gameObject, transform.position, transform.rotation, transform).GetComponent<SkillBase>();
            //m_healSkill.SkillInit(this.gameObject, m_model);
            m_agent.enabled = false;

        }

        protected override void AttackStateStay()
        {            
            ModelRotate(m_target.position);


        }

        protected override void AttackStateExit()
        {
            base.AttackStateExit();
            m_healSkill.InactiveSkill();

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
            scale -= Time.deltaTime;
        }

        #endregion
    }


}