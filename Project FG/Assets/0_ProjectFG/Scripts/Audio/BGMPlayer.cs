using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public class BGMPlayer : MonoBehaviour
	{
        [SerializeField] private string m_bgmName;
        [SerializeField] private bool m_playerOnStart;

        private void Start()
        {
            if (m_playerOnStart)
                Play();
        }

        public void Play()
        {
            AudioManager.Instance.PlayBGM(m_bgmName);
        }
    }
}
