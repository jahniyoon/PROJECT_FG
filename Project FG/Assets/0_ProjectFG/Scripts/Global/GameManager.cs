using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;


namespace JH
{
    public class GameManager : MonoBehaviour
    {
        private static GameManager m_instance; // 싱글톤이 할당될 static 변수
        public static GameManager Instance
        {
            get
            {
                // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
                if (m_instance == null)
                {
                    // // 생성 후 할당
                    GameObject obj = new GameObject("GameManger");
                    m_instance = obj.AddComponent<GameManager>();
                }

                // 싱글톤 오브젝트를 반환
                return m_instance;
            }
        }

        private void Awake()
        {
            // 싱글톤 인스턴스 초기화
            if (m_instance == null)
            {
                m_instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            Time.timeScale = 1;

            m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
            m_aim = GameObject.FindGameObjectWithTag("Aim").transform;
            m_levelManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();

            GameObject projectile = new GameObject("Projectile Parent");
            m_projectileParent = projectile.transform;
        }

        private LevelManager m_levelManager;
        [SerializeField] private PlayerController m_playerController;
        [SerializeField] private bool m_isGameOver = false;
        [SerializeField] private string m_gameoverSFX;
        private bool isPause;
        private bool m_quit;
        private bool m_isDebug;

        #region Property
        public bool Quit => m_quit;

        private Transform m_projectileParent;

        private Transform m_aim;

        public Transform ProjectileParent => m_projectileParent;


        public PlayerController PC => m_playerController;

        public bool IsGameOver => m_isGameOver;

        public Transform Aim => m_aim;
        #endregion

        private void Update()
        {
            DebugHotKey();
        }

        private void DebugHotKey()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                PC.GodMode();
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                m_levelManager.SwitchSpawnEnable();
            }
            if (Input.GetKeyDown(KeyCode.F3))
            {
                PC.GetDefaultFoodPower();
            }
            if (Input.GetKeyDown(KeyCode.F4))
            {
                PC.GetDebugFoodPower();
            }
            if (Input.GetKeyDown(KeyCode.F5))
            {
                DebugMode();
            }
            //if (Input.GetKeyDown(KeyCode.R))
            //{
            //    ResetScene();
            //}

            if (Input.GetKeyDown(KeyCode.F10))
            {
                Time.timeScale += 1;
            }
            if (Input.GetKeyDown(KeyCode.F11))
            {
                Time.timeScale = 0.1f;
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                Resume();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Pause();
            }

        }

        private void DebugMode()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach(var enemy in enemies)
            {
                if (enemy.TryGetComponent<EnemyController>(out EnemyController enemyController))
                    enemyController.DebugEnable(!m_isDebug);
            }
            m_isDebug = !m_isDebug;
        }

        private void Pause()
        {
            float timescale = 1;

            isPause = !isPause;

            if (isPause)
                timescale = 0;

            Time.timeScale = timescale;
            UIManager.Instance.SetPauseUI(isPause);
        }
        public void Resume()
        {
            Time.timeScale = 1;
            isPause = false;
            UIManager.Instance.SetPauseUI(isPause);
        }

        // 게임 오버 함수
        public void GameOver()
        {
            m_isGameOver = true;
            UIManager.Instance.SetGameOverUI(m_isGameOver);
            AudioManager.Instance.StopBGM();
            AudioManager.Instance.PlaySFX(m_gameoverSFX);
        }

        public void TitleScene()
        {
            SceneManager.LoadScene("Title Scene");


        }
        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

        }

        public void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        void OnApplicationQuit()
        {
            m_quit = true;
        }

    }
}