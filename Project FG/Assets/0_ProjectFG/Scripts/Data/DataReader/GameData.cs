using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public struct GameData
    {
        public string name;
        public string SheetID;
        public List<GSTU_Data> Data;
        public GameData (string name, string SheetID, List<GSTU_Data> Data)
        {
            this.name = name;
            this.SheetID = SheetID;
            this.Data = Data;
        }
    }
    [System.Serializable]
    public class GSTU_Data
    {
        public string ColumnID;
        public string Value;
    }
}
