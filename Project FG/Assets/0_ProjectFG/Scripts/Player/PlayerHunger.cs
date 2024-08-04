using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PlayerHunger : MonoBehaviour
    {
        private PlayerController m_player;

        [Header ("Hunger")]
        [SerializeField] private int m_curHunger;
        [SerializeField] private int m_maxHunger;

        [Header ("Food Power")]
        [SerializeField] private List<FoodPower> foodPowers = new List<FoodPower>();




        private void Awake()
        {
            m_player = GetComponent<PlayerController>();
        }
        private void Start()
        {
            SetMaxHunger(m_player.Setting.MaxHunger);

            foodPowers.Clear();
        }

        private void Update()
        {
            FoodPowerManager();
        }


        public void SetMaxHunger(int maxHunger, int hunger = 0)
        {
            m_maxHunger = maxHunger;
            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, hunger);
            UIManager.Instance.MainUI.FoodPowerUI.SetMaxFoodPower(m_maxHunger);
        }

        public void AddHunger(FoodPower foodPower, int hunger = 0)
        {
            m_curHunger += hunger;

            foodPower.SetTransform(m_player.Model);
            foodPowers.Add(foodPower);

            UIManager.Instance.MainUI.FoodPowerUI.AddFoodPower(foodPower.Icon);

            if (m_maxHunger <= m_curHunger)
            {
                HungerLevelUp();
            }

            UIManager.Instance.MainUI.HungerUI.SetSlider(m_maxHunger, m_curHunger);
        }


        public void HungerLevelUp()
        {
            m_curHunger = 0;

            foodPowers.Clear();

            UIManager.Instance.MainUI.FoodPowerUI.SetMaxFoodPower(m_maxHunger);
        }

        public void FoodPowerManager()
        {
            if (foodPowers.Count == 0)
                return;
      
            for(int i = 0; i < foodPowers.Count; i++)
            {
                if (foodPowers[i].CoolDown < foodPowers[i].Timer)
                {
                    foodPowers[i].Active();
                    foodPowers[i].SetTimer(0);
                    continue;
                }

                float timer = foodPowers[i].Timer + Time.deltaTime;
                foodPowers[i].SetTimer(timer);                
            }



           
        }

    }
}