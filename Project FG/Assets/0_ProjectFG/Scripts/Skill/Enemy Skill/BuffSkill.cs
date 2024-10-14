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


            foreach(var skill in Caster.Skills)
            {
                if(skill.Data.BaseType == SkillType.Buff)
                    continue;
                skill.FreezeSkill();
            }
        }

        public override void InactiveSkill()
        {
            StopEffect();

            if (m_data.SkillTarget == TargetTag.Caster)
                RemoveBuff(Caster.Transform);

            foreach (var skill in Caster.Skills)
            {
                if (skill.Data.BaseType == SkillType.Buff)
                    continue;
                skill.FreezeSkill(false);
            }

            base.InactiveSkill();

        }


    }
}
