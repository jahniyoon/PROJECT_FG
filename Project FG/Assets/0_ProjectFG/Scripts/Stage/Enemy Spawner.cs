using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class EnemySpawner : MonoBehaviour
    {
        private Transform enemyParent;


        [Header("Enemy Prefab")]
        [SerializeField] private GameObject[] m_enemy;

        [Header("Spawn Info")]
        [SerializeField] private Vector2 m_spawnArea;
        [SerializeField] private int m_spawnCount = 0;


        private void Awake()
        {
            enemyParent = new GameObject("Enemy Parent").transform;
            enemyParent.parent = transform;
        }

        public void SetArea(Vector2 size)
        {
            m_spawnArea = size * 0.5f;
        }

        public void SpawnEnemy(int count)
        {
            m_spawnCount++;

            UIManager.Instance.Debug.WaveText(m_spawnCount);

            for(int i = 0; i < count;  i++)
            {
                GameObject enemy = Instantiate(m_enemy[0], enemyParent);

                Vector3 spawnPos = Vector3.zero;

                spawnPos.x = Random.Range(m_spawnArea.x * -1, m_spawnArea.x);
                spawnPos.z = Random.Range(m_spawnArea.y * -1, m_spawnArea.y);

                enemy.transform.localPosition = spawnPos;
            }

        }
    }
}