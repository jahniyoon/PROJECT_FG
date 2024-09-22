using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace JH
{
	public class MinimapUI : MonoBehaviour
	{
        [SerializeField] private Vector2 m_standardPosition = Vector2.zero;
        [SerializeField] private Vector2 m_uiSize;
        [SerializeField] private Vector2 m_areaSize;
        [SerializeField] private Dictionary<int, RectTransform> m_objectPositions = new Dictionary<int, RectTransform>();
        [SerializeField] private GameObject m_object;

        private void Awake()
        {
           
        }

        public void SetAreaSize(Vector2 areaSize)
        {
            m_areaSize = areaSize;
        }
        public void AddObject(int id, Color color = default, int zDepth = 0)
        { 
            if(color == default)
                color = Color.yellow;


            GameObject newObj = Instantiate(m_object, m_object.transform.parent);
            newObj.GetComponent<Image>().color = color;
            newObj.SetActive(true);

            m_objectPositions.Add(id, newObj.GetComponent<RectTransform>());

            Vector3 position = Vector3.zero;
            position.z = zDepth;
            m_objectPositions[id].anchoredPosition = position;
        }
        public void RemoveObject(int id)
        {
            if (m_objectPositions.ContainsKey(id) == false)
                return;
            Destroy(m_objectPositions[id].gameObject);
            m_objectPositions.Remove(id);
        }

        public void SetPosition(int id, Vector3 position)
        {
            if (m_objectPositions.ContainsKey(id) == false)
                return;
            Vector3 anchred = m_objectPositions[id].anchoredPosition;

            anchred.x = m_uiSize.x / m_areaSize.x * position.x;
            anchred.y = m_uiSize.y / m_areaSize.y * position.z;
            m_objectPositions[id].anchoredPosition = anchred;
        }


	}
}
