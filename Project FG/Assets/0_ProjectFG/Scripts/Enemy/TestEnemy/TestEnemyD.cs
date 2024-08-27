using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.AI;
using Unity.VisualScripting;


namespace JH
{
    public partial class TestEnemyD : EnemyController
    {
        private EnemyDData m_subData;

        [Header("Enemy D")]
        [SerializeField] private DonutAttack m_skill;

   


        protected override void StartInit()
        {
            base.StartInit();

            EnemyDData m_childData = m_data as EnemyDData;
            if(m_childData != null)
            {
                m_subData = m_childData;
            }
            else
            {
                Debug.Log(this.gameObject.name + "데이터를 다시 확인해주세요.");
                this.enabled = false;
            }
        }

        private void ShootDonut(Vector3 targetPos)
        {

            var projectile = Instantiate(m_skill, targetPos, Quaternion.identity, GameManager.Instance.ProjectileParent);
            projectile.SkillInit(m_subData.AttackDamage, m_subData.OuterRadius, m_subData.InnerRadius, m_subData.SliderDuration);
            projectile.SetColor(m_subData.OuterColor, m_subData.SliderColor);
            
            // 투사체 초기화하고
            //projectile.ProjectileInit(m_data.AttackDamage);
            // 발사
            //projectile.Shoot(targetPos);
        }




        //void OnDrawGizmosSelected()
        //{
        //    Gizmos.color = new Color(1, 0, 0, 0.5f);
        //    Gizmos.DrawSphere(transform.position, m_AttackDamageRadius);
        //}
    }
}