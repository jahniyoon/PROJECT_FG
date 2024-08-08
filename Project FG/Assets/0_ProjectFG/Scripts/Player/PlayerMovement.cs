using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace JH
{
    public class PlayerMovement : MonoBehaviour
    {
        private PlayerController m_controller;
        private Rigidbody m_rigid;
        private Transform m_model;

        [SerializeField] LayerMask m_wallMask;
        [SerializeField] float m_wallDistance;

        Vector3 m_velocity;

        private void Awake()
        {
            m_controller = GetComponent<PlayerController>();
            m_rigid = GetComponent<Rigidbody>();
            m_model = transform.GetChild(0);
        }


        // 플레이어 이동 함수
        public void Movement(Vector3 velocity)
        {
            Vector3 newVelocity = m_rigid.position + velocity;

            Ray ray = new Ray(m_rigid.position, velocity.normalized);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, m_wallDistance, m_wallMask))
            {
                Debug.Log(hit.collider.name);
                newVelocity = m_rigid.position;
            }
            

            


            m_rigid.MovePosition(newVelocity);
        }

        // 플레이어가 보는 방향
        public void LookRotation(Vector3 rotation)
        {
            if (Vector3.zero == rotation)
            {
                return;
            }
            // 목표 회전 값
            Quaternion targetRotation = Quaternion.LookRotation(rotation);

            // 현재 회전에서 목표 회전까지의 보간
            Quaternion newRotation = Quaternion.Lerp(m_model.localRotation, targetRotation, m_controller.Setting.PlayerRotateSpeed * Time.deltaTime);

            // 보간된 회전을 적용
            m_model.localRotation = newRotation;
        }
        public void LookAt(Vector3 position)
        {
            Vector3 lookPos = position;
            lookPos.y = m_model.position.y;

            m_model.LookAt(lookPos);
        }


        private void OnDrawGizmosSelected()
        {
            Transform position = transform;

            if (m_model)
                position = m_model;

            Debug.DrawRay(position.position, position.forward, Color.red);
        }
    }
}