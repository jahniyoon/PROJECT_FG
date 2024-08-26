using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PlayerHunger : MonoBehaviour
    {
        private PlayerController m_player;
        [SerializeField] private GameSettings m_gameSettings;

        [Header("Hunger")]
        [SerializeField] private int m_curHunger;
        [SerializeField] private int m_maxHunger;
        [SerializeField] private bool m_cantPredation = false;


        [Header("Food Power")]
        [SerializeField] private List<FoodPower> foodPowers = new List<FoodPower>();

        private Transform m_foodPowerParent;
        private Coroutine m_foodPowerRoutine;

        public bool CantPredation => m_cantPredation;
        [Header("Skill Effect")]
        [SerializeField] private ParticleSystem m_hungerSkillEffect;

        [SerializeField] private Color DebugColor;

        private void Awake()
        {
            m_player = GetComponent<PlayerController>();

            m_foodPowerParent = new GameObject("Food Power").transform;
            m_foodPowerParent.parent = this.transform;
        }
        private void Start()
        {
            SetMaxHunger(m_player.Setting.MaxHunger);
            foodPowers.Clear();
            m_cantPredation = false;
        }

        // Legacy
        //private void Update()
        //{
        //    // 푸드파워들의 쿨타임을 계속 업데이트 해준다.
        //    FoodPowerManager();
        //}

        // 최대 포만감 설정
        public void SetMaxHunger(int maxHunger, int hunger = 0)
        {
            m_maxHunger = maxHunger;
            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, hunger);
            UIManager.Instance.MainUI.FoodPowerUI.SetMaxFoodPower(m_maxHunger);
        }

        // 포식 성공 시 푸드파워 추가
        public void AddHunger(FoodPower foodPower, int hunger = 0)
        {
            // 푸드파워 깊은 복사 이후에 초기화
            FoodPower newPower = Instantiate(foodPower.gameObject, m_foodPowerParent).GetComponent<FoodPower>();
            newPower.Init();

            // 현재 포만도 추가
            m_curHunger += hunger;

            // 푸드파워의 유저를 지정
            newPower.SetTransform(m_player.Model, m_player.Aim);
            newPower.SetAimType(m_player.Setting.FoodPowerAimType);

            // 리스트에 추가한다.
            foodPowers.Add(newPower);

            UIManager.Instance.MainUI.FoodPowerUI.AddFoodPower(newPower.Icon);

            // 맥스 값에 도달하면 포만도 소모기 발동
            if (m_maxHunger <= m_curHunger)
            {
                m_cantPredation = true;
                // 플레이어를 멈춘다.
                m_player.SetFreeze(true);

                // 딜레이 뒤에 실행한다.
                Invoke(nameof(HungerSkill), m_player.Setting.HungerSkillDelay);
            }
            // 레벨업까지 끝내고 푸드파워의 루틴을 초기화 시키고 시작시킨다.
            StartFoodPowerRoutine();

            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, m_curHunger);
        }


        // 포만감 소모기
        private void HungerSkill()
        {
            HungerSkillAttack();
            if (m_hungerSkillEffect)
            {
                // 이펙트의 크기를 세팅하고 공격
                m_hungerSkillEffect.transform.localScale = Vector3.one * m_player.Setting.HungerSkillRange;
                m_hungerSkillEffect?.Stop();
                m_hungerSkillEffect?.Play();
            }

            // 멈춤을 풀어준다.
            m_player.SetFreeze(false);


            // 소모기를 사용하고 레벨업 한다.
            HungerLevelUp();
        }

        private void HungerSkillAttack()
        {
            Collider[] colls = Physics.OverlapSphere(transform.position, m_player.Setting.HungerSkillRange);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag("Enemy"))
                {
                    Damageable damageable = colls[i].GetComponent<Damageable>();

                    if (damageable)
                    {
                        float distance = Vector3.Distance(transform.position, colls[i].transform.position);

                        Vector3 hitPoint = colls[i].ClosestPoint(transform.position);

                        damageable.OnDamage(m_player.Setting.HungerSkillDamage);
                        colls[i].GetComponent<EnemyController>().OnStun(m_player.Setting.HungerSkillStun);
                    }
                }
            }
        }


        // 푸드파워를 모두 모았을 때
        public void HungerLevelUp()
        {
            m_curHunger = 0;
            m_cantPredation = false;

            // 모두 리스트에서 지우고 삭제
            foreach (FoodPower power in foodPowers)
            {
                Destroy(power.gameObject);
            }

            foodPowers.Clear();

            UIManager.Instance.MainUI.FoodPowerUI.SetMaxFoodPower(m_maxHunger);
            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, m_curHunger);
        }


        private void StartFoodPowerRoutine()
        {
            if (foodPowers.Count == 0)
                return;


            if (m_foodPowerRoutine != null)
            {
                StopCoroutine(m_foodPowerRoutine);
                m_foodPowerRoutine = null;
            }

            m_foodPowerRoutine = StartCoroutine(FoodPowerRoutine());
        }

        // 푸드파워 루틴을 딜레이를 주며 시작시킨다.
        IEnumerator FoodPowerRoutine()
        {
            // 먼저 루틴을 모두 꺼준다.
            for (int i = 0; i < foodPowers.Count; i++)
            {
                foodPowers[i].StopFoodPowerRoutine();
            }


            for (int i = 0; i < foodPowers.Count; i++)
            {
                foodPowers[i].StartFoodPowerRoutine();
                yield return new WaitForSeconds(m_player.Setting.FoodPowerDelay);

            }
            yield break;
        }


        void OnDrawGizmosSelected()
        {
            if(m_gameSettings!=null)
            {
                Gizmos.color = DebugColor;
                Gizmos.DrawSphere(transform.position, m_gameSettings.HungerSkillRange);
            }

        }

    }
}


// Legacy : 푸드파워들을 관리하는 메서드
//public void FoodPowerManager()
//{
//    if (foodPowers.Count == 0)
//        return;

//    for(int i = 0; i < foodPowers.Count; i++)
//    {
//        if (foodPowers[i].CoolDown < foodPowers[i].Timer)
//        {
//            foodPowers[i].Active();
//            foodPowers[i].SetTimer(0);
//            continue;
//        }

//        float m_posCheckTimer = foodPowers[i].Timer + Time.deltaTime;
//        foodPowers[i].SetTimer(m_posCheckTimer);                
//    }           
//}

