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

        [Header("AOE 스킬")]

        [SerializeField] private ParticleSystem[] m_effects;

        protected override void Init()
        {
            base.Init();
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                var projectile = CreateProjectile(m_projectiles[i]);
                m_projectiles[i] = projectile;
            }

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
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                ActiveProjectile(m_projectiles[i]);
            }
            foreach (var effect in m_effects)
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
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                m_projectiles[i].InActiveProjectile();
            }
            foreach (var effect in m_effects)
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
