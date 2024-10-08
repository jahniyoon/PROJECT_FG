using Google.GData.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using static Cinemachine.DocumentationSortingAttribute;


namespace JH
{
    [CreateAssetMenu(fileName = "Buff Data", menuName = "ScriptableObjects/Buff Data")]

    public class BuffDataBase : ScriptableObject
    {
        [Header("※ 버프 데이터 \n수동으로 버프 데이터를 추가할 경우,\n아래 리스트에 버프 데이터를 추가해야\n참조할 수 있습니다.")]
        [Space]
        public List<BuffData> m_buffList;

        public void ClearList()
        {
            m_buffList.Clear();
        }
        public void AddBuff(BuffData data)
        {
            m_buffList.Add(data);
        }

        public BuffData TryGetBuff(int buffID)
        {
            foreach(var buff in m_buffList)
            {
                if(buff.ID == buffID)
                    return buff;
            }

            Debug.Log(buffID + " 버프를 찾을 수 없습니다.");
            return null;
        }

    }

  

}