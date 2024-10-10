using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TowerEnemy : EnemyController
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

        public override bool CanActiveSkill()
        {
            return true;
        }



    }
}