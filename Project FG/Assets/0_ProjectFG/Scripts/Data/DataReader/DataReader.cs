using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleSheetsToUnity;

using UnityEngine.Events;
using Newtonsoft.Json;
using System.Text;
using UnityEngine.Rendering;
using UnityEditor.AssetImporters;
using System.Linq;



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
                for(int i = 0; i < item.Value.Data.Count; i++)
                {
                    list.Add(item.Value.Data[i].Value);
                }
                Combined.Add(list);
                //SpreadsheetManager.Write(new GSTU_Search(associatedSheet, sheet, StartCell), new ValueRange(list), null);
                count++;
            }
               SpreadsheetManager.Write(new GSTU_Search(associatedSheet, sheet, StartCell), new ValueRange(Combined), null);
        }




        public void OrderbyDatabase()
        {
            Dictionary<int, GameData> dataDic = m_gameData;
            dataDic = dataDic.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            m_gameData.Clear();
            foreach (var item in dataDic)
            {
                m_gameData.Add(item.Key,item.Value);
            }

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

            GUILayout.Label("\n\nLocal Game Data");
            if (GUILayout.Button("스프레드 시트 데이터 가져오기"))
            {
                ImportData();
            }
            if (GUILayout.Button("스프레드 시트로 데이터 내보내기")   && !Application.isPlaying)
            {
                bool result = EditorUtility.DisplayDialog("Warning", "정말 업로드하시겠습니까?\nGame Data를 기준으로 구글 스프레드 시트의 데이터를 모두 덮어씁니다.", "덮어쓰기", "취소");
                if(result)
                ExportAllData();
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
    }
#endif
}

