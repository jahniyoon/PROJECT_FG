using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerShieldSkill : FoodPowerSkill
    {
        private FoodPowerBSkillData m_subData;
        [Header("실드 스킬")]
        [SerializeField] private GameObject m_shield;
        [SerializeField] private ParticleSystem m_knockbackEffect;
        [SerializeField] private float m_knockbackCooldown;
        [Header("넉백 시간")]
        [SerializeField] private float m_knockbackDuration = 0.1f;



        protected override void Init()
        {
            m_subData = m_skillData as FoodPowerBSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

        }
        // 비활성화되면 리스너를 모두 제거한다.
        private void OnDisable()
        {
            if (Caster.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.DamageEvent.RemoveListener(KnockBack);
            }
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();

            // 피해감소 스킬
            if (Caster.TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
            {
                var buff = m_subData.DamageReductionBuff as DamageReductionBuff;
                // TODO : SO를 셋 벨류하면 안됨 꼭 고쳐야함
                buff.SetValue(m_levelData.GetAdditionalValue(0));
                buffHandler.OnBuff(Caster, m_subData.DamageReductionBuff);
            }

            // 넉백 이벤트 연결
            if (Caster.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.DamageEvent.AddListener(KnockBack);
            }


            StartCoroutine(SkillRoutine());
        }

        protected override void UpdateBehavior()
        {
            // 0 미만이면 시간을 더 빼지 않는다.
            m_knockbackCooldown = 0 <= m_knockbackCooldown ? m_knockbackCooldown - Time.deltaTime : 0;
        }



        public void KnockBack()
        {
            // 쿨타임 한번 체크한다.
            if (0 < m_knockbackCooldown)
                return;
            // 쿨타임 업데이트
            m_knockbackCooldown = m_levelData.CoolDown;

            m_knockbackEffect.Stop();
            m_knockbackEffect.transform.parent.localScale = Vector3.one * m_levelData.Radius;
            m_knockbackEffect.Play();

            Collider[] colls = Physics.OverlapSphere(transform.position, m_levelData.Radius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }


                if (colls[i].CompareTag(m_subData.Target.ToString()))
                {

                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_levelData.Damage);
                    }
                    if (colls[i].TryGetComponent<IKnockbackable>(out IKnockbackable knockbackable))
                    {
                        float distance = Vector3.Distance(transform.position, colls[i].transform.position);
                        Vector3 hitPoint = colls[i].ClosestPoint(transform.position);
                        knockbackable.OnKnockback(hitPoint, m_levelData.GetAdditionalValue(1), m_knockbackDuration);
                    }


                }
            }


        }


        public override void InactiveSkill()
        {
            base.InactiveSkill();

            if (Caster.TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
            {
                buffHandler.RemoveBuff(Caster, m_subData.DamageReductionBuff);
            }

            Destroy(gameObject);

        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
            //   Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_attackOffset, m_attackRadius);
        }

    }
}
