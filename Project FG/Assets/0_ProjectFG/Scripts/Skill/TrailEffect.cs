using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace JH
{
	public class TrailEffect : MonoBehaviour
	{
        [SerializeField] private GameObject m_trailPrefab;
        [SerializeField] private ParticleSystem m_shootHitEffect;
        [SerializeField] private Transform[] m_trails;
        [SerializeField] private float m_trailSpeed;
        int m_trailIndex;
        WaitForSeconds m_trailSeconds;

        private void Awake()
        {

            m_trails = new Transform[10];
            for (int i = 0; i < 10; i++)
            {
                m_trails[i] = Instantiate(m_trailPrefab, GameManager.Instance.ProjectileParent).transform;
                m_trails[i].gameObject.SetActive(false);
            }
            m_trailIndex = 0;
            m_trailSeconds = new WaitForSeconds(0.1f);
        }
        private int TrailIndex()
        {
            int curIndex = m_trailIndex;
            m_trailIndex++;

            if (10 <= m_trailIndex)
                m_trailIndex = 0;

            return curIndex;
        }

 

        public void OnTrail(Vector3 start, Vector3 end)
        {
            StartCoroutine(TrailRoutine(m_trails[TrailIndex()], start, end));

        }
        IEnumerator TrailRoutine(Transform trail, Vector3 startPos, Vector3 endPos)
        {
            trail.gameObject.SetActive(true);
            float timer = 0;
            while (timer < m_trailSpeed)
            {
                trail.position = Vector3.Lerp(startPos, endPos, timer / m_trailSpeed);

                timer += Time.deltaTime;
                yield return null;
            }
            if (m_shootHitEffect)
            {
                m_shootHitEffect.Stop();
                m_shootHitEffect.transform.position = endPos;
                m_shootHitEffect.Play();
            }
            yield return m_trailSeconds;

            trail.gameObject.SetActive(false);

            yield break;
        }
    }
}
