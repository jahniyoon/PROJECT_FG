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



        public bool TargetCheck()
        {
            if(m_player == null)
                return false;
            if (m_player.State == FSMState.Die)
                return false;
            return true;
        }

        public bool CheckFreezeState()
        {
            foreach(var skill in Skills)
            {
                if (skill.IsFixed)
                    return true;
            }
            return false;
        }

     
    }
}