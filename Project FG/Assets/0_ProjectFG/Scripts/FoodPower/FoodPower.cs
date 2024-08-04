using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace JH
{
    [System.Serializable]
    public class FoodPower : MonoBehaviour
    {
        [SerializeField] FoodPowerData m_data;

        [Header("Food Power")]
        [SerializeField] private string m_powerName;
        [SerializeField] private Sprite m_powerIcon;
        [SerializeField] private float m_powerCoolDown;

        [SerializeField] private float m_coolDownTimer;

        protected Transform m_player;


        private bool m_activate;

        public Sprite Icon => m_powerIcon; 
        public float CoolDown => m_powerCoolDown;
        public float Timer => m_coolDownTimer;

        private void Awake()
        {
            if (m_data == null)
                return;

            m_powerName = m_data.FoodPowerName;
            m_powerIcon = m_data.Icon;
            m_powerCoolDown = m_data.CoolDown;
        }

        public void SetTransform(Transform t)
        {
            m_player = t;
        }

        public void SetTimer(float timer)
        {
            m_coolDownTimer = timer;
        }

        public virtual void Active()
        {
        }
    }
}