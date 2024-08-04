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

        private List<Material[]> m_originMats;
        private bool m_isDie = false;

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