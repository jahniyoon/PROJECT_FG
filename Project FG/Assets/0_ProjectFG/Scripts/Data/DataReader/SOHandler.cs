using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace JH
{
    public static class SOHandler
    {
        public static StringBuilder m_savePath = new StringBuilder();
        public static StringBuilder m_loadPath = new StringBuilder();

        // 각 데이터 컨테이너를 만들어준다.
        public static SOData GetSOData(GameData gameData)
        {
            switch (gameData.SheetID)
            {
                case "ENEMY":
                    return GetEnemyData(gameData);

                case "FOOD POWER":
                    return ScriptableObject.CreateInstance<FoodPowerData>();

                case "SKILL":
                    return GetSkillData(gameData);

                case "BUFF":
                    return ScriptableObject.CreateInstance<BuffData>();

                case "PROJECTILE":
                    return ScriptableObject.CreateInstance<ProjectileData>();
            }
            return null;
        }

        // 에네미 데이터를 가져올 때
        public static SOData GetEnemyData(GameData gameData)
        {
            // 3번째가 BaseType
            switch (gameData.Data[3].Value)
            {
                //case "EnemyA":
                //    return ScriptableObject.CreateInstance<EnemyAData>();
                //case "EnemyB":
                //    return ScriptableObject.CreateInstance<EnemyBData>();
                //case "EnemyC":
                //    return ScriptableObject.CreateInstance<EnemyCData>();
                //case "EnemyD":
                //    return ScriptableObject.CreateInstance<EnemyDData>();
                //case "EnemyE":
                //    return ScriptableObject.CreateInstance<EnemyEData>();
                //case "EnemyF":
                //    return ScriptableObject.CreateInstance<EnemyFData>();
                //case "EnemyG":
                //    return ScriptableObject.CreateInstance<EnemyGData>();
                //case "EnemyH":
                //    return ScriptableObject.CreateInstance<EnemyHData>();
            }
            return ScriptableObject.CreateInstance<EnemyData>();
        }

        public static SOData GetSkillData(GameData gameData)
        {
            // 3번째가 BaseType
            //switch (gameData.Data[3].Value)
            //{
               
            //}
            return ScriptableObject.CreateInstance<SkillData>();
        }


        public static void SoToGameData(GameData gameData)
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

        // 불러오기 경로
        public static string SoLoadPath(GameData data)
        {
            m_loadPath.Clear();
            m_loadPath.Append("Data/");
            m_loadPath.Append(data.SheetID);
            m_loadPath.Append("/");
            m_loadPath.Append(data.Name);
            return m_loadPath.ToString();
        }
        // 저장 경로
        public static string SoSavePath(GameData data)
        {
            m_savePath.Clear();
            m_savePath.Append("Assets/Resources/Data/");
            m_savePath.Append(data.SheetID);
            m_savePath.Append("/");
            m_savePath.Append(data.Name);
            m_savePath.Append(".asset");
            return m_savePath.ToString();
        }
    }
}
