using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class ProjectileBase : MonoBehaviour
    {
        [SerializeField] protected ProjectileData m_data;
        [Header("Caster Info")]
        [SerializeField] protected SkillBase m_skill;
        [SerializeField] protected List<BuffBase> m_buffs = new List<BuffBase>();


        public int ID => m_data.ID;


        private void Awake()
        {
            var child = transform.GetChild(0);  
            if(child != null)
                child.transform.localScale = Vector3.one * m_data.ProjectileScale;
            AwakeInit();
        }
        protected virtual void AwakeInit() { }
        public virtual void SetSkill(SkillBase skill)
        {
            m_skill = skill;

            for(int i = 0; i < m_skill.Buffs.Count; i++)
            {
                BuffBase buff = BuffFactory.CreateBuff(m_skill.Buffs[i].Data);
                if (buff == null)
                    continue;

                buff.SetCaster(this.transform);
                buff.SetBuffValue(m_skill.Data.BuffValues);
                m_buffs.Add(buff);
            }
        }

        // 투사체의 수 만큼 버프가 생겨야하므로 따로 관리
        protected void OnBuff(Transform target)
        {
            for(int i = 0; i < m_buffs.Count; i++)
            {
                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.OnBuff(m_skill.Caster, m_buffs[i]);
            }
        }

        protected void RemoveBuff(Transform target)
        {
            for (int i = 0; i < m_buffs.Count; i++)
            {
                if (target.TryGetComponent<BuffHandler>(out BuffHandler buff))
                    buff.RemoveBuff(m_skill.Caster, m_buffs[i]);
            }
        }

        public virtual void ActiveProjectile()
        {
            // TODO : 실행시 라이프타임 추가
        }
        public virtual ProjectileBase InActiveProjectile()
        {
            Destroy(gameObject);
            return this;
        }
    }
}
