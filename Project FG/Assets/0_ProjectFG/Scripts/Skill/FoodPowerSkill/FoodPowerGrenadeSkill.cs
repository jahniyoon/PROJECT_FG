using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.VFX;
using UnityEngine.SocialPlatforms;

namespace JH
{
    public class FoodPowerGrenadeSkill : FoodPowerSkill
    {
        private FoodPowerDSkillData m_subData;
        private float m_offset = 0.5f;
        public GameObject m_range;
        [Header("거리 랜덤")]
        [SerializeField] private bool m_isRandomRange = false;
        

        protected override void Init()
        {
            m_subData = m_data as FoodPowerDSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

        }

        public override void LeagcyActiveSkill()
        {
            base.LeagcyActiveSkill();
            Shoot();
            m_range.transform.localScale = Vector3.one * m_levelData.Range;
            StartCoroutine(ActiveSkillRoutine());
        }

        public void Shoot()
        {
            Transform parent = GameManager.Instance.ProjectileParent;

            for (int i = 0; i < m_levelData.Count; i++)
            {
                ShootProjectile(m_subData.GrenadePrefab, i + 1);
            }

        }

        // 탄 생성 및 발사
        private void ShootProjectile(GameObject projectile, int index = 0)
        {
            // 발사되는 위치
            Vector3 position = m_casterPosition.position;
            position.y += m_offset;

            Quaternion rotation = GFunc.GetQuaternion(m_aimtype, m_casterPosition);
            Transform parent = GameManager.Instance.ProjectileParent;



            var cloneProjectile = Instantiate(projectile, position, rotation, parent).GetComponent<DefaultProjectile>();
            cloneProjectile.SetSkill((SkillBase)this);

            Vector3 targetPosition = cloneProjectile.transform.position + cloneProjectile.transform.forward * m_levelData.Range;

            if (m_isRandomRange)
            {
                targetPosition = cloneProjectile.transform.position + cloneProjectile.transform.forward * Random.Range(0, m_levelData.Range);
            }


        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();
            Destroy(gameObject);

        }


    }
}
