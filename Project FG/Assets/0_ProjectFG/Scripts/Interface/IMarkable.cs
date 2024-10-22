using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public interface IMarkable 
	{
        public void SetCaster(ISkillCaster caster);
        public void OnMarkStack(float markDamage, float duration, int maxStack);
        public void StackDown();
	}
}
