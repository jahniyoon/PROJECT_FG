using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class AimShader : MonoBehaviour
    {
        private MeshRenderer m_renderer;

        private void Awake()
        {
            m_renderer = GetComponentInChildren<MeshRenderer>();
        }

        public void SetRadius(float radius, float angle)
        {
            transform.localScale = Vector3.one * radius;
            m_renderer.materials[0].SetFloat("_SliderScale", 0);
            m_renderer.materials[0].SetFloat("_Angle", angle);
        }
        public void SetColor(Color radiusColor, Color sliderColor)
        {
            m_renderer.materials[0].SetColor("_MainColor", radiusColor);
            m_renderer.materials[0].SetColor("_SliderColor", sliderColor);
        }
        public void SetSlider(float ratio)
        {
            m_renderer.materials[0].SetFloat("_SliderScale", ratio);
        }
    }
}