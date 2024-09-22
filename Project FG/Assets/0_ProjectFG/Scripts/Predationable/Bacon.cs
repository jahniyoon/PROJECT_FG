using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class Bacon : PredationableObject
    {
        [SerializeField] private bool m_notCount;
        [Header("Minimap Color")]
        [SerializeField] protected Color m_minimapColor = Color.green;
        int m_instanceID;
        private void Start()
        {
            m_instanceID = this.gameObject.GetInstanceID();
            UIManager.Instance.MinimapUI.AddObject(m_instanceID, m_minimapColor, 3);
        }
        private void Update()
        {
            UIManager.Instance.MinimapUI.SetPosition(m_instanceID, this.transform.position);

        }
        private void OnDisable()
        {
            UIManager.Instance.MinimapUI.RemoveObject(m_instanceID);
        }
    }
}