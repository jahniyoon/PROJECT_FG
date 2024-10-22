using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public interface ISkillCaster
	{
        public bool CanActiveSkill();
        public void UpdateSkillTimer(float timer);
        public FSMState State { get; }
        public Transform Transform { get; }
        public Transform Model { get; }
        public GameObject GameObject { get; }
        public List<SkillBase> Skills { get; }
        public float FinalDamage(float damage, DamageType type);
	}

    
}
