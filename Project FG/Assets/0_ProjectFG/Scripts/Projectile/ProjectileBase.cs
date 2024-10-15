using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class ProjectileBase : MonoBehaviour
    {
        protected Vector3 m_spawnPos;
        protected Transform m_model;
        protected Transform m_effectsTransform;
        [SerializeField] protected ProjectileData m_data;
        [Header("Caster Info")]
        [SerializeField] protected SkillBase m_skill;
        [SerializeField] protected bool m_derivatives;
        [SerializeField] protected List<BuffBase> m_buffs = new List<BuffBase>();
        [Header("Projectile Info")]

        [SerializeField] protected int m_penetrateCount;
        [SerializeField] protected float m_projectileSpeed;
        [SerializeField] protected float m_radius;
        [Header("Particles")]
        [SerializeField] private ParticleSystem[] m_particles;
        [SerializeField] private VisualEffect[] m_effects;


        [Header("Debug")]

        [SerializeField] protected Transform m_debug;
        [SerializeField] private bool m_debugMode;

        public int ID => m_data.ID;


        private void Awake()
        {
            m_model = transform.GetChild(0);
            if (m_model != null)
                m_model.transform.localScale = Vector3.one * m_data.ProjectileScale;

            m_effectsTransform = transform.GetChild(1);

            m_spawnPos = transform.position;
            m_penetrateCount = m_data.Penetrate;
            m_projectileSpeed = m_data.ProjectileSpeed;
            AwakeInit();
        }

        protected virtual void AwakeInit() { }

        public void SetSkill(SkillBase skill, bool derivatives)
        {
            m_derivatives = derivatives;
            SetSkill(skill);
        }
        public virtual void SetSkill(SkillBase skill)
        {
            m_skill = skill;
            m_buffs.Clear();
            for (int i = 0; i < m_skill.Buffs.Count; i++)
            {
                BuffBase buff = BuffFactory.CreateBuff(m_skill.Buffs[i].Data);
                if (buff == null)
                    continue;

                buff.SetCaster(transform);
                buff.SetBuffValue(m_skill.LevelData.BuffValues);
                m_buffs.Add(buff);
            }
            SetRadius(m_skill.LevelData.Radius);
        }

        public virtual void SetRadius(float radius)
        {
            m_radius = radius;
            m_effectsTransform.transform.localScale = Vector3.one * radius;
        }
        public void SetSpeedPenetrate(float speed, int penetrate)
        {
            m_penetrateCount = penetrate;
            m_projectileSpeed = speed;
        }
        // 투사체의 수 만큼 버프가 생겨야하므로 따로 관리
        protected void OnBuff(Transform target)
        {
            if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))

                for (int i = 0; i < m_buffs.Count; i++)
                {
                    buff.OnBuff(m_skill.Caster.GameObject, m_buffs[i]);
                }
        }




        protected void RemoveBuff(Transform target)
        {
            if (target == null)
            {
                Debug.LogError("타겟이 없다");
                return;
            }
            if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))

                for (int i = 0; i < m_buffs.Count; i++)
                {
                    buff.RemoveBuff(m_skill.Caster.GameObject, m_buffs[i]);
                }
        }

        public virtual void ActiveProjectile()
        {
            TryDebug();
            if (0 <= m_skill.LevelData.LifeTime)
                Invoke(nameof(InActiveProjectile), m_skill.LevelData.LifeTime);
        }

        public virtual ProjectileBase InActiveProjectile()
        {
            CreateDerivativesProjectiles();
            gameObject.SetActive(false);
            return this;
        }

        // 파생 투사체 생성
        public void CreateDerivativesProjectiles()
        {

            if (m_derivatives) return;

            foreach (var projectileID in m_data.DerivativesID)
            {
                var projectilePrefab = GFunc.GetProjectilePrefab(projectileID);
                var projectile = Instantiate(projectilePrefab, transform.position, transform.rotation, GameManager.Instance.ProjectileParent);
                projectile.SetSkill(m_skill, true);
                projectile.SetRadius(m_skill.LevelData.TryGetValue3());
                projectile.ActiveProjectile();
            }
        }

        protected void PlayEffect()
        {
            for (int i = 0; i < m_particles.Length; i++)
            {
                m_particles[i].gameObject.SetActive(true);
                m_particles[i].Stop();
                m_particles[i].Play();
            }
            for (int i = 0; i < m_effects.Length; i++)
            {
                m_effects[i].gameObject.SetActive(true);
                m_effects[i].Stop();
                m_effects[i].Play();
            }
        }

        protected void StopEffect()
        {
            for (int i = 0; i < m_particles.Length; i++)
            {
                m_particles[i].Stop();
                m_particles[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < m_effects.Length; i++)
            {
                m_effects[i].Stop();
                m_effects[i].gameObject.SetActive(false);
            }
        }


        private void TryDebug()
        {
            if (m_debugMode == false || m_debug == null)
                return;
            DebugProjectile();
        }
        protected virtual void DebugProjectile()
        {

        }
    }
}
