using Google.GData.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.CompilerServices;
using UnityEngine;

namespace JH
{
    // 스킬과 관련된 클래스
    [System.Serializable]
    public class SkillData : SOData
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField][field: TextArea] public string Description { get; protected set; }
        [field: Header("Skill")]

        [field: SerializeField] public SkillType BaseType { get; protected set; }
        [field: SerializeField] public TargetTag SkillTarget { get; protected set; }
        [field: SerializeField] public float SkillCoolDown { get; protected set; }
        [field: SerializeField] public float SkillDuration { get; protected set; }
        [field: SerializeField] public float SkillDamage { get; protected set; }
        [field: SerializeField] public AimType AimType { get; protected set; }
        [field: SerializeField] public float SkillRange { get; protected set; }
        [field: Header("Skill Projectile")]
        [field: SerializeField] public int[] ProjectileID { get; protected set; }
        [field: SerializeField] public float ProjectileOffset { get; protected set; }
        [field: SerializeField] public float SkillRadius { get; protected set; }
        [field: SerializeField] public float SkillArc { get; protected set; }
        [field: SerializeField] public float SkillLifeTime { get; protected set; }
        [field: Header("Buff")]
        [field: SerializeField] public int[] BuffID { get; protected set; }
        [field: Header("Additional Value")]
        [field: SerializeField] public float[] Value1 { get; protected set; }
        [field: SerializeField] public float[] Value2 { get; protected set; }
        [field: SerializeField] public float[] Value3 { get; protected set; }


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
            GameData newData = new GameData(gameData.GameData[ID].Name, "SKILL", ExportData());
            gameData.GameData[ID] = newData;
        }

        // 데이터 행의 순서가 바뀌면 여기를 수정해야함
        public virtual List<GSTU_Data> ExportData()
        {
            List<GSTU_Data> dataList = new List<GSTU_Data>();
            GSTU_Data data = new GSTU_Data();


            dataList.Add(SetData("ID", ID.ToString()));


            return dataList;

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
                    BaseType = (SkillType)Enum.Parse(typeof(SkillType), item.Value);

                if (item.ColumnID == "Target")
                    SkillTarget = (TargetTag)Enum.Parse(typeof(TargetTag), item.Value);

                if (item.ColumnID == "CoolDown")
                    SkillCoolDown = float.Parse(item.Value);

                if (item.ColumnID == "Dutation")
                    SkillDuration = float.Parse(item.Value);

                if (item.ColumnID == "Damage")
                    SkillDamage = float.Parse(item.Value);

                if (item.ColumnID == "AimType")
                    AimType = (AimType)Enum.Parse(typeof(AimType), item.Value);

                if (item.ColumnID == "Range")
                    SkillRange = float.Parse(item.Value);

                if (item.ColumnID == "ProjectileID")
                    ProjectileID = GFunc.StringToInts(item.Value);

                if (item.ColumnID == "ProjectileOffset")
                    ProjectileOffset = float.Parse(item.Value);

                if (item.ColumnID == "Radius")
                    SkillRadius = float.Parse(item.Value);

                if (item.ColumnID == "Arc")
                    SkillArc = float.Parse(item.Value);

                if (item.ColumnID == "LifeTime")
                    SkillLifeTime = float.Parse(item.Value);

                if (item.ColumnID == "BuffID")
                    BuffID = GFunc.StringToInts(item.Value);

                if (item.ColumnID == "Value1")
                    Value1 = GFunc.StringToFloats(item.Value);

                if (item.ColumnID == "Value2")
                    Value1 = GFunc.StringToFloats(item.Value);

                if (item.ColumnID == "Value3")
                    Value3 = GFunc.StringToFloats(item.Value);
            }
        }














        // 스킬 사용이 가능한지 체크를 한다.
        public virtual bool CanActiveSkill(bool enable = true)
        {
            return enable;
        }

        // 스킬을 사용한다.
        public virtual void ActiveSkill() { }

        public virtual void InActiveSkill() { }


    }


}
