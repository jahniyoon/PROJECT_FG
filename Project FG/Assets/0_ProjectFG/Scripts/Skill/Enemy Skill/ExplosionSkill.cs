using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
	public class ExplosionSkill : SkillBase
	{
        [SerializeField] private VisualEffect m_collisionEffect;

        public override void LeagcyActiveSkill()
        {
            base.LeagcyActiveSkill();
            Invoke(nameof(StopEffect), m_data.SkillLifeTime);
        }

        private void StopEffect()
        {
            if (m_collisionEffect != null)
            {
                m_collisionEffect.Stop();
            }
        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();

        }

    }
}
