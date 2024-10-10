using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class FoodPowerShieldSkill : FoodPowerSkill
    {
        [Header("실드 이펙트")]
        [SerializeField] private GameObject m_shieldEffect;
        [SerializeField] private ParticleSystem m_knockbackEffect;

        [SerializeField] BuffBase m_shieldBuff;
        [SerializeField] BuffBase m_knockBackBuff;


        protected override void Init()
        {
            base.Init();

            //  피해감소 버프 가져오기
            m_shieldBuff = GFunc.TryGetBuff(Data.TryGetBuffID(0));

            // 넉백 이벤트 연결
            if (Caster.Transform.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.DamageEvent.AddListener(DamageEvent);

            // 넉백 버프 가져오기
            m_knockBackBuff = GFunc.TryGetBuff(Data.TryGetBuffID(1));

        }

      
        // 레벨데이터가 변경될 때 호출되는 메서드
        protected override void LevelDataChange()
        {
            base.LevelDataChange();
            if (m_shieldBuff != null)
            {
                float[] damageBuffValue = new float[1] { LevelData.TryGetBuffValue() };
                m_shieldBuff.SetBuffValue(damageBuffValue);
            }

            if (m_knockBackBuff != null)
            {
                m_knockBackBuff.SetCaster(transform);
                float[] knockBackBuffValue = new float[2] { LevelData.TryGetBuffValue(1), LevelData.TryGetBuffValue(2) };
                m_knockBackBuff.SetBuffValue(knockBackBuffValue);
            }


            // 피해감소 버프 활성화
            if (Caster.Transform.TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
            {
                buffHandler.RemoveBuff(Caster.GameObject, m_shieldBuff);
                buffHandler.OnBuff(Caster.GameObject, m_shieldBuff);
            }
        }

        public override void ActiveSkill()
        {
            base.ActiveSkill();
            m_shieldEffect.gameObject.SetActive(true);
        }


        // 캐스터가 데미지 입혀질 경우
        public void DamageEvent()
        {
            if (State == SkillState.Reloading)
                return;


            m_knockbackEffect.Stop();
            m_knockbackEffect.transform.parent.localScale = Vector3.one * LevelData.Radius;
            m_knockbackEffect.Play();

            Collider[] colls = Physics.OverlapSphere(transform.position, LevelData.Radius, Data.TargetLayer, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].CompareTag(Data.SkillTarget.ToString()) == false)
                    continue;

                if (colls[i].TryGetComponent<Damageable>(out Damageable damageable))
                    damageable.OnDamage(LevelData.Damage);

                    if (colls[i].TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
                    buffHandler.OnBuff(Caster.GameObject, m_knockBackBuff);
            }
            InactiveSkill();

        }


        public override void InactiveSkill()
        {
            base.InactiveSkill();

            StopEffect();

        }

        // 비활성화되면 리스너를 모두 제거한다.
        private void OnDisable()
        {
            // 피해감소 버프 비활성화
            if (Caster.GameObject.TryGetComponent<BuffHandler>(out BuffHandler buffHandler))
                buffHandler.RemoveBuff(Caster.GameObject, m_shieldBuff);

            if (Caster.Transform.TryGetComponent<Damageable>(out Damageable damageable))
            {
                damageable.DamageEvent.RemoveListener(DamageEvent);
            }
        }


    }
}
