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
        [field: SerializeField] public int ID { get; protected set; }
        [field: SerializeField] public string Name { get; protected set; }
        [field: SerializeField][field: TextArea] public string Description { get; protected set; }
        [field: Header("Skill")]

        [field: SerializeField] public SkillType BaseType { get; protected set; }
        [field: SerializeField] public TargetTag SkillTarget { get; protected set; }
        [field: SerializeField] public SkillActiveTime ActiveTime { get; protected set; }
        [field: SerializeField] public AimType AimType { get; protected set; }
        [field: SerializeField] public LayerMask TargetLayer { get; private set; }


        [field: Header("Skill Level Data")]
        [field: SerializeField] public LevelData LevelData { get; protected set; }
        [field: SerializeField] public float SkillDelay { get; protected set; }
        [field: SerializeField] public float SkillSpeed { get; protected set; }
        [field: Header("Skill Buff")]
        [field: SerializeField] public int[] BuffID { get; protected set; }
        [field: Header("Skill Projectile")]
        [field: SerializeField] public int[] ProjectileID { get; protected set; }


        public float CoolDown => LevelData.CoolDown;
        public float Duration => LevelData.Duration;
        public float SkillDamage => LevelData.Damage;
        public float SkillRange => LevelData.Range;
        public float ProjectileOffset => LevelData.ProjectileOffset;
        public float SkillRadius => LevelData.Radius;
        public float SkillArc => LevelData.Arc;
        public float SkillLifeTime => LevelData.LifeTime;
        public List<BuffValues> BuffValues => LevelData.BuffValues;
        public float[] Value1 => LevelData.Value1;
        public float[] Value2 => LevelData.Value2;
        public float[] Value3 => LevelData.Value3;

        public int TryGetBuffID(int num = 0)
        {
            if (BuffID.Length - 1 < num)
                return BuffID[BuffID.Length -1];
            return BuffID[num];
        }
        public float TryGetBuffValue(int index, int num = 0)
        {
            return LevelData.TryGetBuffValue(index, num);
        }
        public float TryGetValue1(int num = 0)
        {
            return LevelData.TryGetValue1(num);

        }
        public float TryGetValue2(int num = 0)
        {
            return LevelData.TryGetValue2(num);

        }
        public float TryGetValue3(int num = 0)
        {
            return LevelData.TryGetValue3(num);

        }
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
            dataList.Add(SetData("Name", Name.ToString()));
            dataList.Add(SetData("Description", Description.ToString()));
            dataList.Add(SetData("BaseType", BaseType.ToString()));
            dataList.Add(SetData("Target", SkillTarget.ToString()));
            dataList.Add(SetData("ActiveTime", ActiveTime.ToString()));
            dataList.Add(SetData("CoolDown", CoolDown.ToString()));
            dataList.Add(SetData("Duration", Duration.ToString()));
            dataList.Add(SetData("Damage", SkillDamage.ToString()));
            dataList.Add(SetData("AimType", AimType.ToString()));
            dataList.Add(SetData("ProjectileID", GFunc.IntsToString(ProjectileID)));
            dataList.Add(SetData("Range", SkillRange.ToString()));
            dataList.Add(SetData("ProjectileOffset", ProjectileOffset.ToString()));
            dataList.Add(SetData("Radius", SkillRadius.ToString()));
            dataList.Add(SetData("Arc", SkillArc.ToString()));
            dataList.Add(SetData("LifeTime", SkillLifeTime.ToString()));
            dataList.Add(SetData("BuffID",GFunc.IntsToString(BuffID)));
            dataList.Add(SetData("BuffValue", GFunc.BuffValuesToString(BuffValues)));
            dataList.Add(SetData("Value1",GFunc.FloatsToString(Value1)));
            dataList.Add(SetData("Value2", GFunc.FloatsToString(Value2)));
            dataList.Add(SetData("Value3", GFunc.FloatsToString(Value3)));
            dataList.Add(SetData("SkillDelay", SkillDelay.ToString()));
            dataList.Add(SetData("SkillSpeed", SkillSpeed.ToString()));


            return dataList;

        }
        //  데이터를 업데이트한다.
        public virtual void UpdateData(List<GSTU_Data> datas)
        {
            LevelData = new LevelData();
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

                if (item.ColumnID == "ActiveTime")
                    ActiveTime = (SkillActiveTime)Enum.Parse(typeof(SkillActiveTime), item.Value);

                if (item.ColumnID == "CoolDown")
                    LevelData.CoolDown = float.Parse(item.Value);

                if (item.ColumnID == "Duration")
                    LevelData.Duration = float.Parse(item.Value);

                if (item.ColumnID == "Damage")
                    LevelData.Damage = float.Parse(item.Value);

                if (item.ColumnID == "AimType")
                    AimType = (AimType)Enum.Parse(typeof(AimType), item.Value);
      
                if (item.ColumnID == "ProjectileID")
                    ProjectileID = GFunc.StringToInts(item.Value);

                if (item.ColumnID == "Range")
                    LevelData.Range = float.Parse(item.Value);

                if (item.ColumnID == "ProjectileOffset")
                    LevelData.ProjectileOffset = float.Parse(item.Value);

                if (item.ColumnID == "Radius")
                    LevelData.Radius = float.Parse(item.Value);

                if (item.ColumnID == "Arc")
                    LevelData.Arc = float.Parse(item.Value);

                if (item.ColumnID == "LifeTime")
                    LevelData.LifeTime = float.Parse(item.Value);

                if (item.ColumnID == "BuffID")
                    BuffID = GFunc.StringToInts(item.Value);

                if (item.ColumnID == "BuffValue")
                    LevelData.BuffValues = GFunc.StringToBuffValues(item.Value);

                if (item.ColumnID == "Value1")
                    LevelData.Value1 = GFunc.StringToFloats(item.Value);

                if (item.ColumnID == "Value2")
                    LevelData.Value2 = GFunc.StringToFloats(item.Value);

                if (item.ColumnID == "Value3")
                    LevelData.Value3 = GFunc.StringToFloats(item.Value);

                if (item.ColumnID == "SkillDelay")
                    SkillDelay = float.Parse(item.Value);

                if (item.ColumnID == "SkillSpeed")
                    SkillSpeed = float.Parse(item.Value);
            }
        }



        // 스킬 사용이 가능한지 체크를 한다.
        public virtual bool CanActiveSkill(bool enable = true)
        {
            return enable;
        }

        // 스킬을 사용한다.
        public virtual void ActiveSkill() { }

        // 스킬 비활성화
        public virtual void InActiveSkill() { }


    }


}
