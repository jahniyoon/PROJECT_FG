using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerAProjectile : CollisionProjectile
    {


        public override void Collision()
        {
            // 캐스터 타겟이면 사용하지 않는다.
            if (m_skill.Data.SkillTarget == TargetTag.Caster)
                return;

            Collider[] colls = Physics.OverlapSphere(transform.position, m_radius, m_skill.Data.TargetLayer, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < colls.Length; i++)
            {


                if (colls[i].CompareTag(m_skill.Data.SkillTarget.ToString()))
                {
                    // 180도만 제한한다.
                    if (GFunc.TargetAngleCheck(transform, colls[i].transform, m_skill.LevelData.Arc) == false)
                        continue;


                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        damageable.OnDamage(m_skill.Caster.FinalDamage(m_skill.LevelData.Damage, DamageType.Default));
                    }

                    // 푸드파워 A는 스택을 내린다.
                    if (colls[i].TryGetComponent<AttackMark>(out AttackMark mark))
                        mark.StackDown();

                    // 버프도 같이 보낸다.
                    m_skill.OnBuff(colls[i].transform);
                }
            }

            PlayEffect();

            if (0 <= m_skill.LevelData.LifeTime)
                Invoke(nameof(InActiveProjectile), m_skill.LevelData.LifeTime);
        }


    }

}
