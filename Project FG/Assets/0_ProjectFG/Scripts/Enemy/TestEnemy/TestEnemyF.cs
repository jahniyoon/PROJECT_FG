using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyF : EnemyController
    {

        private EnemyFData m_subData;

        private ParticleSystem m_skillEffect;
        [SerializeField] private SkillBase m_frozenSkill;

        protected override void StartInit()
        {
            base.StartInit();

            EnemyFData m_childData = m_data as EnemyFData;
            if (m_childData != null)
            {
                m_subData = m_childData;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
            }
            m_frozenSkill = Instantiate(m_subData.FrozenSkill, transform).GetComponent<SkillBase>();
            m_frozenSkill.SkillInit(this.gameObject, m_model);
            m_frozenSkill.ActiveSkill();
        }

        protected override void Die()
        {
            base.Die();
            m_frozenSkill.InactiveSkill();
        }

        //private void SkillStart()
        //{
        //    m_skillEffect?.Stop();
        //    m_skillEffect?.Play();

        //    StartCoroutine(SkillRoutine());
        //}

        //IEnumerator SkillRoutine()
        //{
        //    float timer = 0;
        //    Vector3 effectScale = Vector3.one;

        //    while(timer < m_subData.SkillTickDuration)
        //    {
        //        effectScale.x = m_subData.SkillAreaRadius * (timer / m_subData.SkillTickDuration);
        //        effectScale.z = m_subData.SkillAreaRadius * (timer / m_subData.SkillTickDuration);

        //        m_skillEffect.transform.localScale = effectScale;
        //        timer += Time.deltaTime;
        //        yield return null;  
        //    }


        //    // 죽기 전까지 틱간격동안 반복
        //    while (State != FSMState.Die)
        //    {

        //        Collider[] colls = Physics.OverlapSphere(transform.position, m_subData.SkillAreaRadius);
        //        for (int i = 0; i < colls.Length; i++)
        //        {
        //            if (colls[i].isTrigger)
        //            {
        //                continue;
        //            }

        //            if (colls[i].CompareTag("Enemy"))
        //            {

        //            }

        //            if (colls[i].CompareTag("Player"))
        //            {
        //                colls[i].GetComponent<IDamageable>().OnDamage(m_subData.SkillTickDamage);
        //                // TODO : 플레이어에게 틱 주는 디버프 넣기
        //            }
        //        }
        //        yield return new WaitForSeconds(m_subData.SkillTickDuration);
        //    }

        //    yield break;
        //}








        //void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = new Color(1, 0, 0, 0.5f);
        //    Gizmos.DrawSphere(transform.position, m_skillAreaRadius);
        //}
    }
}