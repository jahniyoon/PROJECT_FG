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
        [SerializeField] protected FoodPowerData m_data;

        [Header("Food Power")]
        [SerializeField] protected int m_powerLevel;
        [SerializeField] private float m_coolDownTimer;
        [SerializeField] protected bool m_mainPower;

        protected GameObject m_caster;
        protected Transform m_casterPosition;
        protected Transform m_aimPosition;

        // 푸드파워 실행 루틴
        Coroutine foodRoutine;

        bool isActive;

        public Sprite Icon => m_data.Icon;
        public float CoolDown => m_data.GetLevelData(m_powerLevel).CoolDown;
        public float Timer => m_coolDownTimer;
        public bool Main => m_mainPower;
        public int ID => m_data.ID;

        private void Awake()
        {
            Init();
        }

        public virtual void Init(bool isMain = false)
        {
            if (m_data == null)
                return;

            SetLevel(0);
            m_coolDownTimer = 0;
        }
        public void SetMain(bool enable = true)
        {
            m_mainPower = enable;
        }
        public virtual void SetCaster(GameObject caster, Transform casterPosition, Transform aim)
        {
            m_caster = caster;
            m_casterPosition = casterPosition;
            m_aimPosition = aim;
        }

        public void SetTimer(float timer)
        {
            m_coolDownTimer = timer;
        }

        // 푸드파워의 효과
        public virtual void Active()
        {

        }
        public virtual void Inactive()
        {

        }

        public virtual void LevelUp()
        {
            m_powerLevel++;
        }
        // 레벨 업
        public virtual void SetLevel(int value)
        {
            m_powerLevel = value;
        }




        // 타입에 따라 발사 방향을 정해준다.
        protected Quaternion GetDirection()
        {
            Quaternion direction = Quaternion.identity;

            switch (m_data.GetLevelData(m_powerLevel).AimType)
            {
                // 플레이어 방향
                case FoodPowerAimType.MoveDirection:
                    return m_casterPosition.localRotation;


                // 가까운 타겟 방향
                case FoodPowerAimType.TargetNearest:
                    Transform target = ScanPosition(m_casterPosition.position, m_data.TargetNearestScanRadius);

                    // 타겟이 Null이 아닐 경우에만
                    if (target != null)
                        return Quaternion.LookRotation(target.position - m_casterPosition.position);

                    // 만약 항상 쏴야하는 경우, 플레이어 방향으로 다시 바꿔준다.
                    else if (target == null && m_data.AlwaysShoot)
                        return m_casterPosition.rotation;

                    break;

                // 포인터 방향
                case FoodPowerAimType.PointerDirection:
                    return Quaternion.LookRotation(m_aimPosition.position - m_casterPosition.position);

                // PC의 포지션
                case FoodPowerAimType.PcPosition:
                    return direction;

                    // 랜덤한 방향
                case FoodPowerAimType.RandomDirection:
                    Vector3 randomDir = direction.eulerAngles;
                    randomDir.y = Random.Range(0, 360);
                    direction.eulerAngles = randomDir;
                    return direction;

                    // 랜덤한 적 방향
                case FoodPowerAimType.RandomEnemyDirection:
                    Transform randomTarget = ScanRandomPosition(m_casterPosition.position, m_data.TargetNearestScanRadius);

                    // 타겟이 Null이 아닐 경우에만
                    if (randomTarget != null)
                        return Quaternion.LookRotation(randomTarget.position - m_casterPosition.position);

                    // 만약 항상 쏴야하는 경우, 플레이어 방향으로 다시 바꿔준다.
                    else if (randomTarget == null && m_data.AlwaysShoot)
                        return m_casterPosition.rotation;
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

        //  스캔된 에네미중 랜덤으로 포지션을 정한다.
        private Transform ScanRandomPosition(Vector3 position, float radius)
        {
            Transform target = null;

            Collider[] colls = Physics.OverlapSphere(position, radius);
            List<Collider> enemies = new List<Collider>();
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
                    enemies.Add(colls[i]);
                }
            }
            int random = Random.Range(0, enemies.Count);

            target = enemies[random].transform;
            return target;
        }
        // 타겟의 거리를 체크한다.
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
            Inactive();
            isActive = false;
        }


        // 쿨타임에 맞게 실행이된다.
        IEnumerator FoodPowerRoutine()
        {
            m_coolDownTimer = m_data.GetLevelData(m_powerLevel).CoolDown;

            while (isActive)
            {
                // 쿨타임이 지나면 액티브
                if (m_data.GetLevelData(m_powerLevel).CoolDown <= m_coolDownTimer)
                {
                    Active();
                    m_coolDownTimer = 0;

                    // 지속시간이 짧은 케이스의 경우 바로 종료
                    if (m_data.GetLevelData(m_powerLevel).Duration < 0)
                        yield break;

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