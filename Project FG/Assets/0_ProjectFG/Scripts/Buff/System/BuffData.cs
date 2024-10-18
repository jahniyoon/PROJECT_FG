using Google.GData.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace JH
{

    [System.Serializable]
    [CreateAssetMenu(fileName = "Buff Data", menuName = "ScriptableObjects/Buff/Buff Data")]

    public class BuffData : SOData
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField][field: TextArea] public string Description { get; private set; }
        [field: Header("Buff Info")]
        [field: SerializeField] public BuffType Type { get; private set; } // 버프 타입
        [field: SerializeField] public BuffEffectCondition Condition { get; private set; } // 버프 타입
        [field: SerializeField] public string StackType { get; private set; } // 버프 타입
        [field: SerializeField] public float[] Value1 { get; private set; } // 버프 타입
        [field: SerializeField] public float[] Value2 { get; private set; } // 버프 타입




        [field: Header("One And Only Buff")]
        [field: SerializeField] public bool IsOneAndOnly { get; private set; }
        [field: SerializeField] public int Priority { get; private set; }

        [field: Header("Stack Buff")]
        [field: SerializeField] public int ActiveStack { get; private set; }
        [field: SerializeField] public float StackUpTime { get; private set; }
        [field: SerializeField] public float DecreaseTime { get; private set; }




        public override void SetData(GameData gamedata)
        {
            base.SetData(gamedata);
            UpdateData(gamedata.Data);
        }
        public override void UpdateGameData()
        {
            base.UpdateGameData();
            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
            if (gameData.GameData.ContainsKey(ID) == false)
            {
                Debug.LogWarning("데이터 ID를 확인해주세요." + ID);
                return;
            }
            GameData newData = new GameData(gameData.GameData[ID].Name, "BUFF", ExportData());
            gameData.GameData[ID] = newData;
        }

        //  데이터를 업데이트한다.
        public virtual void UpdateData(List<GSTU_Data> datas)
        {
            // 가져온 데이터를 변환하는 부분
            foreach (var item in datas)
            {
                if (item.Value == "-")
                    continue;


                if (item.ColumnID == "ID")
                    ID = int.Parse(item.Value);

                if (item.ColumnID == "Name")
                    Name = item.Value;

                if (item.ColumnID == "Description")
                    Description = item.Value;

                if (item.ColumnID == "BaseType")
                    Type = (BuffType)Enum.Parse(typeof(BuffType), item.Value);

                if (item.ColumnID == "EffectCondition")
                    Condition = (BuffEffectCondition)Enum.Parse(typeof(BuffEffectCondition), item.Value);

                if (item.ColumnID == "StackType")
                    StackType = item.Value;

                if (item.ColumnID == "Value1")
                    Value1 = GFunc.StringToFloats(item.Value);

                if (item.ColumnID == "Value2")
                    Value1 = GFunc.StringToFloats(item.Value);
            }
        }

        // 데이터 행의 순서가 바뀌면 여기를 수정해야함
        public virtual List<GSTU_Data> ExportData()
        {
            List<GSTU_Data> dataList = new List<GSTU_Data>();
            GSTU_Data data = new GSTU_Data();

            dataList.Add(SetData("ID", ID.ToString()));
            dataList.Add(SetData("Name", Name));
            dataList.Add(SetData("Description", Description));
            dataList.Add(SetData("BaseType", Type.ToString()));
            dataList.Add(SetData("EffectCondition", Condition.ToString()));
            dataList.Add(SetData("StackType", StackType));
            dataList.Add(SetData("Value1", GFunc.FloatsToString(Value1)));
            dataList.Add(SetData("Value2", GFunc.FloatsToString(Value2)));
            return dataList;

        }

    }
}
