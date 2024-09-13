using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerSlashSkill : FoodPowerSkill
    {
        [Header("근접공격 스킬")]

        private SlashSkillData m_subData;
        [SerializeField] private GameObject m_slashEffect;

        [SerializeField] private bool m_debug;
        protected override void Init()
        {
            m_subData = m_skillData as SlashSkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            Slash();

            StartCoroutine(SkillRoutine());
        }

        public void Slash()
        {
            if (m_debug)
            {
                Transform DebugObj = transform.GetChild(0);
                DebugObj.position = transform.position + m_casterPosition.forward * m_levelData.GetValue(0);
                DebugObj.localScale = Vector3.one * m_levelData.Radius * 2;
                DebugObj.gameObject.SetActive(true);
            }

            Collider[] colls = Physics.OverlapSphere(transform.position + m_casterPosition.forward * m_levelData.GetValue(0), m_levelData.Radius);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }


                if (colls[i].CompareTag(m_subData.Target.ToString()))
                {
                    // 180도만 제한한다.
                    if (GFunc.TargetAngleCheck(transform, colls[i].transform, 180) == false)
                        continue;


                    if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    {
                        float distance = Vector3.Distance(transform.position, colls[i].transform.position);

                        Vector3 hitPoint = colls[i].ClosestPoint(transform.position);

                        damageable.OnDamage(m_levelData.Damage);
                    }
                }
            }
            if(m_slashEffect != null)
            {
                var visualEffect = Instantiate(m_slashEffect, transform).GetComponentInChildren<VisualEffect>();
                visualEffect.Play();
            }
        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();
            Destroy(gameObject);

        }


    }
}
