using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JH
{
    public class FoodPowerIcon : MonoBehaviour
    {
        [SerializeField] private Image m_icon;
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
    }
}