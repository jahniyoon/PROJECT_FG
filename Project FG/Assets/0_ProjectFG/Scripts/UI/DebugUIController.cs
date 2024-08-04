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
        private int m_killCount = 0;


        public void WaveText(int wave)
        {
            m_waveText.text = wave + " Wave";
        }

        public void KillCountText(int count)
        {
            m_killCount += count;
            m_killCountText.text = m_killCount + " Kill";
        }
    }

}