using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class AreaSkill : SkillBase
    {
        protected override void Init()
        {
            base.Init();

            if (Data.TryGetValue1() != 0)
            {
                m_skillCoolDown = Random.Range(Data.TryGetValue1(), Data.TryGetValue1(1));
                if (m_data.ActiveTime == SkillActiveTime.CoolDown)
                    ResetTimer();
            }
            
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();

            // 한 투사체만 필요하므로
            ActiveProjectiles(onlyProjectile:true);
            PlayEffect();

            if (m_data.SkillTarget == TargetTag.Caster)
                OnBuff(Caster.Transform);
        }


        public override void InactiveSkill()
        {
            ResetProjectiles();

            if (m_data.SkillTarget == TargetTag.Caster)
                RemoveBuff(Caster.Transform);

            StopEffect();
            base.InactiveSkill();

        }



    }
}
