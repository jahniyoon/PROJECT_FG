using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JH
{
	public class WaveUIController : MonoBehaviour
	{
        [SerializeField] private GameObject m_nextWaveUI;
        [Header("Score")]
        [SerializeField] private TMP_Text m_waveText;
        [SerializeField] private TMP_Text m_remainEnemy;
        

        public void SetWave(int wave, bool enable = true)
        {
            m_waveText.gameObject.SetActive(enable);
            m_waveText.text = wave.ToString() + " Wave";
        }
        public void SetRemainEnemy(int value, bool enable = true)
        {
            m_remainEnemy.gameObject.SetActive(enable);  
            m_remainEnemy.text = "Enemy : " + value.ToString();
        }


        public void NextWave(bool enable)
        {
            m_nextWaveUI.SetActive(enable);
        }

	}
}
