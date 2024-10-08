using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


namespace JH
{
    [CreateAssetMenu(fileName = "Projectile Data", menuName = "ScriptableObjects/Projectile/Projectile")]

    public class ProjectileData : SOData
    {
        [field: Header("투사체 데이터")]
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField][field: TextArea] public string Description { get; private set; }
        [field: SerializeField] public ProjectileType Type { get; private set; }


        [field: Tooltip("투사체의 관통 가능 횟수")]
        [field: SerializeField] public int Penetrate { get; private set; }
        [field: Tooltip("투사체의 크기")]
        [field: SerializeField] public float ProjectileScale { get; private set; }
        [field: Tooltip("투사체의 속도")]
        [field: SerializeField] public float ProjectileSpeed { get; private set; }
        [field: SerializeField] public int[] DerivativesID { get; private set; }



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
            GameData newData = new GameData(gameData.GameData[ID].Name, "PROJECTILE", ExportData());
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

                // 레벨 데이터를 모두 가져와서 넣는다.
                if (item.ColumnID == "Type")
                    Type = (ProjectileType)Enum.Parse(typeof(ProjectileType), item.Value);

                if (item.ColumnID == "Penetrate")
                    Penetrate = int.Parse(item.Value);

                if (item.ColumnID == "ProjectileSpeed")
                    ProjectileSpeed = int.Parse(item.Value);

                if (item.ColumnID == "ProjectileScale")
                    ProjectileScale = float.Parse(item.Value);

                if (item.ColumnID == "DerivativesID")
                    DerivativesID = GFunc.StringToInts(item.Value);
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
            dataList.Add(SetData("Type", Type.ToString()));
            dataList.Add(SetData("Penetrate", Penetrate.ToString()));
            dataList.Add(SetData("ProjectileSpeed", ProjectileSpeed.ToString()));
            dataList.Add(SetData("ProjectileScale", ProjectileScale.ToString()));
            dataList.Add(SetData("DerivativesID", GFunc.IntsToString(DerivativesID)));

            return dataList;

        }

    }
}