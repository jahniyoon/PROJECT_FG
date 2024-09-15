using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerAimSkill : FoodPowerSkill
    {

        private FoodPowerASkillData m_subData;
        [Header("에임")]
        [SerializeField] private float m_targetResearchTime = 0.1f;
        [SerializeField] private Transform m_target;
        float m_targetResearchTimer;

        [Header("이펙트")]
        [SerializeField] private GameObject m_aimEffect;

        protected override void Init()
        {
            m_subData = m_skillData as FoodPowerASkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            StartCoroutine(SkillRoutine());
        }
        protected override void UpdateBehavior()
        {
            if (m_targetResearchTime <= m_targetResearchTimer)
            {
                ResearchTarget();
            }

            m_targetResearchTimer += Time.deltaTime;


        }
        private void ResearchTarget()
        {
            m_targetResearchTime = 0;
        }

        private Vector3 aimScale = Vector3.one;
        private void UpdateAimTargetLine()
        {
            m_aimEffect.gameObject.SetActive(m_target != null);

            if (m_target == null)
                return;

            Quaternion rotation = Quaternion.identity;

            m_aimEffect.transform.localRotation = rotation;


            float Distance = Vector3.Distance(transform.position, m_target.position);
            aimScale.z = Distance;
            m_aimEffect.transform.localScale = aimScale;


        }


        public override void InactiveSkill()
        {
            base.InactiveSkill();
            Destroy(gameObject);

        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
        }

    }
}
