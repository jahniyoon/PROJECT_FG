using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class CollisionProjectile : ProjectileBase
    {
        [Header("Collision")]
        [SerializeField] bool m_isStopDestroy;




        public override void ActiveProjectile()
        {
            base.ActiveProjectile();
            Collision();
        }


        public void Collision()
        {
            // 캐스터 타겟이면 사용하지 않는다.
            if (m_skill.Data.SkillTarget == TargetTag.Caster)
                return;

            Collider[] colls = Physics.OverlapSphere(transform.position, m_skill.LevelData.Radius, m_skill.Data.TargetLayer, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < colls.Length; i++)
            {


                if (colls[i].CompareTag(m_skill.Data.SkillTarget.ToString()))
                {
                    // 180도만 제한한다.
                    if (GFunc.TargetAngleCheck(transform, colls[i].transform, m_skill.LevelData.Arc) == false)
                        continue;


                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_skill.LevelData.Damage);
                    }


                    // 버프도 같이 보낸다.
                    m_skill.OnBuff(colls[i].transform);
                    m_skill.RemoveBuff(colls[i].transform);
                }
            }

            PlayEffect();

            if (0 <= m_skill.LevelData.LifeTime)
                Invoke(nameof(StopEffect), m_skill.LevelData.LifeTime);
        }


        protected override void DebugProjectile()
        {
            m_debug.gameObject.SetActive(true);
            m_debug.position = transform.position;
            m_debug.localScale = Vector3.one * m_skill.LevelData.Radius;
        }
    }

}
