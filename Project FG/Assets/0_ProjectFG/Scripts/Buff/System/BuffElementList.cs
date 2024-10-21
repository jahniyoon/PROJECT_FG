using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    // 가지고 있는 버프들을 계산하는 클래스
    [System.Serializable]
	public class BuffElementList 
	{
        [SerializeField] private List<BuffElement> m_buffList = new List<BuffElement>();

        public List<BuffElement> BuffList => m_buffList;

        public int Count => m_buffList.Count;
        public bool Contains(int casterID)
        {
            foreach (BuffElement element in m_buffList)
            {
                if (element.CasterID == casterID)
                    return true; 
            }
            return false;
        }
        public void Clear()
        {
            m_buffList.Clear();
        }

        public void Add(BuffElement buff)
        {
            if (Contains(buff.CasterID))
                return;

            m_buffList.Add(buff);
        }
        public void Remove(BuffElement buff)
        {
            if(Contains(buff.CasterID))
                m_buffList.Remove(buff);
        }

        public BuffElement Buff(int casterID)
        {
            foreach (BuffElement element in m_buffList)
            {
                if(element.CasterID == casterID)
                    return element;
            }
            return null;
                
        }
	}
}
