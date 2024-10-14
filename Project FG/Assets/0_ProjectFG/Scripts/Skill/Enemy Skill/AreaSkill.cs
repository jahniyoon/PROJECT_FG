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

        public sealed override void ActiveSkill()
        {
            base.ActiveSkill();

            // 한 투사체만 필요하므로
            ActiveProjectiles();
            PlayEffect();

            if (m_data.SkillTarget == TargetTag.Caster)
                OnBuff(Caster.Transform);
            ActiveArea();
        }
        protected virtual void ActiveArea() { }
        protected virtual void InactiveArea() { }



        public sealed override void InactiveSkill()
        {
            ResetProjectiles();

            if (m_data.SkillTarget == TargetTag.Caster)
                RemoveBuff(Caster.Transform);

            StopEffect();
            InactiveArea();
            base.InactiveSkill();

        }



    }
}
