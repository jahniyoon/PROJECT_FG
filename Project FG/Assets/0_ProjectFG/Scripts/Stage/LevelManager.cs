using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class LevelManager : MonoBehaviour
    {
        [SerializeField] GameSettings m_gameSettings;
        [SerializeField] bool m_spawnEnable = true;
        private StageCreator m_stageCreator;
        private EnemySpawner m_enemySpawner;

        private void Awake()
        {
            m_stageCreator = GetComponentInChildren<StageCreator>();
            m_enemySpawner = GetComponentInChildren<EnemySpawner>();

            CreateStage();

        }
        private void Start()
        {
            StartCoroutine(SpawnRoutine());
        }


        public void CreateStage()
        {
            Vector2 stageSize = new Vector2(m_gameSettings.StageWidth, m_gameSettings.StageLength);
            
            if(m_stageCreator == null)
                m_stageCreator = GetComponentInChildren<StageCreator>();

            m_stageCreator.CreateStage(stageSize);
            if(m_enemySpawner)
            m_enemySpawner.SetArea(stageSize);

        }

        public void SwitchSpawnEnable()
        {
            m_spawnEnable = !m_spawnEnable;
        }

        // 에네미 스폰 루틴
        IEnumerator SpawnRoutine()
        {
            yield return new WaitForSeconds(m_gameSettings.StartEnemySpawnTime);

            float timer = m_gameSettings.EnemySpawnInterval;

            while (GameManager.Instance.IsGameOver == false)
            {            
                // 시간 간격이 지나면 에네미 스폰 후 타이머 리셋
                if(m_gameSettings.EnemySpawnInterval < timer && m_spawnEnable || Input.GetKeyDown(KeyCode.N))
                {
                    m_enemySpawner.SpawnEnemy(m_gameSettings.EnemySpawnCount);
                    timer = 0;
                }

                timer += Time.deltaTime;

                yield return null;
            }

            yield break;
        }

    }
}