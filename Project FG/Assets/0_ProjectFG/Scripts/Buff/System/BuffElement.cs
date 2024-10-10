using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{

    [System.Serializable]
    public class BuffElement
    {
        public int ID;
        public int Stack = 0;
        public float Timer = 0;
        public bool isOneAndOnlyEnable; // 원앤온리일 경우 켜져있는지 확인
        public bool isActive = false;
        public bool isStay = false;
        public BuffBase Buff;
        public Coroutine Coroutine;

  
        // 켜지기
        public void SetActive(bool enable)
        {
            //DebugProjectile.Log("켠다");
            isActive = enable;
        }
        // 스택 올리기
        public void StackUp()
        {
            Timer = 0;
            Stack++;
            //DebugProjectile.Log(ID + " 스택 업 : " + Stack);
        }
        // 스택 내리기
        public void StackDown()
        {
            Stack--;
            //DebugProjectile.Log(ID + " 스택 다운 : " + Stack);
        }
        // 스택 리셋
        public void ResetStack()
        {
            //DebugProjectile.Log(ID + " 스택 리셋");
            Timer = 0;
            Stack = 0;
            isActive = false;
        }

        public void MainTain(BuffElement buff)
        {
            //DebugProjectile.Log("계승");
            this.Stack = buff.Stack;
            this.Timer = buff.Timer;
            this.isActive = buff.isActive;
        }
        public void UpdateTimer()
        {
            // 타이머 업데이트
            if (isStay)
            {
                Timer += Time.deltaTime;
                isStay = false;
            }
            // 타이머를 줄여준다.
            else
                Timer -= Time.deltaTime;
        }

    }
}
