using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


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

            m_playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();


            GameObject projectile = new GameObject("Projectile Parent");
            m_projectileParent = projectile.transform;
        }


        [SerializeField ] private PlayerController m_playerController;
        [SerializeField] private bool m_isGameOver = false;

        private Transform m_projectileParent;


        public Transform ProjectileParent => m_projectileParent;


        public PlayerController PC => m_playerController;

        public bool IsGameOver => m_isGameOver;



        private void Update()
        {
            DebugHotKey();
        }

        private void DebugHotKey()
        {
            if(Input.GetKeyDown(KeyCode.F1))
            {
                PC.GodMode();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ResetScene();
            }

            if (Input.GetKeyDown(KeyCode.F11))
            {
                Time.timeScale = 0.1f;
            }

            if (Input.GetKeyDown(KeyCode.F12))
            {
                Time.timeScale = 1f;
            }

        }

        // 게임 오버 함수
        public void GameOver()
        {
            m_isGameOver = true;
            Invoke(nameof(ResetScene), PC.Setting.ResetDuration);
        }

        private void ResetScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

    }
}