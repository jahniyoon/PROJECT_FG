using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

using UnityEngine.Events;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Rendering;
using System.Linq;
using UnityEngine.Playables;




#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JH
{
    [CreateAssetMenu(fileName = "Reader", menuName = "ScriptableObjects/DataReader/DataReader", order = int.MaxValue)]


    public class DataReader : DataReaderBase
    {
        [SerializeField] public SerializedDictionary<int, GameData> m_gameData = new SerializedDictionary<int, GameData>();
        public SerializedDictionary<int, GameData> GameData => m_gameData;

        StringBuilder m_savePath = new StringBuilder();
        StringBuilder m_loadPath = new StringBuilder();


        #region UPDATE / EXPORT

        // 지정한 시트 전체의 데이터를 스프레드 시트에서 불러오는 메소드
        internal void UpdateGameData(string sheetID, List<GSTU_Cell> list)
        {
            int id = 0;
            string name = default;

            List<GSTU_Data> datas = new List<GSTU_Data>();
            foreach (var item in list)
            {
                // 데이터를 필요한 것만 추출
                GSTU_Data gstuData = new GSTU_Data();
                gstuData.ColumnID = item.columnId;
                gstuData.Value = item.value;
                datas.Add(gstuData);

                if (item.columnId == "ID")
                    id = int.Parse(item.value);

                if (item.columnId == "Name")
                    name = ("     " + item.value);
            }
            GameData data = new GameData(id + name, sheetID, datas);
            m_gameData.Add(id, data);
        }

        // 현재 SO에 저장된 데이터중 특정 시트의 데이터를 내보낸다.
        // 특정 위치로 업로드하려면 해당 위치를 알아야함.
        public void ExportData(string sheet)
        {
            int count = START_ROW_LENGTH;

            // 시트별로 시트 전체 리스트 안에 리스트를 넣어 한번에 보낸다.
            List<List<string>> Combined = new List<List<string>>();
            string StartCell = "A" + count;

            foreach (var item in m_gameData)
            {
                // 같은 시트대상만 업데이트한다.
                if (item.Value.SheetID.Equals(sheet) == false)
                    continue;
                List<string> list = new List<string>();
                // 한 라인
                for (int i = 0; i < item.Value.Data.Count; i++)
                {
                    list.Add(item.Value.Data[i].Value);
                }
                Combined.Add(list);
                //SpreadsheetManager.Write(new GSTU_Search(associatedSheet, sheet, StartCell), new ValueRange(list), null);
                count++;
            }
            SpreadsheetManager.Write(new GSTU_Search(associatedSheet, sheet, StartCell), new ValueRange(Combined), null);
        }

        // 데이터 베이스 정렬
        public void OrderbyDatabase()
        {
            Dictionary<int, GameData> dataDic = m_gameData;
            dataDic = dataDic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            m_gameData.Clear();
            foreach (var item in dataDic)
            {
                m_gameData.Add(item.Key, item.Value);
            }
        }

        #endregion


        #region Create SO

        // 가져온 스프레드 시트 데이터를 스크립터블 오브젝트로 생성한다.
        public void CreateAllSO()
        {
            foreach (var gameData in m_gameData)
            {
                //생성해야하는지 먼저 체크
                if (CheckIgnoreSOSheet(gameData.Value))
                    continue;

                // 생성 또는 데이터를 업데이트한다.
                CreateSO(gameData.Value);
            }

        }


        // SO 생성 무시 해야하는지 체크
        // TRUE : ID 충돌. 무시해야함
        private bool CheckIgnoreSOSheet(GameData data)
        {
            foreach (var sheetID in ignoreSOWorksheets)
            {
                if (sheetID.Equals(data.SheetID))
                    return true;
            }
            return false;
        }

        // 불러오기 경로
        public string SoLoadPath(GameData data)
        {
            m_loadPath.Clear();
            m_loadPath.Append("Data/");
            m_loadPath.Append(data.SheetID);
            m_loadPath.Append("/");
            m_loadPath.Append(data.Name);
            return m_loadPath.ToString();
        }
        // 저장 경로
        public string SoSavePath(GameData data)
        {
            m_savePath.Clear();
            m_savePath.Append("Assets/Resources/Data/");
            m_savePath.Append(data.SheetID);
            m_savePath.Append("/");
            m_savePath.Append(data.Name);
            m_savePath.Append(".asset");
            return m_savePath.ToString();
        }

        // 가져온 SO를 유형에 맞게 생성해준다.
        private void CreateSO(GameData gameData)
        {
            bool createAsset = false;

            string loadPath = SoLoadPath(gameData);
            string savePath = SoSavePath(gameData);

            // 데이터 불러오기
            SOData data = Resources.Load<SOData>(loadPath);

            if (data == null)
            {
                createAsset = true;
                data = GetSOData(gameData);
            }

            data.SetData(gameData);

            if (createAsset)
                AssetDatabase.CreateAsset(data, savePath);


#if UNITY_EDITOR
            EditorUtility.SetDirty(data); // 데이터가 변경되었음을 알림

            // 저장 (에디터에서만 동작)
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#else
    Debug.LogError("SaveData는 에디터에서만 사용할 수 있습니다.");
#endif
        }


        // 각 데이터 컨테이너를 만들어준다.
        private SOData GetSOData(GameData gameData)
        {
            switch (gameData.SheetID)
            {
                case "ENEMY":
                    return GetEnemyData(gameData);

                case "FOOD POWER":
                    return ScriptableObject.CreateInstance<FoodPowerData>();

            }
            return null;

        }

        // 에네미 데이터를 가져올 때
        private SOData GetEnemyData(GameData gameData)
        {
            // 3번째가 BaseType
            switch (gameData.Data[3].Value)
            {
                case "EnemyA":
                    return ScriptableObject.CreateInstance<EnemyAData>();
                case "EnemyB":
                    return ScriptableObject.CreateInstance<EnemyBData>();
                case "EnemyC":
                    return ScriptableObject.CreateInstance<EnemyCData>();
                case "EnemyD":
                    return ScriptableObject.CreateInstance<EnemyDData>();
                case "EnemyE":
                    return ScriptableObject.CreateInstance<EnemyEData>();
                case "EnemyF":
                    return ScriptableObject.CreateInstance<EnemyFData>();
                case "EnemyG":
                    return ScriptableObject.CreateInstance<EnemyGData>();
                case "EnemyH":
                    return ScriptableObject.CreateInstance<EnemyHData>();
            }
            return ScriptableObject.CreateInstance<EnemyData>();
        }

        #endregion

        public void UpdateAllSO()
        {
            List<int> keyList = new List<int>();


            foreach (var gameData in m_gameData)
            {
                keyList.Add(gameData.Key);
            }

            // 데이터가 변경되므로 foreach 대신 for문으로 변경
            for(int i = 0; i < keyList.Count; i++)
            {
                var gameData = m_gameData[keyList[i]];

                //생성해야하는지 먼저 체크
                if (CheckIgnoreSOSheet(gameData))
                    continue;

                // 생성 또는 데이터를 업데이트한다.
                SoToGameData(gameData);
            }


        }
        private void SoToGameData(GameData gameData)
        {
            string loadPath = SoLoadPath(gameData);

            // 데이터 불러오기
            SOData data = Resources.Load<SOData>(loadPath);

            if (data == null)
            {
                Debug.Log(loadPath + " 경로의 데이터를 찾을 수 없습니다.");
                    return;
            }
            // 데이터 베이스의 게임 데이터를 업데이트한다.
            data.UpdateGameData();

        }

    }

#if UNITY_EDITOR
    [CustomEditor(typeof(DataReader))]
    public class DataReaderEditor : Editor
    {
        DataReader data;

        void OnEnable()
        {
            data = (DataReader)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Label("\n\n스프레드 시트");
            if (GUILayout.Button("데이터 가져오기 | 스프레드 시트 ▶ 데이터 베이스"))
            {
                ImportData();
            }
            //if (GUILayout.Button("데이터 내보내기 | 데이터 베이스 ▶ 스프레드 시트") && !Application.isPlaying)
            //{
            //    bool result = EditorUtility.DisplayDialog("Warning", "정말 업로드하시겠습니까?\nGame Data를 기준으로 구글 스프레드 시트의 데이터를 모두 덮어씁니다.", "덮어쓰기", "취소");
            //    if (result)
            //        ExportAllData();
            //}

            GUILayout.Label("\n\n스크립터블 오브젝트");
            if (GUILayout.Button("SO 데이터 생성 및 업데이트"))
            {
                CreateAllSO();
            }
            if (GUILayout.Button("데이터 내보내기 |  데이터 베이스 ▶ 스프레드 시트"))
            {
                ExportAllSO();
            }
        }


        void ImportData()
        {
            data.GameData.Clear();
            // 시트 전체를 읽어오고 시트별로 업데이트해준다.
            foreach (string sheet in data.associatedWorksheets)
            {
                UpdateSheets(UpdateAllMethod, sheet);
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(data); // 데이터가 변경되었음을 알림

            // 저장 (에디터에서만 동작)
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#else
    Debug.LogError("SaveData는 에디터에서만 사용할 수 있습니다.");
#endif
        }
        void UpdateSheets(UnityAction<GstuSpreadSheet> callback, string sheet, bool mergedCells = false)
        {
            SpreadsheetManager.Read(new GSTU_Search(data.associatedSheet, sheet), callback, mergedCells);
        }
        void UpdateAllMethod(GstuSpreadSheet ss)
        {
            int count = 0;
            for (int i = data.START_ROW_LENGTH; i <= 99; ++i)
            {
                if (ss.rows.ContainsKey(i) == false)
                {
                    break;
                }
                UpdateData(ss, i);
                count++;
            }

            EditorUtility.SetDirty(target);
        }
        // 시트 명에 따라 다른 리스트 업데이트
        void UpdateData(GstuSpreadSheet ss, int count)
        {
            data.UpdateGameData(ss.sheetID, ss.rows[count]);

            data.OrderbyDatabase();
        }

        void ExportAllData()
        {
            foreach (string sheet in data.associatedWorksheets)
            {
                data.ExportData(sheet);
            }
        }

        void CreateAllSO()
        {
            data.CreateAllSO();
        }

        void ExportAllSO()
        {
            // 데이터들을 각각 불러온뒤
            data.UpdateAllSO();
            //  시트에 맞게 내보낸다.
            ExportAllData();
        }
    }
#endif
}

