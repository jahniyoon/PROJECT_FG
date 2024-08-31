using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public partial class TestEnemyE : EnemyController
    {
        EnemyEData m_subData;
        [Header("Enemy E")]
        [SerializeField] private Transform m_shootPos;
        [SerializeField] private LayerMask m_targetLayer;
        [Header("Effects")]
        [SerializeField] private AimShader m_aimShader;
        [SerializeField] private ParticleSystem m_shootHitEffect;
        [SerializeField] private GameObject m_trailPrefab;
        [SerializeField] private float m_trailSpeed = 0.1f;

        [SerializeField] private GameObject m_targetPoint;

        private Transform[] m_trails;
        private int m_trailIndex;
        private WaitForSeconds m_trailSeconds;

        protected override void AwakeInit()
        {
            base.AwakeInit();

            m_trails = new Transform[10];
            for (int i = 0; i < 10; i++)
            {
                m_trails[i] = Instantiate(m_trailPrefab, GameManager.Instance.ProjectileParent).transform;
                m_trails[i].gameObject.SetActive(false);
            }
            m_trailIndex = 0;
            m_trailSeconds = new WaitForSeconds(0.1f);
        }

        protected override void StartInit()
        {
            base.StartInit();
            TryGetData();

        }
        private bool TryGetData()
        {
            var m_childData = m_data as EnemyEData;
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

        private void AimEnable(bool enable = true)
        {
            m_aimShader.gameObject.SetActive(enable);
            m_aimShader.SetRadius(m_subData.AttackRange, m_subData.AimAngle);
            m_aimShader.SetColor(m_subData.OuterColor, m_subData.SliderColor);
        }
        private void AimSlider(float value)
        {
            m_aimShader.SetSlider(value);
        }

        // 타겟 방향을 체크한다.
        private bool TargetAngleCheck()
        {         
            float angle = TargetAngle();

            return Mathf.Abs(angle) <= m_subData.AimAngle / 2;
        }

        private void Shoot()
        {
            m_shootPos.LookAt(m_target.position);

            RaycastHit hit;
            if (Physics.Raycast(m_shootPos.position, m_shootPos.forward, out hit, m_data.AttackRange, m_targetLayer, QueryTriggerInteraction.Ignore))
            {
                if (hit.transform.TryGetComponent<IDamageable>(out IDamageable damageable))
                {
                    damageable.OnDamage(m_subData.ShootDamage);
                    if (m_shootHitEffect)
                    {
                        m_shootHitEffect.Stop();
                        m_shootHitEffect.transform.position = hit.point;
                        m_shootHitEffect.Play();
                    }
                    StartCoroutine(TrailRoutine(m_trails[TrailIndex()], m_shootPos.position, hit.point));                   
                }
            }
            else
            {
                Vector3 forwardPoint = m_shootPos.position + m_shootPos.forward * m_data.AttackRange;
                if (m_shootHitEffect)
                {
                    m_shootHitEffect.Stop();
                    m_shootHitEffect.transform.position = forwardPoint;
                    m_shootHitEffect.Play();
                }
                StartCoroutine(TrailRoutine(m_trails[TrailIndex()], m_shootPos.position, forwardPoint));

            }
        }

        private int TrailIndex()
        {
            int curIndex = m_trailIndex;
            m_trailIndex++;

            if(10 <= m_trailIndex)
                m_trailIndex = 0;

            return curIndex;
        }

        IEnumerator TrailRoutine(Transform trail, Vector3 startPos, Vector3 endPos)
        {
            trail.gameObject.SetActive(true);
            float timer = 0;
            while(timer < m_trailSpeed)
            {
                trail.position = Vector3.Lerp(startPos, endPos, timer / m_trailSpeed);

                timer += Time.deltaTime;
                yield return null;
            }
            yield return m_trailSeconds;

            trail.gameObject.SetActive(false);

            yield break;
        }


        void OnDrawGizmosSelected()
        {
            bool canGizmo = TryGetData();

            if (canGizmo)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(transform.position, m_subData.AttackRange);
            }
        }
    }
}