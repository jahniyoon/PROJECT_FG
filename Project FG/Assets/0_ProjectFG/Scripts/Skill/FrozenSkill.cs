using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class FrozenSkill : SkillBase
    {
        private FrozenSkillData m_subData;
        private ParticleSystem m_frozen;

        private SphereCollider m_range;
        private Rigidbody m_rigidbody;
        float m_dotDuration = 0;

        protected override void Init()
        {
            m_subData = m_skillData as FrozenSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

            m_frozen = Instantiate(m_subData.FrozenPrefab, Caster.transform).GetComponent<ParticleSystem>();
            m_frozen.transform.localEulerAngles = Vector3.zero;

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
            m_frozen.transform.localScale = scale;
            m_frozen.Stop();
            m_frozen.Play();

            StartCoroutine(SkillRoutine());
        }

        protected override void UpdateBehavior()
        {
            base.UpdateBehavior();
            m_dotDuration += Time.deltaTime;
        }
        IEnumerator SkillRoutine()
        {
            float timer = 0;
            Vector3 effectScale = Vector3.one;

            while (timer < 0.5f)
            {
                effectScale.x = m_subData.SkillRadius * (timer / 0.5f);
                effectScale.z = effectScale.x;

                m_frozen.transform.localScale = effectScale;
                timer += Time.deltaTime;
                yield return null;
            }
 

            yield break;
        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();
            m_frozen.Stop();
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
                    buff.OnBuff(Caster, m_subData.SlowDebuff);
                    buff.OnBuff(Caster, m_subData.DotDamageBuff);
                    buff.OnBuff(Caster, m_subData.FrozenDebuff);
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
                    buff.RemoveBuff(Caster, m_subData.FrozenDebuff, true);

                    buff.RemoveBuff(Caster, m_subData.SlowDebuff);
                    buff.RemoveBuff(Caster, m_subData.DotDamageBuff);
                }
            }
        }

    }
}
