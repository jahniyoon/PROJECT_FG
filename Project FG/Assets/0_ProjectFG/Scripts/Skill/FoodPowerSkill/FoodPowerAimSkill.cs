using GoogleSheetsToUnity;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerAimSkill : FoodPowerSkill
    {

        private FoodPowerESkillData m_subData;
        [Header("에임")]
        [SerializeField] private float m_targetResearchTime = 0.1f;
        [SerializeField] private Transform m_target;
        [SerializeField] private LayerMask m_targetLayer;

        Coroutine m_aimRouine;
        float m_targetResearchTimer;
        [Header("무시 에네미 ID")]
        [SerializeField] private int[] m_ignoreIDs;

        [Header("이펙트")]
        [SerializeField] private GameObject m_aimEffect;
        [SerializeField] private float m_aimDuration = 0.25f;
        private SpriteRenderer m_spriteRenderer;
        [SerializeField] private TrailEffect m_trailEffect;


        protected override void Init()
        {
            m_spriteRenderer = m_aimEffect.GetComponentInChildren<SpriteRenderer>();
            m_subData = m_data as FoodPowerESkillData;
            if (m_subData == null)
            {
                Debug.LogError("데이터를 확인해주세요.");
                return;
            }

        }

        public override void LeagcyActiveSkill()
        {
            base.LeagcyActiveSkill();
        }
        protected override void UpdateBehavior()
        {
            UpdateAimTargetLine();

            if (m_targetResearchTimer <= 0 && m_target == null)
            {
                ResearchTarget();
            }
            m_targetResearchTimer -= Time.deltaTime;

        }
        private void ResearchTarget()
        {
            m_targetResearchTimer = m_targetResearchTime;

            Collider[] colls = Physics.OverlapSphere(transform.position, m_levelData.Range);
     

            List<Collider> enemies = new List<Collider>();
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].isTrigger)
                {
                    continue;
                }

                if (colls[i].TryGetComponent<EnemyController>(out EnemyController enemy))
                {
                    if (EnemyCheck(enemy))
                        enemies.Add(colls[i]);
                }
            }

            if (enemies.Count <= 0)
                return;

            int random = Random.Range(0, enemies.Count);

            Transform target = enemies[random].transform;

            if(m_aimRouine !=null)
            {
                StopCoroutine(m_aimRouine);
                m_aimRouine = null;
            }

            m_aimRouine = StartCoroutine(AimRoutine(target));
        }

        private bool EnemyCheck(EnemyController enemy)
        {
            if (enemy.State == FSMState.Die)
                return false;

            foreach (var id in m_ignoreIDs)
            {
                if (enemy.ID == id)
                    return false;
            }
            return true;    
        }

        // 조준 루틴
        IEnumerator AimRoutine(Transform target)
        {
            // 타겟 세팅 후 조준상태로 전환
            m_target = target;

            EnemyController enemy = m_target.GetComponent<EnemyController>();
            float timer = 0;

            bool aimEnable = false;
            float aimTimer = 0;
            while (timer < m_levelData.GetAdditionalValue(0)) 
            {
                if(aimTimer <= 0)
                {
                    aimEnable = !aimEnable;
                    LineEnable(aimEnable);

                    aimTimer = m_aimDuration;
                }


                // 죽으면 조준 종료
                if(enemy.State == FSMState.Die)
                {
                    m_target = null;
                    yield break;
                }
                timer += Time.deltaTime;
                aimTimer -= Time.deltaTime;
                yield return null;
            }

            LineEnable(true);


            timer = 0;
            float shootTimer = 0;
            while (timer < m_levelData.GetAdditionalValue(2))
            {
                // 죽으면 조준 종료
                if (enemy.State == FSMState.Die)
                {
                    m_target = null;
                    yield break;
                }
               
                // 연사속도 넘기면 발사
                if (shootTimer <= 0)
                {
                    Shoot();
                    // 연사속도로 초기화
                    shootTimer = 1 / m_levelData.GetAdditionalValue(1);
                }
                shootTimer -= Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }


            m_target = null;
            yield break;
        }

        private void Shoot()
        {
            RaycastHit hit;
            if (m_target == null)
                return;
            float distance = Vector3.Distance(transform.position, m_target.position);

            if (Physics.Raycast(m_aimEffect.transform.position, m_aimEffect.transform.forward, out hit, distance + 1, m_targetLayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.OnDamage(m_levelData.Damage);

                    m_trailEffect?.OnTrail(transform.position, hit.point);
                }
            }
        }

        private Vector3 aimScale = Vector3.one;
        private Vector3 targetPos;
        private void UpdateAimTargetLine()
        {
            m_aimEffect.gameObject.SetActive(m_target != null);

            if (m_target == null)
                return;
            targetPos = m_target.position;
            targetPos.y = m_aimEffect.transform.position.y;

            m_aimEffect.transform.LookAt(targetPos);


            float Distance = Vector3.Distance(transform.position, m_target.position);
            aimScale.z = Distance;
            m_aimEffect.transform.localScale = aimScale;


        }

        private void LineEnable(bool enable)
        {
            m_spriteRenderer.enabled = enable;
        }



        public override void InactiveSkill()
        {
            base.InactiveSkill();
            Destroy(gameObject);

        }
        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
        }

    }
}
