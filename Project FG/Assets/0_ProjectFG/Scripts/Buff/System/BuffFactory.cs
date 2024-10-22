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

                case BuffType.Frozen:
                    return buff = new FrozenBuff(data);

                case BuffType.Heal:
                    return buff = new HealBuff(data);

                case BuffType.Burn:
                    return buff = new BurnBuff(data);

     

                case BuffType.HitDamageDecrease:
                        return buff = new HitDamageDecrease(data);

                case BuffType.HitDamageIncrease:
                    return buff = new HitDamageIncrease(data);

                case BuffType.AttackDamageIncrease:
                    return buff = new AttackDamageIncrease(data);

                case BuffType.AttackDamageDecrease:
                    return buff = new AttackDamageDecrease(data);

                case BuffType.SlowSpeed:
                    return buff = new SlowDebuff(data);

                case BuffType.FastSpeed:
                    return buff = new FastSpeedBuff(data);



                case BuffType.KnockBack:
                    return buff = new KnockbackBuff(data);

                case BuffType.Stun:
                    return buff = new StunBuff(data);

                case BuffType.PredationreStun:
                    return buff = new StunBuff(data);

                case BuffType.Invincible:
                    return buff = new InvincibleBuff(data);


                default:
                    return buff = new BuffBase(data);
            }

        }
    }
}
