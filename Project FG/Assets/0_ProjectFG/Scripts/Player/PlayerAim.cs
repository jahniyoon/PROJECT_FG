using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class PlayerAim : MonoBehaviour
    {
        private PlayerController m_player;
        private Camera m_mainCam;

        [SerializeField] private Transform m_aim;
        [SerializeField] LayerMask m_floorLayer;


        private void Awake()
        {
            m_player = GetComponent<PlayerController>();
            m_mainCam = Camera.main;
        }

        private void FixedUpdate()
        {
            AimUpdate();
        }

        private void AimUpdate()
        {
            if (m_mainCam == null)
                return;

            RaycastHit hit;
            Ray ray = m_mainCam.ScreenPointToRay(m_player.Input.AimPoint);

            if (Physics.Raycast(ray, out hit, 100, m_floorLayer, QueryTriggerInteraction.Ignore))
            {
                m_aim.position = hit.point;
            }
        }


        public Vector3 GetPoint()
        {
            if (m_aim == null)
            { 
                return Vector3.zero;
            }

            return m_aim.position; 
        }
    }
}