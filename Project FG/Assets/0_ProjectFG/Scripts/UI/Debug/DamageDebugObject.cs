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

        [SerializeField] private RectTransform CanvasRect;

        private void Awake()
        {
            m_textMeshPro = GetComponentInChildren<TMP_Text>();
            m_rectTransform = GetComponent<RectTransform>();
            CanvasRect = transform.root.GetComponent<RectTransform>();
        }
        public void SetPosition(Vector3 position)
        {
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(position);
            Vector2 WordObjectToScreenPosition = new Vector2(
                ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)));

            m_rectTransform.anchoredPosition = WordObjectToScreenPosition;
        }
        public void OnDamage(float value, float duration, Vector3 position)
        {
            if (m_textMeshPro == null)
                return;

            SetPosition(position);
            m_textMeshPro.text = value.ToString();

            Invoke(nameof(DamageEnd), duration);
        }


        public void DamageEnd()
        {
            this.gameObject.SetActive(false);
        }

    }
}
