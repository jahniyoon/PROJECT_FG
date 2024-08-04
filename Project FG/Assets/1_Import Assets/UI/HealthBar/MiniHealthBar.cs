using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class MiniHealthBar : MonoBehaviour
    {
        private GameObject _healthBar;
        private Vector2 _healthBarOffset;
        private bool _healthBarEnable;

        [Header("Slider")]
        [SerializeField] private Renderer m_healthSlider;
        [SerializeField] private Renderer m_hitSlider;


        [Header("Slider Setting")]
        [SerializeField] private float m_healthBarScale = 1;
        [SerializeField] private float m_hitDuration = 0.5f;
        [SerializeField] private float m_offset = 1;

        [Header("Visible Setting")]
        [SerializeField] private bool m_alwaysVisible = false;                  // 절대 꺼지지 않는다.    
        [SerializeField] private bool m_zeroHealthAutoDisable = true;       // 체력이 0이되면 자동으로 꺼진다.
        [SerializeField] private float m_autoDisableTime = 30f;



        private float m_lastHitTime;

        Coroutine hitRoutine;

        private void Awake()
        {
            _healthBar = transform.GetChild(0).gameObject;
            m_lastHitTime = Time.time - m_autoDisableTime;
        }

        private void Start()
        {
            transform.localScale = Vector3.one * m_healthBarScale;
            _healthBar.transform.localPosition = new Vector3(0, m_offset, 0);
        }

        private void OnEnable()
        {
            _healthBarOffset = Vector2.zero;
            m_healthSlider.material.SetTextureOffset("_BaseMap", _healthBarOffset);
            m_hitSlider.material.SetTextureOffset("_BaseMap", _healthBarOffset);
            HealthBarEnable(m_alwaysVisible);
        }

        private void LateUpdate()
        {
            if (_healthBarEnable)
            {
                _healthBar.transform.rotation = Camera.main.transform.rotation;

                if (m_lastHitTime + m_autoDisableTime < Time.time)
                {
                    HealthBarEnable(m_alwaysVisible);
                }

            }
        }

        // 체력바 끄고 켜기
        public void HealthBarEnable(bool enable)
        {
            _healthBarEnable = enable;
            _healthBar.gameObject.SetActive(enable);
        }

        // 체력바 슬라이더 변경
        public void SetHealthSlider(float value)
        {
            HealthBarEnable(true);

            m_lastHitTime = Time.time;

            // 값이 0~1 사이 값이여야함
            if (1 <= value)
            {
                value = 1;
            }
            else if (value <= 0)
            {
                value = 0;
            }
            float beforeHealth = _healthBarOffset.x;

            _healthBarOffset.x = 1 - value;
            m_healthSlider.material.SetTextureOffset("_BaseMap", _healthBarOffset);

            if (hitRoutine != null)
            {
                StopCoroutine(hitRoutine);
                hitRoutine = null;
                // 이미 코루틴이 실행중이면 현재까지의 값을 가져온다.
                beforeHealth = m_hitSlider.material.GetTextureOffset("_BaseMap").x;
            }

            hitRoutine = StartCoroutine(HitRoutine(beforeHealth));
        }

        // 데미지를 입을 때 주황색으로 표시되는 체력바
        IEnumerator HitRoutine(float value)
        {
            float timer = 0;
            Vector2 offset = Vector2.zero;
            offset.x = value;

            while (timer < m_hitDuration)
            {
                offset.x = Mathf.Lerp(value, _healthBarOffset.x, timer / m_hitDuration);
                m_hitSlider.material.SetTextureOffset("_BaseMap", offset);

                timer += Time.deltaTime;
                yield return null;
            }

            // 루틴이 끝난 뒤 체력 게이지가 0이면 자동으로 꺼진다.
            if (m_zeroHealthAutoDisable && 1 <= _healthBarOffset.x)
            {
                HealthBarEnable(false);
            }

            yield break;
        }

    }
}