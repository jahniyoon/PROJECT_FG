using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
	public class ExplosionSkill : SkillBase
	{

        public override void CastSkill()
        {
            base.CastSkill();
            SetSkillFix();

            if (Caster.GameObject.TryGetComponent<SpriteColor>(out SpriteColor sprite))
                sprite.PlayFlicking();

        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();

            Explosion();

        }

        private void Explosion()
        {

            ShootProjectiles();
            if(Caster.GameObject.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.Die();

        }

    }
}
