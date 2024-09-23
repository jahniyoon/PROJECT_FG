using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class Bacon : PredationableObject
    {
        [SerializeField] private GameSettings m_gameSettings;
        [SerializeField] private bool m_notCount;
        [SerializeField] private Animator m_animator;

        [Header("Minimap Color")]
        [SerializeField] protected Color m_minimapColor = Color.green;
        int m_instanceID;
        private void Start()
        {
            m_animator = transform.GetComponent<Animator>();
            m_instanceID = this.gameObject.GetInstanceID();
            UIManager.Instance.MinimapUI.AddObject(m_instanceID, m_minimapColor, 3);
        }
        private void Update()
        {
            UIManager.Instance.MinimapUI.SetPosition(m_instanceID, this.transform.position);

        }
        private void OnDisable()
        {
            //UIManager.Instance.MinimapUI.RemoveObject(m_instanceID);
        }

        public override void Predation()
        {

            m_canPredationable = false;
            m_icon.gameObject.SetActive(false);
            UIManager.Instance.MinimapUI.RemoveObject(m_instanceID);

            SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            if (spriteRenderer != null)
                spriteRenderer.sortingOrder = 6;

            m_animator?.SetTrigger("isPredation");

            Destroy(gameObject, 1.5f);
        }
    }
}