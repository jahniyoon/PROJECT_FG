using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

namespace JH
{
    public partial class AimAndShootEnemy : EnemyController, IAimSkillCaster
    {
        [Header("Aim And Shoot")]
        [SerializeField] private AimState m_aimState;
        [SerializeField] private bool m_isAim;
        [SerializeField] private float m_aimTimer;
        [SerializeField] private float m_shootingTimer;

        SkillBase m_aimSkill;

        public AimState AimState => m_aimState;
        protected override void StartInit()
        {
            base.StartInit();
            m_aimSkill = TryGetSkill(0);
        }



        public override bool CanActiveSkill()
        {
            if (m_state != FSMState.Attack)
                return false;

            // 조준중일때 스킬을 사용할 수 없다.
            if (m_isAim)
                return false;

            return true;
        }


        private void SetAim(bool enable)
        {
            m_isAim = enable;
        }
        private void ResetTimer()
        {
            m_aimTimer = 0;
            m_shootingTimer = 0;
        }
   
        // 공격 상태를 체크
        private bool AttackAimCheck()
        {

            if (m_aimSkill == null)
            {
                Debug.Log(gameObject.name + " 스킬을 다시 확인해주세요.");
                return false;
            }

            if (m_aimState == AimState.Reload)
                return false;

            if (m_isAim == false && m_data.ChaseRange < m_targetDistance)
            {
                return false;
            }


            // 조준이 완료되고, 타겟이 공격범위 밖으로 나가면 다시 이동상태
            if (m_isAim && m_data.ChaseRange < m_targetDistance)
            {
                return false;
            }

            // 조준 완료상태에서타겟의 각도가 스킬 범위 밖이면 이동상태
            if (m_isAim && m_aimSkill.Data.LevelData.Arc * 0.5f < Mathf.Abs(TargetAngle()))
            {
                return false;
            }


            return true;
        }


    }
}