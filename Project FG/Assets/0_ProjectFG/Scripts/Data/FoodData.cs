using Google.GData.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;


namespace JH
{
    [CreateAssetMenu(fileName = "Food Power Data", menuName = "ScriptableObjects/Food Power/Food Power")]

    public class FoodPowerData : ScriptableObject
    {
        [field: Header("푸드파워 데이터")]
        [field: SerializeField] public int ID { get; private set; }
        [field: SerializeField] public string Name { get; private set; }
        [field: TextArea]
        [field: SerializeField] public string Description { get; private set; }
        [field: Tooltip("푸드 파워 아이콘\n푸드 파워 UI에 표시할 아이콘")]
        [field: SerializeField] public Sprite Icon { get; private set; }   // 체력
        [field: Header("푸드파워 레벨 데이터")]
        [field: SerializeField] public FoodPowerLevelData[] LevelDatas { get; private set; }

        [field: Header("스캔 범위")]
        [field: Tooltip("TargetNearest 방식의 스캔 시 스캔할 범위")]

        [field: SerializeField] public float TargetNearestScanRadius { get; private set; }
        [field: Tooltip("TargetNearest 타겟 검출 실패 시, 발사 여부")]
        [field: SerializeField] public bool AlwaysShoot { get; private set; }


        public FoodPowerLevelData GetLevelData(int level)
        {
            if (LevelDatas.Length < level)
            {
                Debug.Log($"{Name}의 '{level}'번째 레벨데이터를 찾을 수 없습니다.");
                return LevelDatas[LevelDatas.Length];
            }
            return LevelDatas[level];
        }
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
                // 레벨 데이터를 모두 가져와서 넣는다.
                if (item.ColumnID == "LevelData")
                {
                    // 범위를 가져온다. 60001-60010
                    int[] idRange = item.Value.Split('-').Select(int.Parse).ToArray();

                    // 두칸일 경우에만
                    if(idRange.Length == 2)
                    {
                        int count = idRange[1] - idRange[0] + 1;

                        // 기존 데이터 초기화하고
                        LevelDatas = new FoodPowerLevelData[count];

                        for (int i=0; i < count; i++)
                        {
                            FoodPowerLevelData LevelData = new FoodPowerLevelData();
                            // ID에 맞는 데이터를 가져온다.
                            LevelDatas[i] = LevelData.SetFoodPowerLevelData(idRange[0] + i);
                        }
                    }
                }
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
            
            string LevelData = "0";
            if (LevelDatas != null)
             LevelData = $"{LevelDatas[0].ID}-{LevelDatas[LevelDatas.Length - 1].ID}";

            // 보유하고있는 푸드파워 데이터또한 모두 업데이트한다.
            foreach(var Data in LevelDatas)
            {
                Data.ExportLevelData();
            }

            
            dataList.Add(SetData("LevelData", LevelData));

            return dataList;

        }


    }

#if UNITY_EDITOR

    [CustomEditor(typeof(FoodPowerData), editorForChildClasses: true), CanEditMultipleObjects]
    public class FoodPowerDataEditor : Editor
    {
        FoodPowerData data;

        void OnEnable()
        {
            data = (FoodPowerData)target;
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
                bool result = EditorUtility.DisplayDialog("Warning", "FoodPower와 FoodPowerLevel 데이터를 스프레드 시트로 내보냅니다.\n정말 업로드하시겠습니까?", "덮어쓰기", "취소");
                if (result)
                    ExportData();
            }
        }

        // 데이터 가져오기
        public void ImportData(bool isOnline = false)
        {
            List<GSTU_Data> gstuDatas = GFunc.GetGameData(data.ID);
            if(gstuDatas != null)
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
            GameData newData = new GameData(gameData.GameData[data.ID].name, "FOOD POWER", data.ExportData());
            gameData.GameData[data.ID] = newData;
            // TODO : 인덱스 값에 맞게 하나만 업데이트하고있음.
            // 만약 숫자 사이에 공백이 있으면 안됨. 수치를 고정하던가 다른 방법 찾기
            // 푸드파워 레벨 데이터도 함께 업데이트
            gameData.ExportData("FOOD POWER");
            gameData.ExportData("FOOD POWER LEVEL");

        }

    }

#endif

}