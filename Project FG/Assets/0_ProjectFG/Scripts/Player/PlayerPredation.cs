using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PlayerPredation : MonoBehaviour
    {
        private PlayerController m_player;
        private PlayerAim m_aim;
        private PlayerHunger m_hunger;
        private Damageable m_damageable;

        [SerializeField] private Transform m_predationTarget;
        [SerializeField] private float m_predationCoolDown;

        Coroutine m_dashRoutine;
        public Transform PredationTarget => m_predationTarget;
        public bool IsPredation => m_predationTarget != null;

        private void Awake()
        {
            m_player = GetComponent<PlayerController>();
            m_aim = GetComponent<PlayerAim>();
            m_hunger = GetComponent<PlayerHunger>();
            m_damageable = GetComponent<Damageable>();
        }


        private void Update()
        {
            if (m_player.Input.PredationDown)
            {
                Predation();
            }

            if (0 < m_predationCoolDown)
            {
                m_predationCoolDown -= Time.deltaTime;
            }

        }

        // 가능한지 체크한다.
        private void Predation()
        {
            //  사망 상태 및 이미 포식 타겟이 있으면 체크하지 않음                    
            if (m_predationTarget != null || m_player.State == FSMState.Die || m_hunger.CantPredation)
                return;

            m_predationTarget = ScanTarget();
        }

        private Transform ScanTarget()
        {
            Transform target = null;


            // 1. 마우스 Aim 먼저 검사
            target = ScanPosition(m_aim.GetPoint(), m_player.Setting.PredationAimRange);

            if (target != null)
                return target;

            // 2. PC 주변 검사
            target = ScanPosition(transform.position, m_player.Setting.PredationPlayerRange);

            return target;
        }

        private Transform ScanPosition(Vector3 position, float radius)
        {
            Transform target = null;

            Collider[] colls = Physics.OverlapSphere(position, radius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag("Enemy"))
                {

                    if (colls[i].TryGetComponent<IPredationable>(out IPredationable predation))
                    {
                        if (predation.CanPredation == false)
                            continue;
                        target = DistanceChecker(position, target, predation.Transform);

                    }
                }

                //if (colls[i].CompareTag("Enemy"))
                //{
                //    EnemyController enemy = colls[i].GetComponent<EnemyController>();

                //    //  포식 가능 상태가 아니면 패스
                //    if (enemy.CanPredation == false)
                //        continue;

                //    target = DistanceChecker(position, target, enemy.transform);
                //}

            }

            return target;
        }

        private Transform DistanceChecker(Vector3 startPos, Transform curTarget, Transform newTarget)
        {
            if (curTarget == null)
                return newTarget;

            float curDistance = Vector3.Distance(startPos, curTarget.position);
            float newDistance = Vector3.Distance(startPos, newTarget.position);

            if (curDistance < newDistance)
                return curTarget;

            return newTarget;

        }


        public void PredationDash()
        {
            //  적이면 처형
            if (m_predationTarget.TryGetComponent<EnemyController>(out EnemyController enemy))
            {
                enemy.Execution(100);
                FoodPower food = enemy.GetFoodPower();
                m_hunger.AddHunger(food, 1);
                // 포만감
            }
            if (m_predationTarget.TryGetComponent<IPredationable>(out IPredationable predationable))
                predationable.Predation();

            m_damageable.RestoreHealth(m_player.Setting.PredationRestoreHealth);

            m_predationCoolDown = m_player.Setting.PredationCoolDown;

            if (m_dashRoutine != null)
            {
                StopCoroutine(m_dashRoutine);
                m_dashRoutine = null;
            }
            m_dashRoutine = StartCoroutine(DashRoutine());

        }

        IEnumerator DashRoutine()
        {
            float dashTimer = 0;

            Vector3 startPos = this.transform.position;
            Vector3 targetPos = m_predationTarget.position;

            while (dashTimer < m_player.Setting.DashDuration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, dashTimer / m_player.Setting.DashDuration);
                m_player.LookAt(targetPos);

                dashTimer += Time.deltaTime;
                yield return null;
            }

            Invoke(nameof(ResetTarget), m_player.Setting.PredationFatalityDuration);
            yield break;
        }

        public void ResetTarget()
        {
            m_predationTarget = null;
        }

    }
}