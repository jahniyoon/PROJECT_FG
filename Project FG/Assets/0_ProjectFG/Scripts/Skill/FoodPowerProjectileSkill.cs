using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerProjectileSkill : FoodPowerSkill
    {
        private ProjectileSkillData m_subData;
        [Header("발사체 스킬")]
        [SerializeField] private float m_offset = 0.5f;
        [SerializeField] private float m_angleIncrement = 10f;

        protected override void Init()
        {
            m_subData = m_skillData as ProjectileSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            Shoot();

            StartCoroutine(SkillRoutine());
        }

        public void Shoot()
        {
            Transform parent = GameManager.Instance.ProjectileParent;

            Vector3 position = m_casterPosition.position;
            position.y += m_offset;

            for (int i = 0; i < m_levelData.Count; i++)
            {
                ShootProjectile(m_subData.ProjectilePrefab, i + 1);
            } 

        }

        // 탄 생성 및 발사
        private void ShootProjectile(GameObject projectile, int index = 0)
        {
            // 발사되는 위치
            Vector3 shootPos = transform.position;

            Quaternion rotation = GetProjectileDirection(transform, index);
            Transform parent = GameManager.Instance.ProjectileParent;

            var Projectile = Instantiate(projectile, shootPos, rotation, parent).GetComponent<Projectile>();
            Projectile.ProjectileInit(m_levelData.Damage, m_levelData.GetValue(0), Mathf.FloorToInt(m_levelData.GetValue(1)), m_levelData.Duration, m_subData.Target);
        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();
            Destroy(gameObject);

        }


        private Quaternion GetProjectileDirection(Transform t, int index)
        {
            // 첫번째가 기본공격이기 때문에 빼준다.
            index--;

            Quaternion direction = t.rotation;

            if (index == 0)
                return direction;

            // 홀수짝수 구분
            int n = (index + 1) / 2;
            // 홀수짝수에 따라 각도 크기 정하기
            float angle = n * m_angleIncrement;

            // 홀수면 음수
            if (index % 2 == 1)
                angle = angle * -1;

            direction = Quaternion.Euler(0, t.transform.eulerAngles.y + angle, 0);
            //Debug.Log($"Index : {index}, Angle : {angle} , Direction {direction.eulerAngles}");

            return direction;
        }
    }
}
