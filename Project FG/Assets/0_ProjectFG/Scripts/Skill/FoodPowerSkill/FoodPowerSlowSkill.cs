using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerSlowSkill : FoodPowerSkill
    {
        private FoodPowerFSkillData m_subData;
        private SphereCollider m_range;
        private Rigidbody m_rigidbody;

        private BuffBase m_buff;


        [Header("데미지")]
        [SerializeField] private float m_damageCoolDown;
        [Header("파티클")]
        [SerializeField] private ParticleSystem m_frozen;


        float[] values;
        protected override void Init()
        {
            m_subData = m_data as FoodPowerFSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

            m_range = transform.AddComponent<SphereCollider>();
            m_range.isTrigger = true;
            m_rigidbody = transform.AddComponent<Rigidbody>();
            m_rigidbody.isKinematic = true;

            m_buff = BuffFactory.CreateBuff(m_subData.SlowDebuff);

            values = new float[1] { m_levelData.GetAdditionalValue(0) };


        }


        public override void LeagcyActiveSkill()
        {
            base.LeagcyActiveSkill();
            m_range.radius = m_levelData.Radius;
            m_frozen.transform.localScale = Vector3.one * m_levelData.Radius;
            StartCoroutine(ActiveSkillRoutine());
        }

        protected override void UpdateBehavior()
        {
            if (m_damageCoolDown <= 0)
            {
                Damage();
            }
            // 0 미만이면 시간을 더 빼지 않는다.
            m_damageCoolDown = 0 <= m_damageCoolDown ? m_damageCoolDown - Time.deltaTime : 0;
        }
        private void Damage()
        {
            // 쿨타임 한번 체크한다.
            if (0 < m_damageCoolDown)
                return;
            // 쿨타임 업데이트
            m_damageCoolDown = m_levelData.CoolDown;


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
                }
            }

        }




        public override void InactiveSkill()
        {
            base.InactiveSkill();
            InactiveBuff();
            Destroy(gameObject);

        }

        private void InactiveBuff()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_levelData.Radius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag(m_subData.Target.ToString()))
                {
                    if (colls[i].TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
                    {
                        buffHandler.RemoveBuff(Caster, m_buff);
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
            //   Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_attackOffset, m_attackRadius);
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.isTrigger || IsActive == false)
                return;

            if (other.CompareTag(m_subData.Target.ToString()))
            {
                if (other.TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
                {
                    //var buff = m_subData.SlowDebuff as SlowDebuff;
                    //// TODO : SO를 셋 벨류하면 안됨 꼭 고쳐야함

                    m_buff.SetBuffValue(values);
                    buffHandler.OnBuff(Caster, m_buff);
                }
            }
        }

    }

}

