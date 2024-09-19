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
        [SerializeField] private FoodPower m_defaultFoodPower;
        [SerializeField] private List<FoodPower> foodPowers = new List<FoodPower>();

        [Header("Food Combo")]
        [SerializeField] SerializableDictionary<FoodPower, int> m_FoodComboEffects = new SerializableDictionary<FoodPower, int>();
        [SerializeField] SerializableDictionary<FoodPower, int> m_foodComboStacks = new SerializableDictionary<FoodPower, int>();

        [SerializeField] private List<string> foodComboNames;


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

            m_defaultFoodPower = m_gameSettings.DefaultFoodPower;
        }
        private void Start()
        {
            SetMaxHunger(m_player.Setting.MaxHunger);
            foodPowers.Clear();
            m_cantPredation = false;

            if (m_defaultFoodPower != null)
                AddHunger(m_defaultFoodPower, 0);
        }


        // 최대 포만감 설정
        public void SetMaxHunger(int maxHunger, int hunger = 0)
        {
            m_maxHunger = maxHunger;
            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, hunger);
            UIManager.Instance.MainUI.FoodPowerUI.SetMaxFoodPower(m_maxHunger);
        }

        // 포식 성공 시 푸드파워 추가
        public FoodPower AddHunger(FoodPower foodPower, int hunger = 0)
        {
            // 푸드파워 깊은 복사 이후에 초기화
            FoodPower newPower = Instantiate(foodPower.gameObject, m_foodPowerParent).GetComponent<FoodPower>();
            string PowerName = hunger == 0 ? $"Default : {foodPower.name} {foodPowers.Count}" :
                $"{foodPower.name} {foodPowers.Count}";

            newPower.gameObject.name = PowerName;
            newPower.Init();

            // 포만도에 영향이 없으면 효과푸드파워이다.
            if (hunger == 0)
                newPower.EffectFoodPower();

            // 푸드파워의 캐스터 세팅
            newPower.SetCaster(this.gameObject, m_player.Model, m_player.Aim);

            // 현재 포만도 추가
            m_curHunger += hunger;

            // 먹은게 있어야 아이콘을 켜기
            if (hunger != 0)
                UIManager.Instance.MainUI.FoodPowerUI.AddFoodPower(newPower.Icon);
            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, m_curHunger);

            bool cantStart = false;

            // 맥스 값에 도달하면 포만도 소모기 발동
            if (m_maxHunger <= m_curHunger)
            {
                m_cantPredation = true;
                // 플레이어를 멈춘다.
                m_player.SetFreeze(true);

                // 딜레이 뒤에 실행한다.
                Invoke(nameof(HungerSkill), m_player.Setting.HungerSkillDelay);
                cantStart = true;
            }
            else
            {
                for (int i = 0; i < foodPowers.Count; i++)
                {
                    // 같은 푸드파워가 있다면 레벨 업
                    if (foodPowers[i].ID == foodPower.ID)
                    {
                        cantStart = true;

                        if (hunger == 0)
                        {
                            for(int j = 0; j <= foodPower.Level; j++)
                                foodPowers[i].LevelUp();                            
                        }

                        else
                            foodPowers[i].LevelUp();

                        break;
                    }
                }
            }
            // 리스트에 추가한다.
            foodPowers.Add(newPower);
            CheckFoodCombo();

            // 레벨업이 아닌경우에만 루틴을 시작
            if (cantStart == false)
            {
                newPower.SetMain(true);
                StartFoodPowerRoutine();

            }

            return newPower;
        }

        // 푸다파워 콤보
        private void CheckFoodCombo()
        {
            int count = 0;
            m_foodComboStacks.Clear();

            FoodPower targetFP = null;
            for (int i = 0; i < foodPowers.Count; i++)
            {
                // 효과는 계산하지 않는다.
                if (foodPowers[i].IsEffectFoodPower)
                    continue;

                // 7이상은 패스
                if (7 <= count)
                    continue;

                // 없으면 비워주고 다음으로
                if (targetFP == null)
                {
                    targetFP = foodPowers[i];
                    count = 1;
                    continue;
                }
                // 만약 같으면 카운트를 높인다.
                if (targetFP.ID == foodPowers[i].ID)
                {
                    count++;
                    continue;
                }
                // 아니면 푸드파워 초기화
                else
                {
                    // 카운트가 2개 이상일 경우에만 콤보로 인정하고 삽입
                    if (3 <= count)
                        m_foodComboStacks.Add(targetFP, count-2);

                    // 다음 타겟
                    targetFP = foodPowers[i];
                    count = 1;
                }
            }
            // 순회하고 나서 카운트가 2개 이상일 경우에만 콤보로 인정하고 삽입
            if (3 <= count)
                m_foodComboStacks.Add(targetFP, count-2);

            UIManager.Instance.MainUI.FoodComboUI.SetFoodComboUI(m_FoodComboEffects, m_foodComboStacks);
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
                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        float distance = Vector3.Distance(transform.position, colls[i].transform.position);

                        Vector3 hitPoint = colls[i].ClosestPoint(transform.position);

                        damageable.OnDamage(m_player.Setting.HungerSkillDamage);
                    }

                }
            }
        }


        // 푸드파워를 모두 모았을 때
        public void HungerLevelUp()
        {
            m_curHunger = 0;
            m_cantPredation = false;

            // 식사콤보 세팅
            SetFoodCombo();

            // 모두 리스트에서 지우고 삭제
            foreach (FoodPower power in foodPowers)
            {
                // 파워도 꺼야함
                power.Inactive();
                Destroy(power.gameObject);
            }

            foodPowers.Clear();

            // 기본 푸드파워를 넣어준다.
            if (m_defaultFoodPower != null)
                AddHunger(m_defaultFoodPower, 0);

            foreach (var item in m_FoodComboEffects)
            {
                AddHunger(item.Key, 0);
                Destroy(item.Key.gameObject);
            }


            UIManager.Instance.MainUI.FoodPowerUI.SetMaxFoodPower(m_maxHunger);
            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, m_curHunger);
        }
        private void SetFoodCombo()
        {
            // 식사콤보 이전
            m_FoodComboEffects.Clear();

            foreach (var item in m_foodComboStacks)
            {
                // 푸드파워 깊은 복사 이후에 초기화
                FoodPower newPower = Instantiate(item.Key.gameObject, m_foodPowerParent).GetComponent<FoodPower>();
                newPower.gameObject.name = $"Effect : {newPower.name} Lv.{item.Value}";
                newPower.Init();
                newPower.SetLevel(item.Value);
                m_FoodComboEffects.Add(newPower, item.Value);
            }
            m_foodComboStacks.Clear();
            UIManager.Instance.MainUI.FoodComboUI.SetFoodComboUI(m_FoodComboEffects, m_foodComboStacks);

        }


        private void StartFoodPowerRoutine()
        {
            if (foodPowers.Count == 0)
                return;
            SetFoodPowerRoutine();
        }

        private void SetFoodPowerRoutine()
        {
            StopFoodPowerRoutine();

            for (int i = 0; i < foodPowers.Count; i++)
            {

                if (foodPowers[i].Main)
                    foodPowers[i].StartFoodPowerRoutine();

            }
        }
        public void StopFoodPowerRoutine()
        {
            // 먼저 루틴을 모두 꺼준다.
            for (int i = 0; i < foodPowers.Count; i++)
            {
                foodPowers[i].StopFoodPowerRoutine();
            }

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

        [ContextMenu("Add Power")]
        public void AddDefaultPower()
        {
            AddHunger(m_defaultFoodPower, 1);
        }


        void OnDrawGizmosSelected()
        {
            if (m_gameSettings != null)
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

