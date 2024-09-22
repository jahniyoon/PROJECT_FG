using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JH
{


    public class WaveManager : MonoBehaviour
    {
        private StageCreator m_stageCreator;
        private EnemySpawner m_enemySpawner;

        [Header("레벨 사이즈")]
        [SerializeField] private Vector2 m_stageSize;

        [Header("웨이브")]
        [SerializeField] private List<LevelWave> m_waves;
        [SerializeField] bool m_spawnEnable = true;

        [Header("웨이브 상황판")]
        [SerializeField] private int m_curWave;
        [SerializeField] private int m_spawnCount;
        [SerializeField] private int m_remainEnemy;
        [SerializeField] private int m_baconSpawnCount;


        [Header("베이컨")]
        [SerializeField] private GameObject m_baconPrefab;
        [SerializeField] private float m_baconMinDistance = 5;
        private Transform m_enemyParent;
        private List<Vector3> m_spawnBaconPos = new List<Vector3>();



        private void Awake()
        {
            m_stageCreator = GetComponentInChildren<StageCreator>();
            m_enemySpawner = GetComponentInChildren<EnemySpawner>();

            m_enemyParent = new GameObject("Enemy Parent").transform;

            CreateStage();
        }
        private void Start()
        {
            WaveStart();
        }

        // 스테이지 생성
        public void CreateStage()
        {
            Vector2 stageSize = new Vector2(m_stageSize.x, m_stageSize.y);

            if (m_stageCreator == null)
                m_stageCreator = GetComponentInChildren<StageCreator>();

            m_stageCreator.CreateStage(stageSize);
            if (m_enemySpawner)
                m_enemySpawner.SetArea(stageSize);

            UIManager.Instance.MinimapUI.SetAreaSize(stageSize);
        }

        public void SwitchSpawnEnable()
        {
            m_spawnEnable = !m_spawnEnable;
        }



        Coroutine waveRoutine;


        // 웨이브 시작
        public void WaveStart(int value = 1)
        {
            m_curWave = value;
            if (waveRoutine != null)
            {
                StopCoroutine(waveRoutine);
                waveRoutine = null;
            }
            UIManager.Instance.WaveUI.SetWave(m_curWave, true);

            waveRoutine = StartCoroutine(WaveRoutine(m_waves[value - 1]));
        }

        // 웨이브 루틴
        IEnumerator WaveRoutine(LevelWave Wave)
        {
            m_spawnCount = 0;
            m_baconSpawnCount = 0;
            m_spawnBaconPos.Clear();

            SpawnEnemy(Wave.SpawnEnemy);

            float timer = 0;
            float baconTimer = 0;

            while (0 < m_remainEnemy)
            {
                // 타이머가 되면 스폰한다.
                if (m_spawnCount < Wave.EnemyCreationCount)
                {
                    if (m_spawnEnable)
                    {
                        if (Wave.EnemyCreationTime <= timer || m_remainEnemy < Wave.EnemyLeftNumber)
                        {
                            SpawnEnemy(Wave.SpawnEnemy);
                            timer = 0;
                        }
                    }
                }


                // 베이컨 생성 가능한지 체크
                if (Wave.BaconSpawnDuration <= baconTimer && m_baconSpawnCount < Wave.BaconMaxValue)
                {
                    if (Random.Range(0, 100) < Wave.BaconCreateProbability)
                        SpawnBacon();

                    baconTimer = 0;
                }


                timer += Time.deltaTime;
                baconTimer += Time.deltaTime;
                yield return null;
            }

            WaveEnd();
            yield break;
        }

        private void SpawnEnemy(GameObject EnemyPrefab)
        {
            GameObject Enemy = Instantiate(EnemyPrefab, m_enemyParent);
            EnemyController[] Enemies = Enemy.GetComponentsInChildren<EnemyController>();

            foreach (var enemy in Enemies)
            {
                if (enemy.NotCount)
                {
                    continue;
                }
                enemy.Damageable.DieEvent.AddListener(EnemyDie);
                m_remainEnemy++;
            }
            m_spawnCount++;
            UIManager.Instance.WaveUI.SetRemainEnemy(m_remainEnemy);

        }

        private void SpawnBacon()
        {
            GameObject Bacon = Instantiate(m_baconPrefab, m_enemyParent);

            // 최소거리 구하는 알고리즘 필요
            Vector3 spawnPos = Vector3.zero;
            Vector2 area = m_stageSize * 0.4f;
            spawnPos.x = Random.Range(area.x * -1, area.x);
            spawnPos.z = Random.Range(area.y * -1, area.y);

            m_spawnBaconPos.Add(spawnPos);

            Bacon.transform.localPosition = spawnPos;

            m_baconSpawnCount++;
        }

        public void EnemyDie()
        {
            m_remainEnemy--;
            UIManager.Instance.WaveUI.SetRemainEnemy(m_remainEnemy);
        }


        // 웨이브 종료
        public void WaveEnd()
        {
            EnemyController[] Enemies = m_enemyParent.GetComponentsInChildren<EnemyController>();
            foreach (EnemyController enemy in Enemies)
            {
                enemy.KillEnemy();
            }


            if (m_waves.Count <= m_curWave)
            {
                Clear();
                return;
            }
            UIManager.Instance.WaveUI.SetRemainEnemy(0, false);
            UIManager.Instance.WaveUI.SetWave(m_curWave, false);


            UIManager.Instance.WaveUI.NextWave(true);
        }
        public void NextWave()
        {
            UIManager.Instance.WaveUI.NextWave(false);

            m_curWave++;
            WaveStart(m_curWave);
        }

        public void Clear()
        {
            UIManager.Instance.SetGameClearUI(true);
        }

    }
    [System.Serializable]
    public class LevelWave
    {
        public GameObject SpawnEnemy;
        [Header("에네미")]
        public float EnemyCreationCount = 4;
        public float EnemyCreationTime;
        public float EnemyLeftNumber;

        [Header("베이컨")]
        public float BaconSpawnDuration;
        [Range(0, 100)]
        public float BaconCreateProbability;
        public int BaconMaxValue;
    }
}