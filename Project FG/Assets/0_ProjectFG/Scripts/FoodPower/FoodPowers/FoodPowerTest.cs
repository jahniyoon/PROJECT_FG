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

            Vector3 position = m_player.position;
            position.y += m_offset;

            GameObject projectile = Instantiate(m_projectilePrefab, position, m_player.rotation, parent);

            
        }
    }
}