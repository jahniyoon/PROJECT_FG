using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerSlashSkill : FoodPowerSkill
    {


        public override void ActiveSkill()
        {
            bool canActiveSkill = ActiveProjectiles(onlyProjectile:true);
            if (canActiveSkill == false)
                return;
            base.ActiveSkill();
        }




    }
}
