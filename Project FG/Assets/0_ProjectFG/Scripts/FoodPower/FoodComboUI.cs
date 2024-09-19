using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class FoodComboUI : MonoBehaviour
    {
        [SerializeField] private List<FoodPowerIcon> m_foodPowerIcons = new List<FoodPowerIcon>();
        [SerializeField] private int m_iconPooling = 10;
        private GameObject m_iconObject;
        private int m_curValue;
        private int m_maxValue;

        private void Awake()
        {
            m_iconObject = transform.GetChild(0).gameObject;
            m_iconObject.SetActive(false);

            for (int i = 0; i < m_iconPooling; i++)
            {
                FoodPowerIcon newIcon = Instantiate(m_iconObject, transform).GetComponent<FoodPowerIcon>();
                m_foodPowerIcons.Add(newIcon);
            }

        }


        public void SetMaxFoodPower(int value, bool reset = true)
        {
            if(reset)
            {
                m_curValue = 0;
                ResetIcons();
            }

            if (m_iconPooling < value)
            {
                Debug.LogWarning("풀링 숫자보다 높습니다.");
                return;
            }

            for (int i = 0; i < value; i++)
            {
                m_foodPowerIcons[i].ResetIcon();
                m_foodPowerIcons[i].gameObject.SetActive(true);
            }

            m_maxValue = value;
        }

        public void AddFoodPower(Sprite icon)
        {
            // 추가 못한다.
            if (m_maxValue <= m_curValue)
                return;

            m_foodPowerIcons[m_curValue].SetIcon(icon);

            m_curValue ++;
        }
        

        private void ResetIcons()
        {
            for (int i = 0; i < m_foodPowerIcons.Count; i++)
            {
                m_foodPowerIcons[i].ResetIcon();
                m_foodPowerIcons[i].gameObject.SetActive(false);
            }
        }

        public void SetFoodComboUI(Dictionary<FoodPower, int> EffectCombo, Dictionary<FoodPower, int> StackCombo)
        {
            // 우선 다꺼준다.
            foreach (var item in m_foodPowerIcons)
                item.gameObject.SetActive(false);


            int count = 0;
            // 활성화 되어야하는 콤보
            foreach(var item in EffectCombo)
            {
                m_foodPowerIcons[count].gameObject.SetActive(true);
                m_foodPowerIcons[count].SetEnable(true);
                m_foodPowerIcons[count].SetText($"Lv.{item.Value}");
                m_foodPowerIcons[count].SetIcon(item.Key.Icon);
                count++;
            }

            foreach (var item in StackCombo)
            {
                m_foodPowerIcons[count].gameObject.SetActive(true);
                m_foodPowerIcons[count].SetText($"Lv.{item.Value}");
                m_foodPowerIcons[count].SetIcon(item.Key.Icon);
                count++;
            }

        }
    }
}