using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerMineSkill : FoodPowerSkill
    {
        public override void ActiveSkill()
        {
            bool canActiveSkill = ShootProjectiles();
            if (canActiveSkill == false)
                return;
            base.ActiveSkill();
        }

    }

}

