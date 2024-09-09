using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{

    [System.Serializable]
    public class BuffElement
    {
        public int Stack = 1;
        public float Timer = 0;
        public bool isActive = false;
        public bool isBuff = false;
        public BuffBase Buff;

        // 켜지기
        public void SetActive(bool enable)
        {
            isActive = enable;
        }
        // 스택 올리기
        public void StackUp()
        {
            Timer = 0;
            Stack++;
        }
        // 스택 내리기
        public void StackDown()
        {
            Stack--;
        }
        // 스택 리셋
        public void ResetStack()
        {
            Timer = 0;
            Stack = 0;
        }
        public void UpdateTimer()
        {
            // 타이머 업데이트
            if (isBuff)
            {
                Timer += Time.deltaTime;
                isBuff = false;
            }
            // 타이머를 줄여준다.
            else
                Timer -= Time.deltaTime;
        }

    }
}
