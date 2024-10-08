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

        public override void InactiveSkill()
        {
            base.InactiveSkill();
        }

        private void ShootProjectiles()
        {
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                var projectile = CreateProjectile(m_projectiles[i]);
                projectile.ActiveProjectile();
            }

        }
     


    }
}
