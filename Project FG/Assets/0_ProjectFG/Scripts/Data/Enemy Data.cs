using Google.GData.Extensions;
using GoogleSheetsToUnity;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;


namespace JH
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "ScriptableObjects/Enemy/EnemyDefault", order = 0)]

    public class EnemyData : ScriptableObject
    {
        [field: Header("에네미 정보")]
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: SerializeField] public string Description { get; private set; }
        [field: Tooltip("최대 체력 및 체력")]
        [field: SerializeField] public float Health { get; private set; }
        [field: Tooltip("이동 속도")]
        [field: SerializeField] public float MoveSpeed { get; private set; }
        [field: Tooltip("회전 속도")]
        [field: SerializeField] public float RotateSpeed { get; private set; }
        [field: SerializeField] public float Damage { get; private set; }



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
        [field: SerializeField] public BuffBase PredationStun { get; private set; }


        [field: Header("푸드 파워")]
        [field: SerializeField] public FoodPower FoodPower { get; private set; }


        //  데이터를 업데이트한다.
        public virtual void UpdateData(List<GSTU_Data> datas)
        {
            // 가져온 데이터를 변환하는 부분
            foreach (var item in datas)
            {
                if (item.ColumnID == "Name")
                    Name = item.Value;

                if (item.ColumnID == "Description")
                    Description = item.Value;

                if (item.ColumnID == "Health")
                    Health = float.Parse(item.Value);

                if (item.ColumnID == "MoveSpeed")
                    MoveSpeed = float.Parse(item.Value);

                if (item.ColumnID == "AttackRange")
                    AttackRange = float.Parse(item.Value);

                if (item.ColumnID == "EscapeRange")
                    EscapeRange = float.Parse(item.Value);

                if (item.ColumnID == "Damage")
                    Damage = float.Parse(item.Value);

                if (item.ColumnID == "PredationHealthRatio")
                    PredationHealthRatio = float.Parse(item.Value);

                if (item.ColumnID == "PredationStunCoolDown")
                    PredationStunCoolDown = float.Parse(item.Value);
            }
        }

        private GSTU_Data SetData(string ColumnID, string Value)
        {
            GSTU_Data data = new GSTU_Data();
            data.ColumnID = ColumnID;
            data.Value = Value;
            return data;
        }

        // 데이터 행의 순서가 바뀌면 여기를 수정해야함
        public virtual List<GSTU_Data> ExportData()
        {
            List<GSTU_Data> dataList = new List<GSTU_Data>();
            GSTU_Data data = new GSTU_Data();


            dataList.Add(SetData("ID", ID.ToString()));
            dataList.Add(SetData("Name", Name.ToString()));
            dataList.Add(SetData("Description", Description.ToString()));
            dataList.Add(SetData("Health", Health.ToString()));
            dataList.Add(SetData("MoveSpeed", MoveSpeed.ToString()));
            dataList.Add(SetData("AttackRange", AttackRange.ToString()));
            dataList.Add(SetData("EscapeRange", EscapeRange.ToString()));
            dataList.Add(SetData("Damage", Damage.ToString()));
            dataList.Add(SetData("PredationHealthRatio", PredationHealthRatio.ToString()));
            dataList.Add(SetData("PredationStunCoolDown", PredationStunCoolDown.ToString()));

            return dataList;

        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(EnemyData), editorForChildClasses: true), CanEditMultipleObjects]
    public class EnemyDataEditor : Editor
    {
        EnemyData data;

        void OnEnable()
        {
            data = (EnemyData)target;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("\n\nGoogle Sheet");
            if (GUILayout.Button("Local 데이터 가져오기"))
            {
                ImportData();
            }
            GUILayout.Label("\n\nGoogle Sheet");


            if (GUILayout.Button("Online 데이터 내보내기") && !Application.isPlaying)
            {
                bool result = EditorUtility.DisplayDialog("Warning", "정말 업로드하시겠습니까?\n현재 데이터와 Game Data의 데이터들을 기준으로 구글 스프레드 시트의 데이터를 모두 덮어씁니다.", "덮어쓰기", "취소");
                if (result)
                    ExportData();
            }
        }

        // 데이터 가져오기
        public void ImportData(bool isOnline = false)
        {
            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
            if (gameData.GameData.ContainsKey(data.ID) == false)
            {
                Debug.LogWarning("데이터 ID를 확인해주세요." + data.ID);
                return;
            }

            List<GSTU_Data> gstuDatas = gameData.GameData[data.ID].Data;
            data.UpdateData(gstuDatas);
        }
        // 데이터 내보내기
        public void ExportData()
        {
            DataReader gameData = Resources.Load<DataReader>("Data/GameData");
            if (gameData.GameData.ContainsKey(data.ID) == false)
            {
                Debug.LogWarning("데이터 ID를 확인해주세요." + data.ID);
                return;
            }
            GameData newData = new GameData(gameData.GameData[data.ID].name, "ENEMY", data.ExportData());
            gameData.GameData[data.ID] = newData;
            // TODO : 인덱스 값에 맞게 하나만 업데이트하고있음.
            // 만약 숫자 사이에 공백이 있으면 안됨. 수치를 고정하던가 다른 방법 찾기
            gameData.ExportData("ENEMY");
        }

    }

#endif
}