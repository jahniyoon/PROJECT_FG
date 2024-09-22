using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager m_instance; // 싱글톤이 할당될 static 변수
        public static UIManager Instance
        {
            get
            {
                // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
                if (m_instance == null)
                {
                    // // 생성 후 할당
                    GameObject obj = new GameObject("UI Manager");
                    m_instance = obj.AddComponent<UIManager>();
                }

                // 싱글톤 오브젝트를 반환
                return m_instance;
            }
        }

        [Header("Main UI")]
        [SerializeField] private MainUIController m_mainUIController;

        [Header("Wave UI")]
        [SerializeField] private WaveUIController m_waveUIController;

        [Header("Game UI")]
        [SerializeField] private GameObject m_pauseUI;
        [SerializeField] private GameObject m_gameOverUI;
        [SerializeField] private GameObject m_gameClearUI;
        [SerializeField] private MinimapUI m_minimapUI;


        [Header("Debug UI")]
        [SerializeField] private bool m_debugEnable;
        [SerializeField] private DebugUIController m_DebugUI;

        public MainUIController MainUI => m_mainUIController;
        public WaveUIController WaveUI => m_waveUIController;
        public DebugUIController Debug => m_DebugUI;
        public MinimapUI MinimapUI => m_minimapUI;

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

            m_DebugUI.DebugEnable(m_debugEnable);

        }

        public void SetPauseUI(bool enable)
        {
            m_pauseUI.SetActive(enable);
        }
        public void SetGameOverUI(bool enable)
        {
            m_gameOverUI.SetActive(enable);
        }
        public void SetGameClearUI(bool enable)
        {
            m_gameClearUI.SetActive(enable);
        }


    }
}