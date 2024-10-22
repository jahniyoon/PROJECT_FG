using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Invincible Buff", menuName = "ScriptableObjects/Buff/Invincible")]

    public class InvincibleBuff : BuffBase
    {
        public InvincibleBuff(BuffData data):base(data) 
        {
            m_data = data;
        }
        public override float GetDuration()
        {
            return GetBuffValue();
        }
        public void Invincible(BuffHandler handler, bool enable)
        {
            if(handler.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.SetInvincible(enable);
        }

        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);
            handler.Status.AddInvincible(this);

        }
        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            handler.Status.RemoveInvincible(this);

        }

    }
}