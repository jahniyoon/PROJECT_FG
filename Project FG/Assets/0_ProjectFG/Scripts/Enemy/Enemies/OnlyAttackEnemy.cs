using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class OnlyAttackEnemy : EnemyController
    {


        protected override void StartInit()
        {
            base.StartInit();
        }

        protected override void Die()
        {
            // 죽으면 모든 스킬을 꺼준다.
            for (int i = 0; i < m_routineSkills.Count; i++)
                m_routineSkills[i].InactiveSkill();

            for (int i = 0; i < m_attackSkills.Count; i++)
                m_attackSkills[i].InactiveSkill();

            base.Die();
        }



  

        // 타입별로 공격 전 체크
        public override bool CanActiveSkill()
        {
            if (m_state != FSMState.Attack)
                return false;


            for (int i = 0; i < m_routineSkills.Count; i++)
            {
                if (m_routineSkills[i].Data.BaseType == SkillType.Buff && m_routineSkills[i].IsActive)
                    return false;
            }
            for (int i = 0; i < m_attackSkills.Count; i++)
            {
                if (m_attackSkills[i].Data.BaseType == SkillType.Buff && m_attackSkills[i].IsActive)
                    return false;
            }

            return true;
        }


        protected void Explosion()
        {
            m_spriteColor?.StopFlicking();
            m_damageable.Die();
        }
    }
}