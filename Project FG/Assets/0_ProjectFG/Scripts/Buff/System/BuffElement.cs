using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class BuffElement
    {
        public BuffElement() { }
        public BuffElement(BuffElement buff) 
        {
            this.name = buff.name;
            this.CasterID = buff.CasterID;
            this.Buff = buff.Buff;
            this.isActive = buff.isActive;
            this.Stack = buff.Stack;
            this.Coroutine = buff.Coroutine;
        }

        [HideInInspector] public string name;
        public int CasterID;
        public BuffBase Buff;
        public bool isActive = false;
        public int Stack;

        public Coroutine Coroutine;
        public float Duration => Buff.GetDuration();

        public void SetBuff(int id, BuffBase buff)
        {
            CasterID = id;
            Buff = buff;
            name = $"{buff.ID}  {buff.Data.Name}";
        }
        // 켜지기
        public void SetActive(bool enable)
        {
            isActive = enable;
        }
        public void SetStack(int stack)
        {
            Stack = stack;
        }
        public void StackUp()
        {
            Stack++;
        }
        public void StackDown()
        {
            Stack--;
        }
    
        // 버프를 비교한다.
        public void ComparisonBuff(BuffBase buff)
        {
            // 타입이 같아야한다.
            if (Buff.Type != buff.Type)
            {
                return;
            }

            Buff = Buff.ComparisonBuff(buff);
        }


    }
}
