using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PredationableObject : MonoBehaviour, IPredationable
    {
        [SerializeField] private bool m_canPredationable;
        [SerializeField] private WorldSpaceIcon m_icon;
        public bool CanPredation => m_canPredationable;

        public Transform Transform => this.transform;

        private void Awake()
        {
            
        }
        public void Predation()
        {
            m_canPredationable = false;
            m_icon.enabled = false;
            Destroy(gameObject);
        }



    }
}