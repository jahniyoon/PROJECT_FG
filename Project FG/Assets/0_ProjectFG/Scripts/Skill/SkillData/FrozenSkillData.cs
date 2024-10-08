using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [CreateAssetMenu(fileName = "Frozen Skill", menuName = "ScriptableObjects/Skill/Frozen Skill")]

    public class FrozenSkillData : SkillData
	{

        [field: Header("Frozen Skill")]

        [field: SerializeField] public TargetTag Target { get; private set; }
        [field: SerializeField] public GameObject FrozenPrefab { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: Header("Slow Debuff")]
        [field: SerializeField] public BuffData SlowDebuff { get; private set; }
        [field: Header("Dot Damage")]
        [field: SerializeField] public BuffData DotDamageBuff { get; private set; }
        [field: Header("Dot Damage")]
        [field: SerializeField] public BuffData FrozenDebuff { get; private set; }

    }
}
