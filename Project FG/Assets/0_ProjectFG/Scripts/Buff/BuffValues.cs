using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    [System.Serializable]
	public class BuffValues 
	{
        public float[] Values = new float[0];       
        public int Length => Values.Length;

        public float[] GetValue(int index)
        {
            return Values;
        }

        public BuffValues(float[] Values)
        {
            this.Values = Values;   
        }
	}
}
