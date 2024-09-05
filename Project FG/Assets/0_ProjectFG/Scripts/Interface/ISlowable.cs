using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
	public interface ISlowable
	{
        public float FinalSpeed(float speed);
        public void SetSlowSpeed(float speed);
	}
}
