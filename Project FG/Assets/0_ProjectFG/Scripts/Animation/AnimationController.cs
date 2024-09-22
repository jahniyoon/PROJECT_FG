using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace JH
{
    public class AnimationController : MonoBehaviour
    {
        private Animator[] m_animator;

        private void Awake()
        {
            m_animator = GetComponentsInChildren<Animator>();

        }

        public void Rebind()
        {
            foreach (var item in m_animator)
                item.Rebind();
        }

        public void SetBool(AnimationID parameter, bool enable)
        {
            foreach (var item in m_animator)
                item.SetBool(parameter.ToString(), enable);
        }
        public void SetTrigger(AnimationID parameter)
        {
            foreach (var item in m_animator)
                item.SetTrigger(parameter.ToString());
        }

        public void SetLayer(string layerName, float value)
        {
            foreach (var item in m_animator)
                item.SetLayerWeight(item.GetLayerIndex(layerName), value);
        }

    }


    

   



    public enum AnimationID
    {
        isMove,
        isHit,
        isDie,
        isAttack,
        isPredation
    }
}