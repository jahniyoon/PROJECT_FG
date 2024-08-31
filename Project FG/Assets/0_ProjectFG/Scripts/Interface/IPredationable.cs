using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public interface IPredationable 
    {
        public Transform Transform { get; }
        public bool CanPredation { get; }

        public void Predation();
    }
}