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
        Coroutine m_skillCoroutine;

        BuffBase m_slowBuff;
        BuffBase m_frozenBuff;
        BuffBase m_dotDamageBuff;

        protected override void Init()
        {
            m_subData = m_data as FrozenSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

            m_frozen = Instantiate(m_subData.FrozenPrefab, Caster.Transform).GetComponent<ParticleSystem>();
            m_frozen.transform.localEulerAngles = Vector3.zero;

            SetDuration(m_subData.Duration);

            m_range = transform.AddComponent<SphereCollider>();
            m_range.isTrigger = true;
            m_range.radius = m_subData.Radius;
            m_rigidbody = transform.AddComponent<Rigidbody>();
            m_rigidbody.isKinematic = true;

            m_slowBuff = BuffFactory.CreateBuff(m_subData.SlowDebuff);
            m_frozenBuff = BuffFactory.CreateBuff(m_subData.FrozenDebuff);
            m_dotDamageBuff = BuffFactory.CreateBuff(m_subData.DotDamageBuff);
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            Vector3 scale = Vector3.one * m_subData.Radius;
            scale.y = 1;
            m_frozen.transform.localScale = scale;
            m_frozen.Stop();
            m_frozen.Play();
            if(m_skillCoroutine != null)
            {
                StopCoroutine(m_skillCoroutine);
                m_skillCoroutine = null;
            }
            m_skillCoroutine = StartCoroutine(SkillRoutine());
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
                effectScale.x = m_subData.Radius * (timer / 0.5f);
                effectScale.z = effectScale.x;

                m_frozen.transform.localScale = effectScale;
                timer += Time.deltaTime;
                yield return null;
            }

            if (m_data.Duration < 0)
            {
                yield break;
            }

            while (timer < m_data.Duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            InactiveSkill();


            yield break;
        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();
            if (m_frozen != null)
            {
                m_frozen?.Stop();
                Destroy(m_frozen.gameObject);

            }
            if (m_skillCoroutine != null)
            {
                StopCoroutine(m_skillCoroutine);
                m_skillCoroutine = null;
            }

            m_range.enabled = false;
            Destroy(this.gameObject);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.isTrigger || IsActive == false)
                return;

            if (other.CompareTag(m_subData.Target.ToString()))
            {
                if (other.TryGetComponent<BuffHandler>(out BuffHandler buff))
                {
                    buff.OnBuff(Caster.GameObject, m_slowBuff);
                    buff.OnBuff(Caster.GameObject, m_dotDamageBuff);
                    buff.OnBuff(Caster.GameObject, m_frozenBuff);
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
                    buff.RemoveBuff(Caster.GameObject, m_frozenBuff);

                    buff.RemoveBuff(Caster.GameObject, m_slowBuff);
                    buff.RemoveBuff(Caster.GameObject, m_dotDamageBuff);
                }
            }
        }

    }
}
