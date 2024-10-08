using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    // 도트 딜/힐 버프
    [CreateAssetMenu(fileName = "DotDamageBuff", menuName = "ScriptableObjects/Buff/Dot Damage")]
    public class DotDamageBuff : BuffBase
    {
        public DotDamageBuff(BuffData data) : base(data) 
        { 
            m_data = data;
        }

        [Header("Dot Damage Buff")]
        [SerializeField] float m_damage; // 영향을 줄 데미지
        [SerializeField] float m_damageDuration; // 데미지 간격

        // 버프 실행시 타겟 타이머를 정해준다.
        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if (handler.TryGetComponent<Damageable>(out Damageable damage))
            {
                // 숫자가 0보다 낮으면 데미지
                if (m_damage < 0)
                    damage.OnDamage(m_damage * -1);
                // 이외에는 회복
                else
                    damage.RestoreHealth(m_damage);
            }
        }

        // 액티브가 가능한지 체크한다.
        public override bool CanActive(float timer)
        {
            bool canActive = m_damageDuration < timer;
            return canActive;
        }

    


    }
}
