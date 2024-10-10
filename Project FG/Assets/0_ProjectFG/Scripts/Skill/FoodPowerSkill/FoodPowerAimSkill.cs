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
        Coroutine m_aimRouine;
        float m_targetResearchTimer;

        [Header("에임")]
        [SerializeField] private float m_targetResearchTime = 0.1f;

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
            CreateProjectiles();

        }

        protected override void UpdateBehavior()
        {
            UpdateAimTargetLine();

            if (m_targetResearchTimer <= 0 && Target == null)
            {
                ResearchTarget();
            }
            m_targetResearchTimer -= Time.deltaTime;

        }
        private void ResearchTarget()
        {
            m_targetResearchTimer = m_targetResearchTime;

            Collider[] colls = Physics.OverlapSphere(transform.position, LevelData.Range);
     

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
            SetTarget(target);

            EnemyController enemy = Target.GetComponent<EnemyController>();
            float timer = 0;

            bool aimEnable = false;
            float aimTimer = 0;
            while (timer < LevelData.TryGetValue1(0)) 
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
                    SetTarget(null);
                    yield break;
                }
                timer += Time.deltaTime;
                aimTimer -= Time.deltaTime;
                yield return null;
            }

            LineEnable(true);


            timer = 0;
            float shootTimer = 0;
            while (timer < LevelData.TryGetValue1(2))
            {
                // 죽으면 조준 종료
                if (enemy.State == FSMState.Die)
                {
                    SetTarget(null);
                    yield break;
                }
               
                // 연사속도 넘기면 발사
                if (shootTimer <= 0)
                {
                    ActiveProjectiles();
                    //Shoot();
                    // 연사속도로 초기화
                    shootTimer = 1 / LevelData.TryGetValue1(1);
                }
                shootTimer -= Time.deltaTime;
                timer += Time.deltaTime;
                yield return null;
            }


            SetTarget(null);
            yield break;
        }

        private void Shoot()
        {
            RaycastHit hit;
            if (Target == null)
                return;
            float distance = Vector3.Distance(transform.position, Target.position);

            if (Physics.Raycast(m_aimEffect.transform.position, m_aimEffect.transform.forward, out hit, distance + 1, Data.TargetLayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.OnDamage(LevelData.Damage);

                    m_trailEffect?.OnTrail(transform.position, hit.point);
                }
            }
        }

        private Vector3 aimScale = Vector3.one;
        private Vector3 targetPos;
        private void UpdateAimTargetLine()
        {
            m_aimEffect.gameObject.SetActive(Target != null);

            if (Target == null)
                return;
            targetPos = Target.position;
            targetPos.y = m_aimEffect.transform.position.y;

            m_aimEffect.transform.LookAt(targetPos);


            float Distance = Vector3.Distance(transform.position, Target.position);
            aimScale.z = Distance;
            m_aimEffect.transform.localScale = aimScale;


        }

        private void LineEnable(bool enable)
        {
            m_spriteRenderer.enabled = enable;
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, 1);
        }

    }
}
