using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerSoulEaterSkill : FoodPowerSkill
    {
        private FoodPowerGSkillData m_subData;
        float m_damageTimer = 0;

        private Damageable m_casterDamageable;

        private Rigidbody m_rigidbody;
        private SphereCollider m_collider;
        [SerializeField] private SerializableDictionary<int, Damageable> m_damageableDic = new SerializableDictionary<int, Damageable>();


        [Header("영혼 포식자 스킬")]
        [SerializeField] private GameObject m_radiusEffect;



        protected override void Init()
        {
            m_subData = m_skillData as FoodPowerGSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }
            if (Caster.TryGetComponent<Damageable>(out Damageable damageable))
                m_casterDamageable = damageable;
            m_collider = transform.AddComponent<SphereCollider>();
            m_collider.isTrigger = true;
            m_rigidbody = transform.AddComponent<Rigidbody>();
            m_rigidbody.isKinematic = true;

        }
        // 비활성화되면 리스너를 모두 제거한다.
        private void OnDisable()
        {
            foreach (var damageable in m_damageableDic.Values)
            {
                damageable.DieDamageableEvent.RemoveListener(SoulEater);
            }


        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            m_damageTimer = 0;
            m_collider.radius = m_levelData.Radius;
            m_radiusEffect.transform.localScale = Vector3.one * m_levelData.Radius;

            StartCoroutine(SkillRoutine());
        }
        protected override void UpdateBehavior()
        {
            base.UpdateBehavior();

            if (m_levelData.GetAdditionalValue(0) < m_damageTimer)
            {
                Attack();
            }

            m_damageTimer += Time.deltaTime;

        }

        private void Attack()
        {
            m_damageTimer = 0;

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

        public void SoulEater(Damageable damageable)
        {
            float rand = Random.Range(0, 100);

            RemoveDamageable(damageable);

            if (m_levelData.GetAdditionalValue(1) < rand)
                return;

            if (m_casterDamageable != null)
                m_casterDamageable.RestoreHealth(m_levelData.GetAdditionalValue(2));
        }



        public override void InactiveSkill()
        {
            base.InactiveSkill();


            Destroy(gameObject);

        }


        private void AddDamageable(Damageable damageable)
        {
            int key = damageable.gameObject.GetInstanceID();

            if (damageable.IsDie)
                return;

            if (m_damageableDic.ContainsKey(key))
                return;

            m_damageableDic.Add(key, damageable);
            damageable.DieDamageableEvent.AddListener(SoulEater);
        }
        private void RemoveDamageable(Damageable damageable)
        {
            int key = damageable.gameObject.GetInstanceID();
            if (m_damageableDic.ContainsKey(key) == false)
                return;
            damageable.DieDamageableEvent.RemoveListener(SoulEater);
            m_damageableDic.Remove(key);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger || IsActive == false)
                return;

            if (other.CompareTag(m_subData.Target.ToString()))
            {
                if (other.TryGetComponent<Damageable>(out Damageable damageable))
                    AddDamageable(damageable);
            }

        }




        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(m_subData.Target.ToString()))
            {
                if (other.TryGetComponent<Damageable>(out Damageable damageable))
                    RemoveDamageable(damageable);
            }

        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
            //   Gizmos.DrawSphere(transform.position + transform.GetChild(0).forward * m_attackOffset, m_attackRadius);
        }
    }
}
