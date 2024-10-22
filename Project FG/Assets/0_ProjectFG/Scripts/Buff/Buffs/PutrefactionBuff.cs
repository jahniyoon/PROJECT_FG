using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JH
{
    // 부패 관련 버프
    [CreateAssetMenu(fileName = "Putrefaction Buff", menuName = "ScriptableObjects/Buff/Putrefaction Buff")]
    public class PutrefactionBuff : BuffBase
    {

        private Putrefaction m_putrefaction;
        public PutrefactionBuff(BuffData data) : base(data) 
        {
            m_data = data;
        }
        public override float GetDuration()
        {
            return GetBuffValue(1);
        }


        public override void ActiveBuff(BuffHandler handler)
        {
            base.ActiveBuff(handler);

            if(handler.TryGetComponent<IPutrefaction>(out IPutrefaction putrefaction))
            {
                // 부패 세팅하고
                m_putrefaction = new Putrefaction();
                m_putrefaction.SetPutrefaction(GetBuffValue(1), GetBuffValue(0), GetValue1(0), GetValue1(1), GetValue1(2), GetValue1(3));
                // 부패 추가
                putrefaction.AddPuterefaction(m_putrefaction);
            }

        }

        public override void InactiveBuff(BuffHandler handler)
        {
            base.InactiveBuff(handler);
            if (handler.TryGetComponent<IPutrefaction>(out IPutrefaction putrefaction))
            {
                handler.Status.RemovePutrefactionBuff(m_putrefaction);
            }
        }


    }
}