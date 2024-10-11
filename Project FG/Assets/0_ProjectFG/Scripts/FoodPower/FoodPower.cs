using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
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
        [Header("푸드파워 활성화 여부")]
        [SerializeField] protected bool m_mainPower;
        [Header("습득한 푸드파워")]
        [SerializeField] protected bool m_effectFoodPower;

        protected Transform m_aimPosition;

        // 푸드파워 실행 루틴
        Coroutine foodRoutine;
        ISkillCaster m_caster;


        bool isActive;

        #region Property
        public ISkillCaster Caster => m_caster;
        public Sprite Icon => m_data.Icon;
        public float CoolDown => m_data.GetLevelData(m_powerLevel).CoolDown;
        public float Timer => m_coolDownTimer;
        public bool Main => m_mainPower;
        public int ID => m_data.ID;
        public int Level => m_powerLevel;
        public bool IsEffectFoodPower => m_effectFoodPower;
        public FoodPowerData Data => m_data;
        #endregion

        public UnityEvent ActiveEvent = new UnityEvent();
        public UnityEvent LevelUpEvent = new UnityEvent();

        private void OnDisable()
        {
            ActiveEvent.RemoveAllListeners();
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
        public virtual void SetCaster(ISkillCaster caster, Transform aim)
        {
            m_caster = caster;
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
       

        

        // 스킬이 애니메이션을 발동한다.
        public void SkillActiveEvent()
        {
            ActiveEvent.Invoke();
        }
        public virtual void Inactive()
        {

        }

        public virtual void Remove()
        {
            Inactive();
        }

        public virtual void LevelUp()
        {
            m_powerLevel++;
            Inactive();
            Active();
            LevelUpEvent?.Invoke();
        }

        // 레벨 업
        public virtual void SetLevel(int value)
        {
            m_powerLevel = value;
            LevelUpEvent?.Invoke();

        }




        // 타입에 따라 발사 방향을 정해준다.
        protected Quaternion GetDirection()
        {
            return Quaternion.identity;
            float randomRadius = 5;
            Quaternion direction = Quaternion.identity;

            if (m_data.AlwaysShoot == false)
            {
                Transform randomTarget = ScanRandomPosition(Caster.Transform.position, randomRadius);
                if(randomTarget == null)
                    return Quaternion.identity;
            }


            //switch (m_data.GetLevelData(m_powerLevel).AimType)
            //{
            //    // 플레이어 방향
            //    case AimType.MoveDirection:
            //        return Caster.Transform.localRotation;


            //    // 가까운 타겟 방향
            //    case AimType.NearTargetDirection:
            //        Transform target = ScanPosition(Caster.Transform.position, randomRadius);

            //        // 타겟이 Null이 아닐 경우에만
            //        if (target != null)
            //            return Quaternion.LookRotation(target.position - Caster.Transform.position);

            //        // 만약 항상 쏴야하는 경우, 플레이어 방향으로 다시 바꿔준다.
            //        else if (target == null && m_data.AlwaysShoot)
            //            return Caster.Transform.rotation;

            //        break;

            //    // 포인터 방향
            //    case AimType.PointerDirection:
            //        return Quaternion.LookRotation(m_aimPosition.position - Caster.Transform.position);

            //    // PC의 포지션
            //    case AimType.PcPosition:
            //        return direction;

            //        // 랜덤한 방향
            //    case AimType.RandomDirection:
            //        Vector3 randomDir = direction.eulerAngles;
            //        randomDir.y = Random.Range(0, 360);
            //        direction.eulerAngles = randomDir;
            //        return direction;

            //        // 랜덤한 적 방향
            //    case AimType.RandomEnemyDirection:
            //        Transform randomTarget = ScanRandomPosition(Caster.Transform.position, randomRadius);

            //        // 타겟이 Null이 아닐 경우에만
            //        if (randomTarget != null)
            //        {
            //            if(randomTarget.position - Caster.Transform.position != Vector3.zero)
            //            return Quaternion.LookRotation(randomTarget.position - Caster.Transform.position);
            //        }

            //        // 만약 항상 쏴야하는 경우, 플레이어 방향으로 다시 바꿔준다.
            //        else if (randomTarget == null && m_data.AlwaysShoot)
            //            return Caster.Transform.rotation;
            //        break;
            //}

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

            if (enemies.Count == 0)
                return null;

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
        public void ActiveFoodPower()
        {
            Active();

            isActive = true;
        }
        // 루틴을 멈춘다.
        public void InActiveFoodPower()
        {
            Inactive();
            isActive = false;
        }



        // 이 푸드파워는 계산하지 않는다.
        public void EffectFoodPower()
        {
            m_effectFoodPower = true;
        }
    }
}