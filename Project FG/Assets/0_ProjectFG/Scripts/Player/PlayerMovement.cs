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
        [SerializeField] SpriteRenderer[] m_2DModel;

        Vector3 m_velocity;
        [Header("Debuff")]

        [SerializeField] private float m_slowDebuff = 0;

        private void Awake()
        {
            m_controller = GetComponent<PlayerController>();
            m_rigid = GetComponent<Rigidbody>();
            m_model = transform.GetChild(0);
            m_2DModel = GetComponentsInChildren<SpriteRenderer>();
        }

        Vector3 m_lookDir;

        private void FixedUpdate()
        {
            if (m_controller.State == FSMState.Move)
            {
                MovementBehavior();
            }
        }

        public void MovementBehavior()
        {
            m_lookDir.x = m_model.forward.x;
            m_lookDir.z = m_model.forward.z;

            if (m_controller.Input.Move != Vector2.zero)
            {
                m_lookDir.x = m_controller.Input.Move.x;
                m_lookDir.z = m_controller.Input.Move.y;
            }


            // 카메라의 방향과 이동 방향을 곱하여 플레이어의 이동 속도 벡터 계산
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0f; // 수평 방향 벡터로 설정

            Vector3 moveDirWorld = Quaternion.FromToRotation(Vector3.forward, cameraForward) * m_lookDir;

            // 방향에 맞게 이동시킨다.
            Vector3 velocity = moveDirWorld * (FinalSpeed(m_controller.Setting.PlayerMoveSpeed) * Time.deltaTime);


            Movement(velocity);
            LookRotation(moveDirWorld);
        }


        // 플레이어 이동 함수
        public void Movement(Vector3 velocity)
        {
            Vector3 newVelocity = m_rigid.position + velocity;

            Ray ray = new Ray(m_rigid.position, velocity.normalized);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, m_wallDistance, m_wallMask))
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
            {
                bool flip = 180 >= newRotation.eulerAngles.y;
                foreach (var renderer in m_2DModel)
                    renderer.flipX = flip;
            }


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
        public void SetMoveSpeed(float value)
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