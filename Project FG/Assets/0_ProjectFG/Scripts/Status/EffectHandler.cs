using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public class EffectHandler : MonoBehaviour
	{
        [SerializeField] private ParticleSystem m_stunEffect;

        public void StunEnable(bool enable)
        {
            if (m_stunEffect == null)
                return;
            if (m_stunEffect.isPlaying)
                return;
            m_stunEffect.gameObject.SetActive(true);

            m_stunEffect.Stop();
            if(enable)
                m_stunEffect.Play();

        }

        public void StopAllEffect()
        {
            m_stunEffect.gameObject.SetActive(false);
        }

    }
}
