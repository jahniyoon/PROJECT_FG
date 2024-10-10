using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class CollisionSkill : SkillBase
    {

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            ActiveProjectiles(onlyProjectile: true);
        }
    }
}
