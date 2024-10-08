using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    public class DonutShader : MonoBehaviour
    {

        private MeshRenderer m_renderer;
        private GameObject m_effect;

        private void Awake()
        {
            m_renderer = GetComponentInChildren<MeshRenderer>();
            m_effect = transform.GetChild(0).gameObject;
            SetActive(false);
        }

        public void SetActive(bool enable)
        {
            m_effect.SetActive(enable);
        }

        public void SetRadius(float outer, float inner)
        {
            transform.localScale = Vector3.one * outer;
            m_renderer.materials[0].SetFloat("_InnerScale", inner / outer);
            m_renderer.materials[0].SetFloat("_SliderScale", 0);

        }

        public void SetColor(Color outerColor, Color sliderColor)
        {
            m_renderer.materials[0].SetColor("_MainColor", outerColor);
            m_renderer.materials[0].SetColor("_SliderColor", sliderColor);
        }

        public void SetSlider(float ratio)
        {
            m_renderer.materials[0].SetFloat("_SliderScale", ratio);
        }

    }
}