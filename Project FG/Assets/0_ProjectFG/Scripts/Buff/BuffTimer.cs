using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class BuffTimer
    {

        [SerializeField] private float m_timer = 0f;
        [SerializeField] private bool m_running = false;


        public float Timer => m_timer;


        public void Start()
        {
            m_running = true;
            m_timer = 0;
        }
        public void Stop()
        {
            m_running = false;
        }

        public void Tick(float dt)
        {
            if (m_running == false)
                return;

            m_timer += dt;
        }
    }
}
