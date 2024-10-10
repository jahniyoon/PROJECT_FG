using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerSlowSkill : FoodPowerSkill
    {
        protected override void Init()
        {
            base.Init();
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            // 한 투사체만 생성
            ActiveProjectiles(onlyProjectile: true);

            PlayEffect();
        

            if (m_data.SkillTarget == TargetTag.Caster)
                OnBuff(Caster.Transform);
        }


        public override void InactiveSkill()
        {

            // 투사체를 리셋
            ResetProjectiles();

            if (m_data.SkillTarget == TargetTag.Caster)
                RemoveBuff(Caster.Transform);

            StopEffect();
            base.InactiveSkill();
        }
    }

}

