using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace JH
{
    public class AnimationController : MonoBehaviour
    {
        private Animator m_animator;

        private void Awake()
        {
            m_animator = GetComponentInChildren<Animator>();

        }

        public void Rebind()
        {
            m_animator.Rebind();
        }

        public void SetBool(AnimationID parameter, bool enable)
        {
            m_animator.SetBool(parameter.ToString(), enable);
        }
        public void SetTrigger(AnimationID parameter)
        {
            m_animator.SetTrigger(parameter.ToString());
        }

        public void SetLayer(string layerName, float value)
        {
            m_animator.SetLayerWeight(m_animator.GetLayerIndex(layerName), value);
        }

    }


    

   



    public enum AnimationID
    {
        isMove,
        isHit,
        isDie,
        isAttack
    }
}