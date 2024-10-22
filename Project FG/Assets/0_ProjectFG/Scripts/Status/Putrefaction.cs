using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
	public class Putrefaction
	{
        [SerializeField] private BuffBase m_buff;
        [SerializeField] private float m_duration;
        [SerializeField] private float m_damage;
        [SerializeField] private float m_interval;
        [SerializeField] private float m_radius;
        [SerializeField] private float m_magDamage;
        [SerializeField] private float m_magDuration;


        [SerializeField] private float m_timer;
        private bool m_putrefactionOver;
        public float Timer => m_timer;
        public float Duration => m_duration;    

        // 부패의 종료를 알리는 변수
        public bool CanPutrefactionDamage => m_interval < m_timer;
        public bool isPutrefactionOver => m_putrefactionOver;
        public void SetPutrefaction(float duration, float damage, float interval, float radius, float magDamage = 1, float magDuration = 1)
        {
            m_duration = duration;
            m_damage = damage;
            m_interval = interval;
            m_radius = radius;
            m_magDamage = magDamage;
            m_magDuration = magDuration;
        }
        public void ResetTimer()
        {
            m_timer = 0;
        }
        public void Tick(float deltatime)
        {
            m_timer += deltatime;
        }
        public void OnPutrefactionDamage(BuffHandler handler)
        {
            if (handler.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.OnDamage(m_buff.FinalDamage(m_damage, DamageType.Attribute), new Color(0.5f, 0.25f, 1, 1));
            ResetTimer();
        }

        // 부패 전이
        public void OnPutrefactionTransition(BuffHandler handler)
        {
            Collider[] colls = Physics.OverlapSphere(handler.transform.position, m_radius, LayerMask.NameToLayer("Enemy"), QueryTriggerInteraction.Ignore);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].TryGetComponent<IPutrefaction>(out IPutrefaction target))
                {
                    var putrefaction = new Putrefaction();
                    // 배율이 적용된 상태로 새로  세팅
                    putrefaction.SetPutrefaction(m_duration * m_magDuration, m_damage * m_magDamage, m_radius, m_magDamage, m_magDuration);
                    // 지속시간 뒤에 제거되도록 예약한다.
                    target.SetPutrefactionOver(putrefaction);
                    // 부패 추가
                    target.AddPuterefaction(putrefaction);
                    
                }
            }
        }
        public void PutrefactionOver()
        {
            m_putrefactionOver = true;
        }
	}
}
