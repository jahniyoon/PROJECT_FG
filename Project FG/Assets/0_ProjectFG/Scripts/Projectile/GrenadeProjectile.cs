using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class GrenadeProjectile : ProjectileBase
    {
        [Header("Grenade")]
        [SerializeField] private bool m_randomRange;

        [Header("Explosion Effect")]
        [SerializeField] private Transform m_effectTransform;

        Vector3 m_targetPos;


        public override void SetSkill(SkillBase skill)
        {
            base.SetSkill(skill);

            float range = skill.LevelData.Range;
            if (m_randomRange)
                range = Random.Range(0, skill.LevelData.Range);

            m_targetPos = transform.position + transform.forward * range;
            m_effectTransform.localScale = Vector3.one * skill.LevelData.Radius;
            StartCoroutine(ShootRoutine());
        }

        public override void ActiveProjectile()
        {
            StartCoroutine(ShootRoutine());
        }
        IEnumerator ShootRoutine()
        {
            m_spawnPos = transform.position;
            float timer = 0;
            while (timer < m_skill.LevelData.LifeTime)
            {
                transform.position = Vector3.Lerp(m_spawnPos, m_targetPos, timer / m_skill.LevelData.LifeTime);

                timer += Time.deltaTime;
                yield return null;
            }
            Explosion();
            PlayEffect();

            timer = 0;
            while (timer < 1)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            InActiveProjectile();
            yield break;
        }


        private void Explosion()
        {
            m_model.gameObject.SetActive(false);
            Collider[] colls = Physics.OverlapSphere(transform.position, m_skill.LevelData.Radius, m_skill.Data.TargetLayer, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < colls.Length; i++)
            {

                if (colls[i].CompareTag(m_skill.Data.SkillTarget.ToString()))
                {
                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                        damageable.OnDamage(m_skill.LevelData.Damage);

                    if (colls[i].TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
                        OnBuff(colls[i].transform);
                }
            }


        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
        }
    }
}
