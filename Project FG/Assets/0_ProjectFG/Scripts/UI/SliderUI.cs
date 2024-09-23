using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class SliderUI : MonoBehaviour
    {
        private TMP_Text m_text;
        private Slider m_slider;

        private float m_maxValue;
        private float m_curValue;
        public bool isDebugs;

        private void Awake()
        {
            m_text = GetComponentInChildren<TMP_Text>();
            m_slider = GetComponentInChildren<Slider>();
        }

        /// <summary>
        /// 슬라이더를 세팅한다.
        /// </summary>
        /// <param name="maxValue">슬라이더의 최대 값</param>
        /// <param name="value">슬라이더의 값</param>
        public void SetSlider(float maxValue, float value)
        {
            m_maxValue = maxValue;
            m_curValue = value;

            float result = Mathf.Round((m_curValue / m_maxValue) * 100) / 100;


            m_slider.maxValue = 1;
            m_slider.value = result;

            if (isDebugs)
            {
                Debug.Log($"입력값 : {result} / {1}, 슬라이더 : {m_slider.value} / {m_slider.maxValue}, 인풋 : {value} / {maxValue}");
            }
            SetText();
        }

        public void UpdateSlider(float nextValue)
        {
            SetSlider(m_maxValue, nextValue);
        }

        public void SetText()
        {
            m_text.text = m_curValue + " / " + m_maxValue;
        }
    }
}