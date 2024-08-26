using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class RingEffect : MonoBehaviour
    {
        [Header("Ring Object")]
        [SerializeField] private GameObject m_outterObj;
        [SerializeField] private GameObject m_innerObj;
        [SerializeField] private GameObject m_sliderObj;

        [Header("Ring Scale")]
        [SerializeField] private float m_outerRadius;
        [SerializeField] private float m_innerRadius;
        [Range(0,1)]
        [SerializeField] private float m_sliderRatio;


        [Header("DEBUG")]
        [SerializeField] private bool m_showSceneSetting = false;

        public void SetRadius(float outer, float inner)
        {
            m_outerRadius = outer;
            m_innerRadius = inner;
            m_outterObj.transform.localScale = Vector3.one * m_outerRadius;
            m_innerObj.transform.localScale = Vector3.one * m_innerRadius;
        }
        public void SetSlider(float value)
        {
            m_sliderRatio = value;
            float sliderRadius = m_innerRadius + (m_outerRadius - m_innerRadius) * m_sliderRatio;
            m_sliderObj.transform.localScale = Vector3.one * sliderRadius;
        }

        public void SetColor(Color outer, Color slider)
        {
            m_outterObj.GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", outer);
            m_sliderObj.GetComponent<MeshRenderer>().materials[0].SetColor("_BaseColor", slider);
        }

        private void OnDrawGizmos()
        {
            if (m_showSceneSetting == false)
                return;
            SetRadius(m_outerRadius, m_innerRadius);
            SetSlider(m_sliderRatio);
        }

    }
}