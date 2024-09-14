using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    // 데미지 무시 관련 버프
    [CreateAssetMenu(fileName = "Damage Reduction Buff", menuName = "ScriptableObjects/Buff/Damage Reduction")]
    public class DamageReductionBuff : BuffBase
	{
        [Header("Damage Reduction Buff")]
        [SerializeField] float m_damageReduction; // 영향을 줄 데미지

        // TODO : 이 부분 꼭 추후 교체해야함!!!
        public void SetValue(float value)
        {
            m_damageReduction = value;
        }
        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            if(handler.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.SetDamageReduction(m_damageReduction);
        }
        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            if (handler.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.SetDamageReduction(m_damageReduction * -1);
        }
     
    }
}
