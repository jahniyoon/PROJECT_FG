using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace JH
{
    public class HitEffect : MonoBehaviour
    {
        [Header("Target Mesh Renderer")]
        [SerializeField] private MeshRenderer[] m_meshRenderers;

        [Header ("Materials")]
        [SerializeField] private Material m_hitMat;
        [SerializeField] private Material m_dieMat;


        [Header("Setting")]
        [SerializeField] private float m_hitDuration = 0.1f;
        [SerializeField]private float m_pingPongSpeed = 1;

        private List<Material[]> m_originMats;
        private bool m_isDie = false;
        private Color m_hitColor;


        Coroutine m_hitRoutine;

        public void Hit()
        {
            if (m_isDie)
                return;

            SetMaterial(m_hitMat);

            if(m_hitRoutine != null)
            {
                StopCoroutine(m_hitRoutine);
                m_hitRoutine = null;
            }
            m_hitRoutine = StartCoroutine(HitRoutine());
        }
        IEnumerator HitRoutine()
        {
            float timer = 0;

            while(timer < m_hitDuration)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            ResetMaterial();

            yield break ;
        }


        public void Die()
        {
            m_isDie = true;
            StopAllCoroutines();
            SetMaterial(m_dieMat);
        }

        Coroutine pingPongRoutine;

        public void PingPong(bool enable = false)
        {
            if(pingPongRoutine != null)
            {
                StopCoroutine(pingPongRoutine);
                pingPongRoutine = null;
            }
            ResetMaterial();

            if(enable)
                pingPongRoutine = StartCoroutine(PingPongRoutine());


        }
        IEnumerator PingPongRoutine()
        {
            SetMaterial(m_hitMat);
            m_hitColor = m_hitMat.GetColor("_BaseColor");
            float curAlpha;
            float maxAlpha = m_hitColor.a;

            while (true)
            {
                curAlpha = Mathf.PingPong(Time.time * m_pingPongSpeed, maxAlpha);
                m_hitColor.a = curAlpha;
                foreach (var mesh in m_meshRenderers)
                {
                    for (int i = 1; i < mesh.materials.Length; i++)
                    {
                        mesh.materials[i].SetColor("_BaseColor", m_hitColor);
                    }
                }

                yield return null;
            }


            yield break;
        }



        public void SetMaterial(Material mat)
        {
            if (mat == null)
                return;

            ResetMaterial();

            foreach (var mesh in m_meshRenderers)
            {
                Material[] mats = new Material[mesh.materials.Length + 1];
                for (int i = 0; i < mesh.materials.Length; i++)
                {
                    mats[i] = mesh.materials[i];
                }
                mats[mesh.materials.Length] = mat;
                mesh.materials = mats;
            }

        }

        public void ResetMaterial()
        {
            if (m_originMats == null)
            {
                m_originMats = new List<Material[]>();
                for (int i = 0; i < m_meshRenderers.Length; i++)
                {
                    m_originMats.Add(m_meshRenderers[i].materials);
                }
            }

            for (int i = 0; i < m_meshRenderers.Length; i++)
            {
                m_meshRenderers[i].materials = m_originMats[i];
            }

        }
    }
}