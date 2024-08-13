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

            m_slider.maxValue = m_maxValue ;
            m_slider.value = m_curValue;
            SetText();
        }

        public void UpdateSlider(float nextValue)
        {
            SetSlider(m_maxValue, nextValue);
        }

        public void SetText()
        {
            m_text.text = m_curValue + " / "  + m_maxValue;
        }
    }
}