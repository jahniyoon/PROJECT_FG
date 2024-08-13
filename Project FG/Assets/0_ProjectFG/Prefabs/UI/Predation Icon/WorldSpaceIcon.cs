using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class WorldSpaceIcon : MonoBehaviour
    {
        [Header ("Icon Setting")]
        [SerializeField] private GameSettings gameSettings;
        [SerializeField] private bool m_enable;
        Transform m_icon;
        private Camera _mainCam;

        private void Awake()
        {
            m_icon = transform.GetChild(0);
        }

        // Update is called once per frame
        void LateUpdate()
        {
            LookCamera();
        }

        public void IconEnable(bool enable)
        {
            m_enable = enable;
            m_icon.gameObject.SetActive(enable);
        }

        private void LookCamera()
        {
            if (m_enable == false)
                return;

            if(_mainCam == null)
            {
                _mainCam = Camera.main;
            }

            m_icon.localScale = Vector3.one * gameSettings.PredationIconScale;
            m_icon.rotation = Quaternion.LookRotation(transform.position - _mainCam.transform.position);

        }
    }
}
