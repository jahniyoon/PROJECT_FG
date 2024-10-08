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
	}

    public interface IAimSkillCaster
    {
        public AimState AimState { get; }
    }
}
