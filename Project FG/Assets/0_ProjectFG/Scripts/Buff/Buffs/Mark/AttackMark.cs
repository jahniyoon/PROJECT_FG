using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class AttackMark : MonoBehaviour, IMarkable
    {
        [SerializeField] List<float> m_markDamage;
        int m_maxStack;

        Coroutine m_durationCoroutine;
        ISkillCaster m_caster;
        public int Stack => m_markDamage.Count;
        
        public void SetCaster(ISkillCaster caster)
        {
            m_caster = caster;
        }
        public void OnMarkStack(float markDamage, float duration, int maxStack)
        {
            m_maxStack = maxStack;

            // 스택이 꽉찼으면 첫번째 제거
            if (maxStack <= Stack)            
                m_markDamage.RemoveAt(0);
            
            // 새로운 표식 추가
            m_markDamage.Add(markDamage);

            if (m_durationCoroutine != null)
            {
                StopCoroutine(m_durationCoroutine);
                m_durationCoroutine = null;
            }
            m_durationCoroutine = StartCoroutine(DurationRoutine(duration));
        }

        IEnumerator DurationRoutine(float duration)
        {
            float timer = 0;
            while (timer < duration)
            {
                timer += Time.deltaTime;
                yield return null;
            }

            StackOver();
            yield break;
        }
        // 스택 소모
        public void StackDown()
        {
            if (Stack == 0) return;

            if (transform.TryGetComponent<Damageable>(out Damageable damageable))
                damageable.OnDamage(m_caster.FinalDamage(m_markDamage[0], DamageType.Default));
            m_markDamage.RemoveAt(0);
        }

        // 스택을 끝낸다.
        public void StackOver()
        {
            m_markDamage.Clear();
        }
    }
}
