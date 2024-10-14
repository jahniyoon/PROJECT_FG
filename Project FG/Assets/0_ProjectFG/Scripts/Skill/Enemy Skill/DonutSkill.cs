using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class DonutSkill : SkillBase
    {

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            ShootProjectiles();
        }

 


    }
}
