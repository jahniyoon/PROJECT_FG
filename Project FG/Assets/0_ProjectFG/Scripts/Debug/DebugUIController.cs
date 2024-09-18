using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JH
{
    public class DebugUIController : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_waveText;
        [SerializeField] private TMP_Text m_killCountText;

        [SerializeField] private DamageDebugController m_damageDebugController;
        private int m_killCount = 0;

        public void OnDamage(float damage, Transform position)
        {
            m_damageDebugController.OnDamage(damage, position);
        }

        public void WaveText(int wave)
        {
            m_waveText.text = wave + " Wave";
        }

        public void KillCountText(int count)
        {
            m_killCount += count;
            m_killCountText.text = m_killCount + " Kill";
        }
        public void DebugEnable(bool enable)
        {
            m_damageDebugController.gameObject.SetActive(enable);
            this.gameObject.SetActive(enable);
        }
    }

}