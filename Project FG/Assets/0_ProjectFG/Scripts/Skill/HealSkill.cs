using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class HealSkill : SkillBase
    {
        private HealSkillData m_subData;
        private ParticleSystem m_radiusEffect;

        private SphereCollider m_range;
        private Rigidbody m_rigidbody;
        float m_dotDuration = 0;

        protected override void Init()
        {
            m_subData = m_skillData as HealSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

            m_radiusEffect = Instantiate(m_subData.HealPrefab, Caster.transform).GetComponent<ParticleSystem>();
            m_radiusEffect.transform.localEulerAngles = Vector3.zero;

            SetDuration(m_subData.SkillDuration);

            m_range = transform.AddComponent<SphereCollider>();
            m_range.isTrigger = true;
            m_range.radius = m_subData.SkillRadius;
            m_rigidbody = transform.AddComponent<Rigidbody>();
            m_rigidbody.isKinematic = true;
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            Vector3 scale = Vector3.one * m_subData.SkillRadius;
            scale.y = 1;
            m_radiusEffect.transform.localScale = scale;
            m_radiusEffect.Stop();
            m_radiusEffect.Play();

            StartCoroutine(SkillRoutine());
        }


        IEnumerator SkillRoutine()
        {
            float timer = 0;
            Vector3 effectScale = Vector3.one;

            while (timer < 0.5f)
            {
                effectScale.x = m_subData.SkillRadius * (timer / 0.5f);
                effectScale.z = effectScale.x;

                m_radiusEffect.transform.localScale = effectScale;
                timer += Time.deltaTime;
                yield return null;
            }
 

            yield break;
        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();
            m_radiusEffect.Stop();
            StopAllCoroutines();
            m_range.enabled = false;

        }

        private void OnTriggerStay(Collider other)
        {
            if (other.isTrigger || IsActive ==false)
                return;

            if (other.CompareTag(m_subData.Target.ToString()))
            {
                if (other.TryGetComponent<BuffHandler>(out BuffHandler buff))
                {
                    buff.OnBuff(Caster, m_subData.HealBuff);

                }
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.isTrigger || IsActive == false)
                return;

            if (other.CompareTag(m_subData.Target.ToString()))
            {
                if (other.TryGetComponent<BuffHandler>(out BuffHandler buff))
                {
                    buff.RemoveBuff(Caster, m_subData.HealBuff, true);

                  
                }
            }
        }

    }
}
