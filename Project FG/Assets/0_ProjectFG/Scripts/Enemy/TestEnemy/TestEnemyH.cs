using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyH : EnemyController
    {
        EnemyHData m_subData;
        [Header("Enemy H")]
        [SerializeField] private ParticleSystem[] m_explosionEffect;
        private bool m_explosionTrigger;

        protected override void StartInit()
        {
            base.StartInit();
            TryGetData();
        }
        private bool TryGetData()
        {
            EnemyHData m_childData = m_data as EnemyHData;
            if (m_childData != null)
            {
                m_subData = m_childData;
                return true;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
                return false;
            }
        }

        private void Explosion()
        {

            Collider[] colls = Physics.OverlapSphere(transform.position , m_subData.ExplosionRadius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].CompareTag("Player"))
                {
                    Damageable damageable = colls[i].GetComponent<Damageable>();

                    if (damageable)
                    {
                        damageable.OnDamage(m_subData.AttackDamage);
                    }
                }
            }
            foreach (var effect in m_explosionEffect)
            {
                effect.transform.parent = GameManager.Instance.ProjectileParent;
                effect.gameObject.SetActive(true);
                effect.Play();
                Destroy(effect.gameObject, 2);

            }

            m_damageable.Die();

        }


        
    }
}