using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    [System.Serializable]

    public class TestFoodPower : FoodPower
    {
        [Header("Test Projectile")]
        [SerializeField] private GameObject m_projectilePrefab;
        [SerializeField] private float m_offset = 0.5f;


        public override void Active()
        {
            Transform parent = GameManager.Instance.ProjectileParent;

            Vector3 position = m_casterPosition.position;
            position.y += m_offset;

            Quaternion direction = GetDirection();

            // 방향이 없으면 발사하지 않는다.
            if (direction == Quaternion.identity)
                return;

            GameObject projectile = Instantiate(m_projectilePrefab, position, direction, parent);
            //projectile.GetComponent<Projectile>().ProjectileInit();
        }

    }
}