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
        // 스킬이 사용한 조건을 체크한다.
        protected override bool CheckCondition()
        {
            // 캐스터가 없으면 패스
            if (Caster == null) return false;

            // 준비가 되어있지 않으면 패스
            if (State != SkillState.Ready)
                return false;

            // 루틴 스킬이면 캐스터 체크를 안해도 된다.
            if (m_routine)
                return true;


            // 캐스터의 상태조건을 체크하고, 쿨타임이 되어야한다.
            return Caster.State == FSMState.Freeze || Caster.State == FSMState.Attack;
        }

    }
}
