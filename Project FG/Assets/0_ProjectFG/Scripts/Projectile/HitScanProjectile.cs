using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace JH
{
    public class HitScanProjectile : ProjectileBase
    {



        [SerializeField] private float m_trailSpeed = 0.1f;

        [Header("Effect")]
        [SerializeField] private List<ParticleSystem> m_shootEffects = new List<ParticleSystem>();
        [SerializeField] private GameObject m_trailPrefab;


        private int m_trailIndex;
        private Transform[] m_trails;
        private WaitForSeconds m_trailSeconds;
        private Transform m_trailParent;

        protected override void AwakeInit()
        {
            GameObject parent = new GameObject("Trails");
            m_trailParent = parent.transform;
            m_trailParent.parent = GameManager.Instance.ProjectileParent;

            m_trails = new Transform[10];
            for (int i = 0; i < 10; i++)
            {
                m_trails[i] = Instantiate(m_trailPrefab, m_trailParent).transform;
                m_trails[i].gameObject.SetActive(false);
            }
            m_trailIndex = 0;
            m_trailSeconds = new WaitForSeconds(0.1f);

        }
  

        public override void ActiveProjectile()
        {
            Shoot();
        }

        private void Shoot()
        {

            Vector3 target = transform.position + transform.forward * m_skill.LevelData.Range;

            if (m_skill.Target != null)
                target = m_skill.Target.position;

            target.y = transform.position.y;
            transform.LookAt(target);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, m_skill.LevelData.Range, m_skill.Data.TargetLayer, QueryTriggerInteraction.Ignore))
            {

                if (hit.transform.CompareTag(m_skill.Data.SkillTarget.ToString()))
                {

                    if (hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
                    {
                        damageable.OnDamage(m_skill.Caster.FinalDamage(m_skill.LevelData.Damage));

                        OnBuff(hit.transform);
                        RemoveBuff(hit.transform);

                    }
                }
                HitEffect(hit.point);
                StartCoroutine(TrailRoutine(m_trails[TrailIndex()], transform.position, hit.point));
            }
            else
            {
                Vector3 forwardPoint = transform.position + transform.forward * m_skill.LevelData.Range;
                HitEffect(forwardPoint);
                StartCoroutine(TrailRoutine(m_trails[TrailIndex()], transform.position, forwardPoint));

            }
        }
        private int TrailIndex()
        {
            int curIndex = m_trailIndex;
            m_trailIndex++;

            if (10 <= m_trailIndex)
                m_trailIndex = 0;

            return curIndex;
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
            yield return m_trailSeconds;

            trail.gameObject.SetActive(false);

            yield break;
        }



        private void HitEffect(Vector3 position)
        {
            foreach (var effect in m_shootEffects)
            {
                effect.Stop();
                effect.transform.position = position;
                effect.Play();
            }
        }

        public override ProjectileBase InActiveProjectile()
        {
            Destroy(m_trailParent.gameObject);
            return this;
        }




    }


}
