using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JH
{

    public class DataReaderBase : ScriptableObject
    {
        // 구글 시트의 URL
        [Header("Sheets URL")]
        [HideInInspector][SerializeField] public string associatedSheet = "";

        // 시트의 이름
        [Header("Sheet Name")]
        [SerializeField] public string[] associatedWorksheets;

        // 시트의 시작하는 칸
        [Header("Index Length")]
        [SerializeField] public int START_ROW_LENGTH = 3;

        // SO를 만들 필요가 없는 시트 이름
        [Header("Ignore SO Sheet Name")]

        [SerializeField] public string[] ignoreSOWorksheets;

    }
}