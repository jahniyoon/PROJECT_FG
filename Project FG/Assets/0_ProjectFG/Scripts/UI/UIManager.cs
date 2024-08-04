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


        [Header("Debug UI")]
        [SerializeField] private bool m_debugEnable;
        [SerializeField] private DebugUIController m_DebugUI;

        public MainUIController MainUI => m_mainUIController;
        public DebugUIController Debug => m_DebugUI;

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

            if(m_DebugUI)
            {
                m_DebugUI.gameObject.SetActive(m_debugEnable);  
            }
        }


    }
}