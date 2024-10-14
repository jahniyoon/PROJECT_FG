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
        FixedAreaSkill,      // 범위 스킬
        Buff,     // 캐스터 버프
        DonutSkill,
        CollisionSkill, // 콜리전 스킬
        ProjectileSkill, // 투사체 스킬
        ExplosionSkill,


        // 푸드파워 타입
        FoodPowerA,
        FoodPowerB,
        FoodPowerC,
        FoodPowerD,
        FoodPowerE,
        FoodPowerF,
        FoodPowerG,
        FoodPowerH,
    }
    public enum SkillActiveTime
    {
        Active, // 즉시 실행
        CoolDown,    // 쿨다운 이후
        ActiveReset,
        CoolDownReset
    }
    public enum BuffType
    {
        None,
        Immediately,    // 즉시
        TimeCondition,      // 조건
        Stack,           // 스택
        Stay,           // 계속 붙어있을 때 까지
        Attachment,           // 뗄 때까지

        Frozen, //냉기
        Burn,   // 화상
        Putrefaction, // 부패
        Heal,       // 힐
        AreaHeal,       // 범위 힐
        HitDamageIncrease, // 받는 피해 증감
        HitDamageDecrease, // 받는 피해 증감
        AttackDamageIncrease, //주는 피해 증감
        AttackDamageDecrease, //주는 피해 증감
        FastSpeed,  // 이속 증가
        SlowSpeed,   // 이속 감소
        Stun,       // 스턴
        KnockBack,  // 넉백
        Fear,       // 공포
        Invincible, //  무적
        Mark,       // 표식
        PredationreStun,    // 포식 스턴
        PredationParrying,  // 포식 패링
        ParryingStun        // 패링 스턴

    }
    public enum BuffEffectCondition
    {
        None,
        Duration,
        Area,
        Mark
    }

    public enum StackType
    {

    }

    public enum SkillState
    {
        Reloading,  // 재장전 중
        Ready,      // 준비 완료. 활성화 할 수 있음
        Cast,       // 캐스트 완료.
        Active,     // 활성화
        Disable,    // 비활성화
        Freeze      // 멈춤
    }
    public enum ProjectileType
    {
        None,
        Collision,
        Projectile,
        Donut,
        AOE,
        Explosion,
        HitScan,
        Grenade,
        Mine

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
        Caster,             // 캐스터 자체에 스폰
        CasterPosition,     // 캐스터 위치
        CasterDropPosition,     // 캐스터 위치
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

    public enum FoodPowerType
    {
        FoodPowerA,
        FoodPowerB,
        FoodPowerC,
        FoodPowerD,
        FoodPowerE,
        FoodPowerF,
        FoodPowerG,
        FoodPowerH,
    }
    public enum FSMState
    {
        Freeze,       // 멈춤
        Idle,       // 대기
        Move,       // 이동
        Attack,     // 공격
        Groggy,     // 그로기
        Predation,  // 포식
        Hit,        // 피격
        Die         // 사망
    }


    public enum AimState
    {
        Idle,
        Aim,
        Shoot,
        Reload
    }
    public enum EnemyType
    {
        Default,
        Suicide,
        CantAttackOnBuff,
        AimAndShoot,
        Tower,




    }
}
