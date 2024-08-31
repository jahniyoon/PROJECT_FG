using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace JH
{
    [System.Serializable]
    public class FoodPower : MonoBehaviour
    {
        [SerializeField] FoodPowerData m_data;

        [Header("Food Power")]
        [SerializeField] private string m_powerName;
        [SerializeField] private Sprite m_powerIcon;
        [SerializeField] private float m_powerCoolDown;

        [SerializeField] private float m_coolDownTimer;

        protected Transform m_startT;
        protected Transform m_aimT;
        protected FoodPowerAimType m_aimType;   // 조준 타입

        // 푸드파워 실행 루틴
        Coroutine foodRoutine;

        bool isActive;

        public Sprite Icon => m_powerIcon;
        public float CoolDown => m_powerCoolDown;
        public float Timer => m_coolDownTimer;

        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            if (m_data == null)
                return;

            m_powerName = m_data.FoodPowerName;
            m_powerIcon = m_data.Icon;
            m_powerCoolDown = m_data.CoolDown;
            m_coolDownTimer = 0;
        }

        public void SetTransform(Transform start, Transform aim)
        {
            m_startT = start;
            m_aimT = aim;
        }

        public void SetTimer(float timer)
        {
            m_coolDownTimer = timer;
        }

        // 푸드파워의 효과
        public virtual void Active()
        {

        }


        // 조준 타입을 설정한다.
        public void SetAimType(FoodPowerAimType type)
        {
            m_aimType = type;
        }

        // 타입에 따라 발사 방향을 정해준다.
        protected Quaternion GetDirection()
        {
            Quaternion direction = Quaternion.identity;

            switch (m_aimType)
            {
                case FoodPowerAimType.PlayerDirection:
                    direction = m_startT.rotation;
                    break;

                case FoodPowerAimType.TargetNearest:
                    Transform target = ScanPosition(m_startT.position, m_data.TargetNearestScanRadius);

                    // 타겟이 Null이 아닐 경우에만
                    if (target != null)
                        direction = Quaternion.LookRotation(target.position - m_startT.position);

                    // 만약 항상 쏴야하는 경우, 플레이어 방향으로 다시 바꿔준다.
                    else if (target == null && m_data.AlwaysShoot)
                        direction = m_startT.rotation;

                    break;

                case FoodPowerAimType.PointerDirection:
                    direction = Quaternion.LookRotation(m_aimT.position - m_startT.position);
                    break;
            }

            return direction;
        }



        private Transform ScanPosition(Vector3 position, float radius)
        {
            Transform target = null;

            Collider[] colls = Physics.OverlapSphere(position, radius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                    continue;

                if (colls[i].CompareTag("Enemy") == false)
                    continue;

                if (colls[i].TryGetComponent<EnemyController>(out EnemyController enemy))
                {
                    if (enemy.State == FSMState.Die)
                        continue;

                    target = DistanceChecker(position, target, enemy.transform);
                }

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

        // 푸드파워 루틴을 실행한다.
        public void StartFoodPowerRoutine()
        {
            // 이미 실행되고 있는 루틴이 있다면 끄고 시작하다.
            StopFoodPowerRoutine();

            isActive = true;
            foodRoutine = StartCoroutine(FoodPowerRoutine());


        }
        // 루틴을 멈춘다.
        public void StopFoodPowerRoutine()
        {
            if (foodRoutine != null)
            {
                StopCoroutine(foodRoutine);
                foodRoutine = null;
            }
            isActive = false;
        }


        IEnumerator FoodPowerRoutine()
        {
            m_coolDownTimer = 0;

            while (isActive)
            {
                // 쿨타임이 지나면 액티브
                if (m_powerCoolDown < m_coolDownTimer)
                {
                    Active();
                    m_coolDownTimer = 0;
                    yield return null;
                    continue;
                }

                m_coolDownTimer += Time.deltaTime;
                yield return null;
            }

            yield break;
        }


    }
}