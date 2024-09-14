using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public class FoodPowerLevelData
    {
        public string Name;
        public string Description;
        public int ID;
        [Header("Level Data")]
        public float CoolDown;
        public float Damage;
        public float Range;
        public float Radius;
        public float Duration;
        public int Count;
        public FoodPowerAimType AimType;
        public float[] AdditionalValues = new float[1];


        public string Json()
        {
            return JsonUtility.ToJson(this);
        }
        public float GetAdditionalValue(int num)
        {
            if (AdditionalValues.Length < num + 1)
            {
                Debug.Log($"{Name}의 '{num}'번째 값을 찾을 수 없습니다.");
                return 0;
            }

            return AdditionalValues[num];
        }
        // 레벨 데이터를 데이터 베이스에서 직접 가져온다.
        public FoodPowerLevelData SetFoodPowerLevelData(int ID)
        {
            List<GSTU_Data> GameData = GFunc.GetGameData(ID);
            this.ID = ID;
            foreach (var Data in GameData)
            {
                if (Data.ColumnID == "Name")
                    Name = Data.Value;
                if (Data.ColumnID == "Description")
                    Description = Data.Value;

                if (Data.ColumnID == "CoolDown")
                    CoolDown = float.Parse(Data.Value);
                if (Data.ColumnID == "Damage")
                    Damage = float.Parse(Data.Value);
                if (Data.ColumnID == "Range")
                    Range = float.Parse(Data.Value);
                if (Data.ColumnID == "Radius")
                    Radius = float.Parse(Data.Value);
                if (Data.ColumnID == "Duration")
                    Duration = float.Parse(Data.Value);
                if (Data.ColumnID == "Count")
                    Count = int.Parse(Data.Value);
                if (Data.ColumnID == "AimType")
                {
                    switch (Data.Value)
                    {
                        case "가장 가까운 적":
                            AimType = FoodPowerAimType.TargetNearest;
                            break;
                        case "이동 방향":
                            AimType = FoodPowerAimType.MoveDirection;
                            break;
                        case "포인터 방향":
                            AimType = FoodPowerAimType.PointerDirection;
                            break;
                        case "랜덤한 방향":
                            AimType = FoodPowerAimType.RandomDirection;
                            break;
                        case "사거리내 랜덤한 적":
                            AimType = FoodPowerAimType.RandomEnemyDirection;
                            break;
                        case "피격 시":
                            AimType = FoodPowerAimType.Hit;
                            break;
                        case "PC위치 소환":
                            AimType = FoodPowerAimType.PcPosition;
                            break;
                        case "PC주변":
                            AimType = FoodPowerAimType.PcRadius;
                            break;
                    }
                }

                if (Data.ColumnID == "AdditionalValues")
                {
                    AdditionalValues = Data.Value.Split(',').Select(float.Parse).ToArray();
                }
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
            dataList.Add(SetData("Type", "-"));
            dataList.Add(SetData("CoolDown", CoolDown.ToString()));
            dataList.Add(SetData("Damage", Damage.ToString()));
            dataList.Add(SetData("Range", Range.ToString()));
            dataList.Add(SetData("Radius", Radius.ToString()));
            dataList.Add(SetData("Duration", Duration.ToString()));
            dataList.Add(SetData("Count", Count.ToString()));
            dataList.Add(SetData("AimType", GetAimTypeName(AimType)));

            StringBuilder sb = new StringBuilder();
            sb.Append(AdditionalValues[0]);
            // 추가데이터는 ,로 분류
            if(1 < AdditionalValues.Length)
            {
                for (int i = 1; i < AdditionalValues.Length; i++)
                {
                    sb.Append(","+AdditionalValues[i]);
                }
            }
            dataList.Add(SetData("AdditionalValues", sb.ToString()));


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
            GameData newData = new GameData(gameData.GameData[ID].name, "FOOD POWER LEVEL", ExportData());
            gameData.GameData[ID] = newData;

        }
        public string GetAimTypeName(FoodPowerAimType AimType)
        {
            switch (AimType)
            {
                case FoodPowerAimType.TargetNearest:
                    return "가장 가까운 적";
                case FoodPowerAimType.MoveDirection:
                    return "이동 방향";
                case FoodPowerAimType.PointerDirection:
                    return "포인터 방향";
                case FoodPowerAimType.RandomDirection:
                    return "랜덤한 방향";
                case FoodPowerAimType.RandomEnemyDirection:
                    return "사거리내 랜덤한 적";
                case FoodPowerAimType.Hit:
                    return "피격 시";
                case FoodPowerAimType.PcPosition:
                    return "PC위치 소환";
                case FoodPowerAimType.PcRadius:
                    return "PC주변";
            }
            return "Type Null";
        }

    }

 
}
