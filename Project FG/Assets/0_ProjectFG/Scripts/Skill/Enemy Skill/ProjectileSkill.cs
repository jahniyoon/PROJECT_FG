using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace JH
{
    public class ProjectileSkill : SkillBase
    {


        public override void ActiveSkill()
        {
            base.ActiveSkill();
            ShootProjectiles();
        }

        private void ShootProjectiles()
        {
            for (int i = 0; i < m_projectiles.Count; i++)
            {
                CreateProjectile(m_projectiles[i]);
            }
        }

  
        //private void CreateProjectile(ProjectileBase projectile)
        //{
        //    Transform parent = GameManager.Instance.ProjectileParent;
        //    Vector3 position = Model.position;
        //    Quaternion rotation = Model.rotation;

        //    Vector3 targetPos = transform.position;
        //    if (m_skillTarget)
        //    {
        //        targetPos = m_skillTarget.transform.position;
        //        targetPos.y = Model.position.y;
        //    }

        //    switch (m_data.AimType)
        //    {
        //        case AimType.Caster:
        //            parent = Model;
        //            break;

        //        case AimType.NearTargetDirection:
        //            if (m_skillTarget)
        //                rotation.SetLookRotation(targetPos - position);
        //            break;

        //        case AimType.TargetDirection:
        //            if (m_skillTarget)
        //                rotation.SetLookRotation(targetPos - position);
        //            break;

        //        case AimType.TargetPosition:
        //            position = m_skillTarget.position;
        //            break;

        //        case AimType.PointerDirection:
        //            rotation.SetLookRotation(GameManager.Instance.Aim.position - transform.position);
        //            break;
        //        default: break;
        //    }

        //    position.y = Model.position.y;
        //    var cloneProjectile = Instantiate(projectile.gameObject, position, rotation, parent).GetComponent<ProjectileBase>();
        //    cloneProjectile.SetSkill(this);
        //}


    }
}
