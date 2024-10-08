using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class BuffSkill : SkillBase
    {
        [Header("버프 스킬")]

        [SerializeField] private ParticleSystem[] m_buffEffects;



        public override void ActiveSkill()
        {
            base.ActiveSkill();
            foreach (var effect in m_buffEffects)
            {
                effect.gameObject.SetActive(true);
                effect.Stop();
                effect.Play();
            }
            if (m_data.SkillTarget == TargetTag.Caster)
                OnBuff(Caster.transform);
        }

        public override void InactiveSkill()
        {
            foreach (var effect in m_buffEffects)
            {
                effect.Stop();
                effect.gameObject.SetActive(false);
            }
            if (m_data.SkillTarget == TargetTag.Caster)
                RemoveBuff(Caster.transform);

            base.InactiveSkill();

        }


    }
}
