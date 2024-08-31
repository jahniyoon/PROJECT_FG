using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class PredationableObject : MonoBehaviour, IPredationable
    {
        [SerializeField] private bool m_canPredationable;
        public bool CanPredationable => m_canPredationable;
        // Start is called before the first frame update
        void Start()
        {

        }

        public void Predationable()
        {

        }



    }
}