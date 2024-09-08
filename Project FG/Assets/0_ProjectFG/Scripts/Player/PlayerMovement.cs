using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

namespace JH
{
    public class PlayerMovement : MonoBehaviour, ISlowable
    {
        private PlayerController m_controller;
        private Rigidbody m_rigid;
        private Transform m_model;
        [SerializeField] LayerMask m_wallMask;
        [SerializeField] float m_wallDistance;
        [Header("2D Setting")]
        [SerializeField] SpriteRenderer m_2DModel;

        Vector3 m_velocity;
        [Header("Debuff")]

        [SerializeField] private float m_slowDebuff = 0;

        private void Awake()
        {
            m_controller = GetComponent<PlayerController>();
            m_rigid = GetComponent<Rigidbody>();
            m_model = transform.GetChild(0);
            m_2DModel = GetComponentInChildren<SpriteRenderer>();
        }


        // 플레이어 이동 함수
        public void Movement(Vector3 velocity)
        {
            Vector3 newVelocity = m_rigid.position + velocity;

            Ray ray = new Ray(m_rigid.position, velocity.normalized);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, m_wallDistance, m_wallMask))
            {
                newVelocity = m_rigid.position;
            }
            m_rigid.MovePosition(newVelocity);
        }

        public void MovePosition(Vector3 position)
        {
            m_rigid.MovePosition(position);
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

            // 2D 모델이면 플립되기만 하면 된다.
            if (m_2DModel != null)
                m_2DModel.flipX = 180 < newRotation.eulerAngles.y ;


            // 보간된 회전을 적용
            m_model.localRotation = newRotation;
        }
        public void LookAt(Vector3 position)
        {
            Vector3 lookPos = position;
            lookPos.y = m_model.position.y;

            m_model.LookAt(lookPos);
        }

        public float FinalSpeed(float curSpeed)
        {
            return curSpeed * (100 - m_slowDebuff) * 0.01f;
        }
        public void SetSlowSpeed(float value)
        {
            m_slowDebuff += value;
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