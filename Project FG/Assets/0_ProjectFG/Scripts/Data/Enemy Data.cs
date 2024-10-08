using Google.GData.Extensions;
using GoogleSheetsToUnity;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace JH
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObjects/Enemy/EnemyDefault", order = 0)]

    public class EnemyData : SOData
    {
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField][field: TextArea] public string Description { get; private set; }
        [field: SerializeField] public EnemyType Type { get; private set; }
        [field: Header("에네미 정보")]

        [field: Tooltip("최대 체력 및 체력")]
        [field: SerializeField] public float Health { get; private set; }
        [field: Tooltip("이동 속도")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: Tooltip("회전 속도")]
        [field: SerializeField] public float RotateSpeed { get; private set; }



        [field: Header("에네미 범위")]
        [field: Tooltip("공격 범위\n해당 범위 내에 타겟이 있으면 공격 상태로 전환한다.")]
        [field: SerializeField] public float AttackRange { get; private set; }
        [field: Tooltip("회피 범위\n해당 범위 내에 타겟이 있으면 회피한다.")]
        [field: SerializeField] public float EscapeRange { get; private set; }



        [field: Header("포식 가능 상태")]
        [field: Tooltip("포식 가능한 상태의 체력 비율")]
        [field: Range(1, 100)]
        [field: SerializeField] public float PredationHealthRatio { get; private set; }
        [field: Tooltip("포식상태 돌입 시 쿨다운")]

        [field: SerializeField] public float PredationStunCoolDown { get; private set; }
        [field: SerializeField] public BuffData PredationStun { get; private set; }



        [field: Header("피격 무시")]
        [field: SerializeField] public bool IgnoreAttack { get; private set; }

        [field: Header("푸드 파워")]
        [field: SerializeField] public FoodPower FoodPower { get; private set; }

        [field: Header("스킬")]

        [field: SerializeField] public int[] AttackSkillID { get; private set; }
        [field: SerializeField] public int[] RoutineSkillID { get; private set; }
        [field: SerializeField] public float[] AdditionalValues { get; private set; }

        public override void SetData(GameData gamedata)
        {
            UpdateData(gamedata.Data);
        }

        // 데이터 베이스의 데이터를 업데이트해준다.
        public override void UpdateGameData()
        {
            base.UpdateGameData();
            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
            if (gameData.GameData.ContainsKey(ID) == false)
            {
                Debug.LogWarning("데이터 ID를 확인해주세요." + ID);
                return;
            }
            GameData newData = new GameData(gameData.GameData[ID].Name, "ENEMY", ExportData());
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
                    Type = (EnemyType)Enum.Parse(typeof(EnemyType), item.Value);

                if (item.ColumnID == "Health")
                    Health = float.Parse(item.Value);

                if (item.ColumnID == "MoveSpeed")
                    MoveSpeed = float.Parse(item.Value);

                if (item.ColumnID == "AttackRange")
                    AttackRange = float.Parse(item.Value);

                if (item.ColumnID == "EscapeRange")
                    EscapeRange = float.Parse(item.Value);

                if (item.ColumnID == "PredationHealthRatio")
                    PredationHealthRatio = float.Parse(item.Value);

                if (item.ColumnID == "PredationStunCoolDown")
                    PredationStunCoolDown = float.Parse(item.Value);

                if (item.ColumnID == "AttackSkill")
                    AttackSkillID = GFunc.StringToInts(item.Value);

                if (item.ColumnID == "RoutineSkill")
                    RoutineSkillID = GFunc.StringToInts(item.Value);

                if (item.ColumnID == "AdditionalValue")
                    AdditionalValues = GFunc.StringToFloats(item.Value);
            }
        }



        // 데이터 행의 순서가 바뀌면 여기를 수정해야함
        public virtual List<GSTU_Data> ExportData()
        {
            List<GSTU_Data> dataList = new List<GSTU_Data>();
            GSTU_Data data = new GSTU_Data();


            dataList.Add(SetData("ID", ID.ToString()));
            dataList.Add(SetData("Name", Name.ToString()));
            dataList.Add(SetData("Description", Description.ToString()));
            dataList.Add(SetData("BaseType", Type.ToString()));
            dataList.Add(SetData("Health", Health.ToString()));
            dataList.Add(SetData("MoveSpeed", MoveSpeed.ToString()));
            dataList.Add(SetData("AttackRange", AttackRange.ToString()));
            dataList.Add(SetData("EscapeRange", EscapeRange.ToString()));
            dataList.Add(SetData("PredationHealthRatio", PredationHealthRatio.ToString()));
            dataList.Add(SetData("PredationStunCoolDown", PredationStunCoolDown.ToString()));
            dataList.Add(SetData("AttackSkill", GFunc.IntsToString(AttackSkillID)));
            dataList.Add(SetData("RoutineSkill", GFunc.IntsToString(RoutineSkillID)));
            dataList.Add(SetData("AdditionalValue", GFunc.FloatsToString(AdditionalValues)));

            return dataList;

        }
    }

//#if UNITY_EDITOR

//    [CustomEditor(typeof(EnemyData), editorForChildClasses: true), CanEditMultipleObjects]
//    public class EnemyDataEditor : Editor
//    {
//        EnemyData data;

//        void OnEnable()
//        {
//            data = (EnemyData)target;
//        }
//        public override void OnInspectorGUI()
//        {
//            base.OnInspectorGUI();

//            GUILayout.Label("\n\nGoogle Sheet");
//            if (GUILayout.Button("Local 데이터 가져오기"))
//            {
//                ImportData();
//            }
//            GUILayout.Label("\n\nGoogle Sheet");


//            if (GUILayout.Button("Ememy Data Online 데이터 내보내기") && !Application.isPlaying)
//            {
//                bool result = EditorUtility.DisplayDialog("Warning", "Enemy 시트만 스프레드 시트로 내보냅니다.\n정말 업로드하시겠습니까?", "덮어쓰기", "취소");
//                if (result)
//                    ExportData();
//            }
//        }

//        // 데이터 가져오기
//        public void ImportData(bool isOnline = false)
//        {
//            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
//            if (gameData.GameData.ContainsKey(data.ID) == false)
//            {
//                Debug.LogWarning("데이터 ID를 확인해주세요." + data.ID);
//                return;
//            }

//            List<GSTU_Data> gstuDatas = gameData.GameData[data.ID].Data;
//            data.UpdateData(gstuDatas);

//#if UNITY_EDITOR
//            EditorUtility.SetDirty(data); // 데이터가 변경되었음을 알림

//            // 저장 (에디터에서만 동작)
//            AssetDatabase.SaveAssets();
//            AssetDatabase.Refresh();
//#else
//    Debug.LogError("SaveData는 에디터에서만 사용할 수 있습니다.");
//#endif
//        }
//        // 데이터 내보내기
//        public void ExportData()
//        {
//            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
//            if (gameData.GameData.ContainsKey(data.ID) == false)
//            {
//                Debug.LogWarning("데이터 ID를 확인해주세요." + data.ID);
//                return;
//            }
//            data.UpdateGameData();
//            // TODO : 인덱스 값에 맞게 하나만 업데이트하고있음.
//            // 만약 숫자 사이에 공백이 있으면 안됨. 수치를 고정하던가 다른 방법 찾기
//            gameData.ExportData("ENEMY");
//        }

//    }

//#endif

}