using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
	public class AimAndShootSkill : SkillBase
	{
        IAimSkillCaster m_aimCaster;

        [Header("Aim Shader")]
        [SerializeField] private AimShader m_aimShader;
        [SerializeField] private Transform m_targetMark;


        [Header("Shader Color")]
        [SerializeField] private Color m_outerColor;
        [SerializeField] private Color m_sliderColor;

        [Header("Aim State")]
        [SerializeField] private float m_aimTimer;
        [SerializeField] private float m_attackTimer;
        [SerializeField] private float m_shootTimer;

        [SerializeField] ProjectileBase m_hitScan;

        Coroutine m_skillRoutine;
        private bool m_aimReady;
        

        protected override void Init()
        {
            CreateProjectiles();
            if(Caster.Transform.TryGetComponent<IAimSkillCaster>(out IAimSkillCaster aimCaster))
            {
                m_aimCaster = aimCaster;
            }
            else 
            {
                Debug.Log("스킬 시전자의 타입을 확인해주세요.");
            }
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            ActiveAimRoutine();
        }

        public override void InactiveSkill()
        {
            base.InactiveSkill();
            AimEnable(false);

            m_targetMark.gameObject.SetActive(false);
        }

        private void ActiveAimRoutine()
        {

            if (m_skillRoutine != null)
            {
                StopCoroutine(m_skillRoutine);
                m_skillRoutine = null;
            }
            m_skillRoutine = StartCoroutine(AimAndShootRoutine());


        }
        private void ResetAimTimer()
        {
            m_aimTimer = 0;
            m_attackTimer = 0;
            m_shootTimer = 0;
        }



        private IEnumerator AimAndShootRoutine()
        {
            ResetAimTimer();
            m_aimReady = false;
            while (true)
            {
                AimEnable(m_aimCaster.AimState == AimState.Shoot);
                m_targetMark.gameObject.SetActive(m_aimCaster.AimState == AimState.Aim);

                // 조준상태
                if (m_aimCaster.AimState == AimState.Aim)
                    AimBehavior();

                // 조준 이후 상태
                else if(m_aimCaster.AimState == AimState.Shoot)
                    ShootBehavior();



                yield return null;  
            }

            yield break;
        }

        private void AimBehavior()
        {

            if(Data.LevelData.TryGetValue1(1) < m_aimTimer)
            {
                ResetAimTimer();
                m_aimReady = true;

                return;
            }
            m_aimTimer += Time.deltaTime;

            Vector3 markPos = m_skillTarget.transform.position;
            markPos.y = m_targetMark.position.y;
            markPos.z += 1f;

            m_targetMark.transform.position = markPos;
            m_targetMark.transform.localScale = Vector3.one * (m_aimTimer - Data.LevelData.TryGetValue1(1)) * 0.5f;
            transform.rotation = Caster.Model.rotation;

        }

        private void ShootBehavior()
        {
            AimSlider(m_attackTimer / Data.LevelData.Duration);

            if(Data.LevelData.Duration < m_attackTimer)
            {
                ResetAimTimer();
                m_aimReady = false;
                return;
            }
            // 바로 발사하도록 하기위해, 슛 타이머가 0이 될 때마다 발사
            if (Data.LevelData.TryGetValue1() <= m_shootTimer)
            {
                ActiveProjectiles();
                m_shootTimer = 0;
            }

            m_attackTimer += Time.deltaTime;
            m_shootTimer += Time.deltaTime;

        }


        private void AimEnable(bool enable)
        {
            m_aimShader.gameObject.SetActive(enable);
            m_aimShader.SetRadius(Data.SkillRadius, Data.SkillArc);
            m_aimShader.SetColor(m_outerColor, m_sliderColor);
        }
        private void AimSlider(float value)
        {
            m_aimShader.SetSlider(value);
        }

    }
}
