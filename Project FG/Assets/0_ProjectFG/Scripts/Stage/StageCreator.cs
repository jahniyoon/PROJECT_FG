using JH;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace JH
{
    public class StageCreator : MonoBehaviour
    {

         [SerializeField] private NavMeshSurface m_navMeshSurface;


        public void CreateStage(Vector2 stageSize)
        {
            Vector3 size = Vector3.one;
            size.x = stageSize.x * 0.1f;
            size.z = stageSize.y * 0.1f;

            m_navMeshSurface.transform.localScale = size;
            m_navMeshSurface.BuildNavMesh();
        }

    }
}