using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class BuffSkill : SkillBase
    {
 


        public override void ActiveSkill()
        {
            base.ActiveSkill();
            PlayEffect();

            if (m_data.SkillTarget == TargetTag.Caster)
                OnBuff(Caster.Transform);
        }

        public override void InactiveSkill()
        {
            StopEffect();

            if (m_data.SkillTarget == TargetTag.Caster)
                RemoveBuff(Caster.Transform);

            base.InactiveSkill();

        }


    }
}
