using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JH
{
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private Vector2 m_move;
        [SerializeField] private Vector2 m_aimPoint;
        [SerializeField] private bool m_attack;
        [SerializeField] private bool m_attackDown;
        [SerializeField] private bool m_predation;
        [SerializeField] private bool m_predationDown;
        public Vector2 Move => m_move;
        public Vector2 AimPoint => m_aimPoint;
        public bool Attack => m_attack;
        public bool AttackDown => m_attackDown;
        public bool Predation => m_predation;
        public bool PredationDown => m_predationDown;

        private WaitForEndOfFrame WaitForEndOfFrame = new WaitForEndOfFrame();

        // ####################### Input System ############################

        // 플레이어 이동 입력
        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnAim(InputValue value)
        {
            AimInput(value.Get<Vector2>());
        }


        public void OnAttack(InputValue value)
        {
            AttackInput(value.Get<float>());
        }
        public void OnPredation(InputValue value)
        {
            PredationInput(value.Get<float>());
        }


        // ######################### 입력 변환 ###############################

        private void MoveInput(Vector2 input)
        {
            m_move = input;
        }

        private void AimInput(Vector2 input)
        {
            m_aimPoint = input;
        }

        /// <param Name="input"> 0 : 입력 대기 / 1 : 입력 </param>
        private void AttackInput(float input)
        {
            if (m_attack == false && input == 1)
            {
                StartCoroutine(AttackDownRoutine());
            }
            m_attack = input != 0;
        }
        IEnumerator AttackDownRoutine()
        {
            m_attackDown = true;
            yield return WaitForEndOfFrame;
            m_attackDown = false;
        }
        /// <param Name="input"> 0 : 입력 대기 / 1 : 입력 </param>
        private void PredationInput(float input)
        {
            if (m_predation == false && input == 1)
            {
                StartCoroutine(PredationDownRoutine());
            }
            m_predation = input != 0;
        }
        IEnumerator PredationDownRoutine()
        {
            m_predationDown = true;
            yield return WaitForEndOfFrame;
            m_predationDown = false;
        }
    }
}