using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public enum TargetTag
    {
        Enemy,
        Player,
        Caster
    }
    public enum SkillType
    {
        None,
        AimShootSkill,  // 특수 스킬
        AreaSkill,      // 범위 스킬
        CasterBuff,     // 캐스터 버프
        CollisionSkill, // 콜리전 스킬
        ProjectileSkill // 투사체 스킬


    }
    public enum BuffType
    {
        Immediately,    // 즉시
        TimeCondition,      // 조건
        Stack,           // 스택
        Stay,           // 계속 붙어있을 때 까지
        Attachment           // 뗄 때까지
    }

    // TODO : 정리 필요
    public enum AimType
    {
        None,
        MoveDirection,      //  이동 방향
        RandomEnemyDirection,    // 랜덤 적 방향
        PcPosition,             // PC 위치
        PcPositionSummon,        // PC 위치 소환
                                 //
        CasterPosition,     // 캐스터 위치
        CasterDirection,    // 캐스터 방향
        PointerDirection,   // 포인터 방향
        NearTargetDirection,      // 가까운 타겟 방향
        TargetDirection,      // 가까운 타겟 방향
        TargetPosition,      // 가까운 타겟 방향
        RandomTargetDirection,      // 가까운 타겟 방향
        RandomTargetPosition,      // 가까운 타겟 방향
        RandomDirection,     // 랜덤 방향


    }
    public enum FoodComboType
    {
        None,
        Triple,
        Quadruple,
        Quintuple,
        Hextuple,
        Septuple
    }
}
