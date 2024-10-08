using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public static class BuffFactory
    {
         
        public static BuffBase CreateBuff(BuffData data)
        {
            if (data == null)
            {
                Debug.Log("버프 데이터가 없습니다.");
                return null;
            }

            BuffBase buff;
            switch (data.Type)
            {
                case BuffType.KnockBack:
                    return buff = new KnockbackBuff(data);

                case BuffType.HitDamageIncrease:
                        return buff = new DamageReductionBuff(data);


                default:
                    return buff = new BuffBase(data);
            }

        }
    }
}
