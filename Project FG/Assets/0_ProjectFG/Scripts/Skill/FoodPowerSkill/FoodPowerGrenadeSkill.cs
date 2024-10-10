using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.VFX;
using UnityEngine.SocialPlatforms;

namespace JH
{
    public class FoodPowerGrenadeSkill : FoodPowerSkill
    {
        public override void ActiveSkill()
        {
            bool canActiveSkill = ActiveProjectiles(true);
            if (canActiveSkill == false)
                return;
            base.ActiveSkill();
        }

    }
}
