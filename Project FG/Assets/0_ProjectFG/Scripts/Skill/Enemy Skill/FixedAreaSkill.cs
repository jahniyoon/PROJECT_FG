using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    // 정지가 필요한 스킬
	public class FixedAreaSkill : AreaSkill
	{

        protected override void ActiveArea()
        {
            base.ActiveArea();
            SetSkillFix();
        }

    }
}
