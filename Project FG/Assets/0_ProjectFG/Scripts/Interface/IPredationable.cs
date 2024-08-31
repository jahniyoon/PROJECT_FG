using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public interface IPredationable 
    {
        public bool CanPredationable { get; }

        public void Predationable();
    }
}