using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class MineProjectile : ProjectileBase
    {
        Rigidbody m_rigid;
        SphereCollider m_collider;
        [Header("Collision")]
        [SerializeField] bool m_isStopDestroy;

        [SerializeField] GameObject m_mine;

        private bool isSpawn;


        protected override void AwakeInit()
        {
            base.AwakeInit();
            m_collider = GetComponent<SphereCollider>();
            m_rigid = GetComponent<Rigidbody>();
        }

        public override void SetSkill(SkillBase skill)
        {
            base.SetSkill(skill);
            m_model.transform.localScale = Vector3.one * m_data.ProjectileScale;
        }
        public override void SetRadius(float radius)
        {
            base.SetRadius(radius);
            m_collider.radius = radius;

        }

        public override void ActiveProjectile()
        {
            base.ActiveProjectile();

            isSpawn = true;
        }

        public override ProjectileBase InActiveProjectile()
        {
            Destroy(gameObject);
            return this;
        }
        public void Explosion()
        {

            CreateDerivativesProjectiles();

            m_isStopDestroy = true;
            //Collider[] colls = Physics.OverlapSphere(transform.position, m_skill.LevelData.TryGetValue1(), m_skill.Data.TargetLayer, QueryTriggerInteraction.Ignore);
            //for (int i = 0; i < colls.Length; i++)
            //{


            //    if (colls[i].CompareTag(m_skill.Data.SkillTarget.ToString()))
            //    {
            //         180도만 제한한다.
            //        if (GFunc.TargetAngleCheck(transform, colls[i].transform, m_skill.LevelData.Arc) == false)
            //            continue;


            //        if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
            //        {
            //            damageable.OnDamage(m_skill.LevelData.Damage);
            //        }


            //         버프도 같이 보낸다.
            //        m_skill.OnBuff(colls[i].transform);
            //        m_skill.RemoveBuff(colls[i].transform);
            //    }
            //}

            m_mine.gameObject.SetActive(false);
            PlayEffect();

        }




        protected override void DebugProjectile()
        {
            m_debug.gameObject.SetActive(true);
            m_debug.position = transform.position;
            m_debug.localScale = Vector3.one * m_radius;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.isTrigger || m_isStopDestroy || isSpawn == false)
                return;

            if (other.CompareTag(m_skill.Data.SkillTarget.ToString()))
            {
                Explosion();
            }

        }
    }

}
