using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class DefaultEnemy : EnemyController
    {


        protected override void StartInit()
        {
            base.StartInit();
        }

        public bool TargetCheck()
        {
            if(m_player == null)
                return false;
            if (m_player.State == FSMState.Die)
                return false;
            return true;
        }
        protected override void Die()
        {
            base.Die();
        }



    }
}