using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
namespace JH
{
	public class DamageDebugController : MonoBehaviour
	{

        public DamageDebugObject debugObj;
        private int curCount = 0;
        [Header("Damage Debug Settings")]
        public int m_poolCount;
        public float m_duration;
        public DamageDebugObject[] m_damageList;
        Transform m_target;

        private void Awake()
        {
            SetDamage();
        }

        private void SetDamage()
        {
            m_damageList = new DamageDebugObject[m_poolCount];

            for (int i = 0; i < m_poolCount; i++)
            {
                var damageObj = Instantiate(debugObj, this.transform);
                m_damageList[i] = damageObj;
            }
        }

        public void OnDamage(float value, Transform position)
        {
            m_damageList[curCount].gameObject.SetActive(true);
            m_damageList[curCount].OnDamage(value, m_duration, position);

            curCount++;
            if(m_poolCount <= curCount)
                curCount = 0;

        }


	
	}

    
}
