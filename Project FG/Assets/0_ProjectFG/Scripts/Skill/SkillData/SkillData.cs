using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    // 스킬과 관련된 클래스
    [System.Serializable]
    public class SkillData : ScriptableObject
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string SkillName { get; protected set; }
        [field: SerializeField][field: TextArea] public string Description { get; protected set; }
        [field: Header("Skill")]
        [field: SerializeField] public float SkillDuration { get; protected set; }
        [field: SerializeField] public float SkillCoolDown { get; protected set; }

        // 스킬 사용이 가능한지 체크를 한다.
        public virtual bool CanActiveSkill(bool enable = true)
        {
            return enable;
        }

        // 스킬을 사용한다.
        public virtual void ActiveSkill() { }

        public virtual void InActiveSkill() { }


    }
}
