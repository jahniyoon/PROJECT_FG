using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyF : EnemyController
    {

        private EnemyFData m_subData;

        private ParticleSystem m_skillEffect;
        [SerializeField] private SkillBase m_frozenSkill;
        private float m_skillCoolDownTimer;

        protected override void StartInit()
        {
            base.StartInit();

            EnemyFData m_childData = m_data as EnemyFData;
            if (m_childData != null)
            {
                m_subData = m_childData;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
            }

        }

        private void ActiveSkill()
        {
            if(m_frozenSkill != null)
            {
                m_frozenSkill.InactiveSkill();
                m_frozenSkill = null;
            }

            m_frozenSkill = Instantiate(m_subData.FrozenSkill, transform).GetComponent<SkillBase>();
            m_frozenSkill.SkillInit(this.gameObject, m_model);
            m_frozenSkill.ActiveSkill();
        }
        private void InactiveSkill()
        {
            m_frozenSkill.InactiveSkill();
        }

        protected override void UpdateBehaviour()
        {
            base.UpdateBehaviour();
            m_skillCoolDownTimer = m_skillCoolDownTimer - Time.deltaTime <= 0 ? 0 : m_skillCoolDownTimer -= Time.deltaTime;
        }

        protected override void Die()
        {
            base.Die();
            m_frozenSkill.InactiveSkill();
        }


    }
}