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
        private Damageable m_casterDamageable;
        private Rigidbody m_rigidbody;
        private SphereCollider m_collider;
        [SerializeField] private SerializableDictionary<int, Damageable> m_damageableDic = new SerializableDictionary<int, Damageable>();


        [Header("영혼 포식자 스킬")]
        [SerializeField] private GameObject m_radiusEffect;

        float m_damageTimer;

        protected override void AwakeInit()
        {
            m_collider = transform.AddComponent<SphereCollider>();
            m_rigidbody = transform.AddComponent<Rigidbody>();
        }

        protected override void Init()
        {
            if (Caster.Transform.TryGetComponent<Damageable>(out Damageable damageable))
                m_casterDamageable = damageable;

            m_collider.isTrigger = true;
            m_rigidbody.isKinematic = true;
        }

        // 비활성화되면 리스너를 모두 제거한다.
        public override void RemoveSkill()
        {
            StopEffect();
            base.RemoveSkill();
            foreach (var damageable in m_damageableDic.Values)
            {
                damageable.DieDamageableEvent.RemoveListener(SoulEater);
            }
        }

        protected override void UpdateBehavior()
        {
            base.UpdateBehavior();
            if (m_damageTimer <= 0 && IsActive)
            {
                m_damageTimer = LevelData.TryGetValue1();
                Attack();
            }


            m_damageTimer = m_damageTimer - Time.deltaTime <= 0 ? 0 : m_damageTimer -= Time.deltaTime;
        }

        private void Attack()
        {
            // 캐스터 타겟이면 사용하지 않는다.
            if (Data.SkillTarget == TargetTag.Caster)
                return;

            Collider[] colls = Physics.OverlapSphere(transform.position, LevelData.Radius, Data.TargetLayer, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < colls.Length; i++)
            {


                if (colls[i].CompareTag(Data.SkillTarget.ToString()))
                {
                    // 180도만 제한한다.
                    if (GFunc.TargetAngleCheck(transform, colls[i].transform, LevelData.Arc) == false)
                        continue;


                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        Debug.Log("데미지 보낸다" + LevelData.Damage);
                        damageable.OnDamage(LevelData.Damage);
                    }


                    // 버프도 같이 보낸다.
                    OnBuff(colls[i].transform);
                    RemoveBuff(colls[i].transform);
                }
            }

           
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();

            m_collider.radius = LevelData.Radius;
            m_radiusEffect.transform.localScale = Vector3.one * LevelData.Radius;

            // 한 투사체만 생성
            ActiveProjectiles();

            PlayEffect();


            if (m_data.SkillTarget == TargetTag.Caster)
                OnBuff(Caster.Transform);
        }

        public override void InactiveSkill()
        {
            // 투사체를 리셋
            ResetProjectiles();

            if (m_data.SkillTarget == TargetTag.Caster)
                RemoveBuff(Caster.Transform);

            StopEffect();
            base.InactiveSkill();
        }
        public void SoulEater(Damageable damageable)
        {
            float rand = Random.Range(0, 100);

            RemoveDamageable(damageable);

            if (LevelData.TryGetValue1(1) < rand)
                return;

            if (m_casterDamageable != null)
                m_casterDamageable.RestoreHealth(LevelData.TryGetValue1(2));
        }

        #region Damageable
        private void AddDamageable(Damageable damageable)
        {
            if (damageable.IsDie)
                return;

            int key = damageable.gameObject.GetInstanceID();

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
        #endregion Damageable

        private void OnTriggerStay(Collider other)
        {
            if (other.isTrigger || IsActive == false)
                return;

            if (other.CompareTag(Data.SkillTarget.ToString()))
            {
                if (other.TryGetComponent<Damageable>(out Damageable damageable))
                    AddDamageable(damageable);
            }

        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Data.SkillTarget.ToString()))
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
