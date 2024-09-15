using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerMineSkill : FoodPowerSkill
    {
        private FoodPowerHSkillData m_subData;
        private SphereCollider m_range;
        private Rigidbody m_rigidbody;


        [Header("이펙트")]
        [SerializeField] private ParticleSystem m_effect;
        [SerializeField] private GameObject m_radiusDebug;



        protected override void Init()
        {
            m_subData = m_skillData as FoodPowerHSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

            m_range = transform.AddComponent<SphereCollider>();
            m_range.isTrigger = true;
            m_rigidbody = transform.AddComponent<Rigidbody>();
            m_rigidbody.isKinematic = true;


        }


        public override void ActiveSkill()
        {
            base.ActiveSkill();
            m_range.radius = m_levelData.Radius;
            m_radiusDebug.transform.localScale = Vector3.one * m_levelData.Radius;

            StartCoroutine(SkillRoutine());
        }

     
        private void Explosion()
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

                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_levelData.Damage);
                    }                 
                }
            }
            var effect = Instantiate(m_effect.gameObject, transform.position, Quaternion.identity, GameManager.Instance.ProjectileParent).GetComponent<ParticleSystem>();
            effect.transform.localScale = Vector3.one * m_levelData.Radius;
            effect.Play();
            Destroy(effect.gameObject, 1f);
            InactiveSkill();

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
            //   Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_attackOffset, m_attackRadius);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger || IsActive == false)
                return;

            if (other.CompareTag(m_subData.Target.ToString()))
            {
                Explosion();
            }
        }

    }

}

