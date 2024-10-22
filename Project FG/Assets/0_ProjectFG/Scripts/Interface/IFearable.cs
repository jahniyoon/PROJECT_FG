using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public interface IFearable 
	{
        public void OnFear(Vector3 fearPos, float escapeSpeed, float duration,float PredationHealthRatio);
	}
}
