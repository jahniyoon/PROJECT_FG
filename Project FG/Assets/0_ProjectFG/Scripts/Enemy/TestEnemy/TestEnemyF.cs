using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyF : EnemyController
    {



        [Header("Area Skill")]
        [SerializeField] private ParticleSystem m_skillEffect;
        [SerializeField] private float m_skillTickDamage = 1;       // 틱 데미지
        [SerializeField] private float m_skillTickDuration = 0.5f;     // 틱 간격
        [SerializeField] private float m_skillAreaRadius = 5;   // 스킬 범위


        [Header("Skill Debuff")]
        [SerializeField] private float m_speedDebuff = 0.5f;   // 속도 디버프
        [SerializeField] private float m_needTicks = 5;   // 스킬 적용에 필요한 틱
        [SerializeField] private float m_DebuffDuration = 2;   // 스킬 적용에 필요한 틱





        protected override void StartInit()
        {
            base.StartInit();

            SkillStart();
        }

        private void SkillStart()
        {
            m_skillEffect?.Stop();
            m_skillEffect?.Play();

            StartCoroutine(SkillRoutine());
        }

        IEnumerator SkillRoutine()
        {
            float timer = 0;
            Vector3 effectScale = Vector3.one;

            while(timer < m_skillTickDuration)
            {
                effectScale.x = m_skillAreaRadius * (timer / m_skillTickDuration);
                effectScale.z = m_skillAreaRadius * (timer / m_skillTickDuration);

                m_skillEffect.transform.localScale = effectScale;
                timer += Time.deltaTime;
                yield return null;  
            }


            // 죽기 전까지 틱간격동안 반복
            while (State != FSMState.Die)
            {

                Collider[] colls = Physics.OverlapSphere(transform.position, m_skillAreaRadius);
                for (int i = 0; i < colls.Length; i++)
                {
                    if (colls[i].isTrigger)
                    {
                        continue;
                    }

                    if (colls[i].CompareTag("Enemy"))
                    {

                    }

                    if (colls[i].CompareTag("Player"))
                    {
                        colls[i].GetComponent<IDamageable>().OnDamage(m_skillTickDamage);
                        // TODO : 플레이어에게 틱 주는 디버프 넣기
                    }
                }



                yield return new WaitForSeconds(m_skillTickDuration);
            }

            yield break;
        }








        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, m_skillAreaRadius);
        }
    }
}