using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JH
{
    public class DamageDebugObject : MonoBehaviour
    {
        private TMP_Text m_textMeshPro;
        private RectTransform m_rectTransform;
        private Transform m_target;

        [SerializeField] private RectTransform CanvasRect;

        private void Awake()
        {
            m_textMeshPro = GetComponentInChildren<TMP_Text>();
            m_rectTransform = GetComponent<RectTransform>();
            CanvasRect = transform.root.GetComponent<RectTransform>();
        }

        private void Update()
        {
            if (m_target != null)
                SetPosition(m_target.position);
        }
        public void SetPosition(Vector3 position)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(position);
            Vector2 WordObjectToScreenPosition = new Vector2(
                ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            m_rectTransform.anchoredPosition = WordObjectToScreenPosition;
        }
        public void OnDamage(float value, float duration, Transform position, Color textColor = default)
        {
            if (m_textMeshPro == null)
                return;


            m_target = position;
            m_textMeshPro.text = value.ToString();

            if(textColor == default)
                textColor = Color.white;
            m_textMeshPro.color = textColor;



            Invoke(nameof(DamageEnd), duration);
        }


        public void DamageEnd()
        {
            this.gameObject.SetActive(false);
        }

    }
}
