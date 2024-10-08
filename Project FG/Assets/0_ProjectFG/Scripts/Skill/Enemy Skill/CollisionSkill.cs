using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class CollisionSkill : SkillBase
    {

        protected override void Init()
        {
            //for (int i = 0; i < m_projectiles.Count; i++)
            //{
            //    var projectile = CreateProjectile(m_projectiles[i]);
            //    m_projectiles[i] = projectile;
            //}
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                var projectile = CreateProjectile(m_projectiles[i]);

                ActiveProjectile(projectile);
            }

        }




    }
}
