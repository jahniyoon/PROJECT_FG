using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public class DebugSpawner : MonoBehaviour
	{

        [SerializeField] GameSettings m_gameSettings;
        [SerializeField] private int m_spawnCount = 1;
            
        Transform enemyParent;
        private Vector2 m_spawnArea;
        public GameObject[] m_enemy; 

        private void Awake()
        {
            m_spawnArea = new Vector2(m_gameSettings.StageWidth, m_gameSettings.StageLength) * 0.4f;
        }
        private void Start()
        {
            GameObject enemyParentObj = GameObject.Find("Enemy Parent");
            if (enemyParentObj == null)
                enemyParentObj = new GameObject("Enemy Parent");
            enemyParent = enemyParentObj.transform;

        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Keypad0))
                SpawnEnemy(0);
            if (Input.GetKeyDown(KeyCode.Keypad1))
                SpawnEnemy(1);
            if (Input.GetKeyDown(KeyCode.Keypad2))
                SpawnEnemy(2);
            if (Input.GetKeyDown(KeyCode.Keypad3))
                SpawnEnemy(3);
            if (Input.GetKeyDown(KeyCode.Keypad4))
                SpawnEnemy(4);
            if (Input.GetKeyDown(KeyCode.Keypad5))
                SpawnEnemy(5);
            if (Input.GetKeyDown(KeyCode.Keypad6))
                SpawnEnemy(6);
            if (Input.GetKeyDown(KeyCode.Keypad7))
                SpawnEnemy(7);
            if (Input.GetKeyDown(KeyCode.Keypad8))
                SpawnEnemy(8);
        }


        public void SpawnEnemy(int value)
        {
            if (m_enemy.Length < value)
                return;

            for (int i = 0; i < m_spawnCount; i++)
            {
                GameObject enemy = Instantiate(m_enemy[value], enemyParent);

                Vector3 spawnPos = Vector3.zero;

                spawnPos.x = Random.Range(m_spawnArea.x * -1, m_spawnArea.x);
                spawnPos.z = Random.Range(m_spawnArea.y * -1, m_spawnArea.y);

                enemy.transform.localPosition = spawnPos;
            }

        }

    }
}
