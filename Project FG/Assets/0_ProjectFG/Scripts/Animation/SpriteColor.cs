using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class SpriteColor : MonoBehaviour
    {
        SpriteRenderer[] m_renderers;
        private Material m_defaultMaterial;
        [Header("Hit")]
        [SerializeField] private float m_hitDuration = 0.25f;
        [SerializeField] private float m_flickingDuration = 0.25f;
        [SerializeField] private Material m_hitMaterial;
        Coroutine m_hitRoutine;
        Coroutine m_flickingRoutine;

        private void Awake()
        {
            m_renderers = transform.GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer renderer in m_renderers)
            {
                m_defaultMaterial = renderer.material;
            }
        }

        public void OnHit()
        {

            m_hitRoutine = StartCoroutine(HitRoutine(m_hitDuration));

        }

        public void StopRoutine()
        {
            if (m_hitRoutine != null)
            {
                StopCoroutine(m_hitRoutine);
                m_hitRoutine = null;
            }
            HitSprite(false);
        }
        private void HitSprite(bool enable)
        {
            if (enable == false)
            {
                foreach (SpriteRenderer renderer in m_renderers)
                {
                    renderer.material = m_defaultMaterial;
                }
            }

            else
            {
                foreach (SpriteRenderer renderer in m_renderers)
                {
                    renderer.material = m_hitMaterial;
                }
            }
        }

        public void PlayFlicking()
        {
            StopFlicking();
            m_flickingRoutine = StartCoroutine(FlickingRoutine());
        }
        public void StopFlicking()
        {
            if (m_flickingRoutine != null)
            {
                StopCoroutine(m_flickingRoutine);
                m_flickingRoutine = null;
            }
            HitSprite(false);
        }

        IEnumerator FlickingRoutine()
        {
            float flickingTimer = 0;
            bool enable = false;

            while (true)
            {
                if (flickingTimer <= 0)
                {
                    enable = !enable;
                    HitSprite(enable);
                    flickingTimer = m_flickingDuration;
                }
                flickingTimer -= Time.deltaTime;
                yield return null;
            }
        }

        IEnumerator HitRoutine(float Duration)
        {
            float timer = 0;
            bool enable = false;
            float flickingTimer = 0;

            while (timer <= Duration)
            {
                if(flickingTimer <= 0)
                {
                    enable = !enable;

                    HitSprite(enable);
                    flickingTimer = m_hitDuration;
                }
                flickingTimer -= Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }
            HitSprite(false);

            yield break;
        }


    }
}
