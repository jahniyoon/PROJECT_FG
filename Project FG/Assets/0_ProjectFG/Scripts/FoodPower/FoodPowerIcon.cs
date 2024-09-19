using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class FoodPowerIcon : MonoBehaviour
    {
        [SerializeField] private Image m_icon;
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private GameObject m_enable;
        private Sprite m_emptyIcon;

        private void Awake()
        {
            ResetIcon();
        }


        public void SetIcon(Sprite icon)
        {
            m_icon.gameObject.SetActive(true);
            m_icon.sprite = icon;
        }

        public void ResetIcon()
        {
            m_icon.sprite = null;
            m_icon.gameObject.SetActive(false);
        }
        public void SetText(string text)
        {
            if (m_text == null)
                return;
            m_text.text = text;
        }
        public void SetEnable(bool enable)
        {
            if (m_enable == null)
                return;
            m_enable.SetActive(!enable);
        }
    }
}