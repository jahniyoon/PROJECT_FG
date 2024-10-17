using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static UnityEditor.Progress;

namespace JH
{
    [System.Serializable]
    public class LevelData
    {
        [Header("Level Data")]
        public float CoolDown;
        public float Duration;
        public float Damage;
        public float Range;
        public float ProjectileOffset;
        public float Radius;
        public float Arc;
        public float LifeTime;
        public float Count = 1;
        public List<BuffValues> BuffValues;
        public float[] Value1;
        public float[] Value2;
        public float[] Value3;
        public float[] TryGetBuffValues(int index)
        {
            {
                if (BuffValues.Count - 1 < index)
                {
                    //Debug.Log($"{index}번째 버프 값이 존재하지 않습니다.");
                    return new float[1] {0};
                }
                return BuffValues[index].Values;
            }
        }

        public float TryGetBuffValue(int index, int num = 0)
        {
            if (BuffValues.Count - 1 < index)
            {
                //Debug.Log($"{index}번째 버프 값이 존재하지 않습니다.");
                return 0;
            }

            
            if (BuffValues[index].Length - 1 < num)
            {
                //Debug.Log($"{index}번째의 {num}버프 값이 존재하지 않습니다.");
                return 0;
            }

            return BuffValues[index].Values[num];
        }
        public float TryGetValue1(int num = 0)
        {
            if (Value1.Length - 1 < num)
                return 0;
            return Value1[num];
        }
        public float TryGetValue2(int num = 0)
        {
            if (Value2.Length - 1 < num)
                return 0;
            return Value2[num];
        }
        public float TryGetValue3(int num = 0)
        {
            if (Value3.Length - 1 < num)
                return 0;
            return Value3[num];
        }
    }
    [System.Serializable]
    public class FoodPowerLevelData
    {
        public string Name;
        public string Description;
        public int ID;

        public LevelData LevelData;

        public string Json()
        {
            return JsonUtility.ToJson(this);
        }
       
        // 레벨 데이터를 데이터 베이스에서 직접 가져온다.
        public FoodPowerLevelData SetFoodPowerLevelData(int ID)
        {
            List<GSTU_Data> GameData = GFunc.GetGameData(ID);
            this.ID = ID;
            LevelData = new LevelData();
            foreach (var Data in GameData)
            {
                if (Data.Value == "-")
                    continue;

                if (Data.ColumnID == "Name")
                    Name = Data.Value;
                if (Data.ColumnID == "Description")
                    Description = Data.Value;

                if (Data.ColumnID == "CoolDown")
                    LevelData.CoolDown = float.Parse(Data.Value);

                if (Data.ColumnID == "Duration")
                    LevelData.Duration = float.Parse(Data.Value);

                if (Data.ColumnID == "Damage")
                    LevelData.Damage = float.Parse(Data.Value);

                if (Data.ColumnID == "Range")
                    LevelData.Range = float.Parse(Data.Value);

                if (Data.ColumnID == "ProjectileOffset")
                    LevelData.ProjectileOffset = float.Parse(Data.Value);

                if (Data.ColumnID == "Radius")
                    LevelData.Radius = float.Parse(Data.Value);

                if (Data.ColumnID == "Arc")
                    LevelData.Arc = float.Parse(Data.Value);

                if (Data.ColumnID == "LifeTime")
                    LevelData.LifeTime = float.Parse(Data.Value);

                if (Data.ColumnID == "Count")
                    LevelData.Count = int.Parse(Data.Value);

                if (Data.ColumnID == "BuffValue")
                    LevelData.BuffValues = GFunc.StringToBuffValues(Data.Value);

                if (Data.ColumnID == "Value1")
                    LevelData.Value1 = GFunc.StringToFloats(Data.Value);

                if (Data.ColumnID == "Value2")
                    LevelData.Value2 = GFunc.StringToFloats(Data.Value);

                if (Data.ColumnID == "Value3")
                    LevelData.Value3 = GFunc.StringToFloats(Data.Value);
            }



            return this;
        }
        private GSTU_Data SetData(string ColumnID, string Value)
        {
            GSTU_Data data = new GSTU_Data();
            data.ColumnID = ColumnID;
            data.Value = Value;
            return data;
        }


        // 데이터 행의 순서가 바뀌면 여기를 수정해야함
        public List<GSTU_Data> ExportData()
        {
            List<GSTU_Data> dataList = new List<GSTU_Data>();
            GSTU_Data data = new GSTU_Data();


            dataList.Add(SetData("ID", ID.ToString()));
            dataList.Add(SetData("Name", Name.ToString()));
            dataList.Add(SetData("Description", Description.ToString()));

            dataList.Add(SetData("CoolDown", LevelData.CoolDown.ToString()));
            dataList.Add(SetData("Duration", LevelData.Duration.ToString()));
            dataList.Add(SetData("Damage", LevelData.Damage.ToString()));
            dataList.Add(SetData("Range", LevelData.Range.ToString()));
            dataList.Add(SetData("ProjectileOffset", LevelData.ProjectileOffset.ToString()));
            dataList.Add(SetData("Radius", LevelData.Radius.ToString()));
            dataList.Add(SetData("Arc", LevelData.Arc.ToString()));
            dataList.Add(SetData("LifeTime", LevelData.LifeTime.ToString()));
            dataList.Add(SetData("Count", LevelData.Count.ToString()));

            dataList.Add(SetData("BuffValue", GFunc.BuffValuesToString(LevelData.BuffValues)));
            dataList.Add(SetData("Value1", GFunc.FloatsToString(LevelData.Value1)));
            dataList.Add(SetData("Value2", GFunc.FloatsToString(LevelData.Value2)));
            dataList.Add(SetData("Value3", GFunc.FloatsToString(LevelData.Value3)));

            return dataList;
        }


        public void ExportLevelData()
        {
            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
            if (gameData.GameData.ContainsKey(ID) == false)
            {
                Debug.LogWarning("데이터 ID를 확인해주세요." + ID);
                return;
            }
            GameData newData = new GameData(gameData.GameData[ID].Name, "FOOD POWER LEVEL", ExportData());
            gameData.GameData[ID] = newData;

        }

    }


}
