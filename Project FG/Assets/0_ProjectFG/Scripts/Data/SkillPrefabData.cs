using Google.GData.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using static Cinemachine.DocumentationSortingAttribute;


namespace JH
{
    [CreateAssetMenu(fileName = "Prefab Data", menuName = "ScriptableObjects/Prefab Data")]

    public class PrefabData : ScriptableObject
    {
        [field: Header("스킬 프리팹 데이터")]
        public List<SkillBase> m_skillPrefabs = new List<SkillBase>();
        [field: Header("투사체 프리팹 데이터")]

        public List<ProjectileBase> m_projectilePrefabs = new List<ProjectileBase>();

        public SkillBase TryGetSkill(int skillID)
        {
            foreach(var skill in m_skillPrefabs)
            {
                if(skill.ID == skillID)
                    return skill;
            }

            Debug.Log(skillID + " 스킬을 찾을 수 없습니다.");
            return null;
        }
        public ProjectileBase TryGetProjectile(int projectileID)
        {
            foreach (var projectile in m_projectilePrefabs)
            {
                if (projectile.ID == projectileID)
                    return projectile;
            }

            Debug.Log(projectileID + " 투사체를 찾을 수 없습니다.");
            return null;
        }

    }


}