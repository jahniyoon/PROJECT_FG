using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class AimShader : MonoBehaviour
    {
        private MeshRenderer m_renderer;
        private Vector3 rotation;

        private void Awake()
        {
            m_renderer = GetComponentInChildren<MeshRenderer>();
        }

        public void SetRadius(float radius, float angle)
        {
            if(m_renderer ==null)
                m_renderer = GetComponentInChildren<MeshRenderer>();

            transform.localScale = Vector3.one * radius;
            m_renderer.materials[0].SetFloat("_SliderScale", 0);
            m_renderer.materials[0].SetFloat("_Angle", angle);
            rotation.y = angle / 2 * -1;
            transform.localEulerAngles = rotation;
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