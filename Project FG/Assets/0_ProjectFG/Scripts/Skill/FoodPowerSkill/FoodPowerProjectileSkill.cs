using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerProjectileSkill : FoodPowerSkill
    {

        public override void ActiveSkill()
        {
            bool canActiveSkill = ShootProjectiles();
            if (canActiveSkill == false)
                return;
            base.ActiveSkill();
        }

        protected override void SetProjectile(ProjectileBase projectile)
        {
            base.SetProjectile(projectile);
            //Debug.Log(LevelData.TryGetValue1() + " " + LevelData.TryGetValue1(1));
            projectile.SetSpeedPenetrate(LevelData.TryGetValue1(), Mathf.FloorToInt(LevelData.TryGetValue1(1)));
        }
    }
}
