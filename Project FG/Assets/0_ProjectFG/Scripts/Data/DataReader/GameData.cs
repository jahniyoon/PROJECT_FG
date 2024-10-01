using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
    public struct GameData
    {
        public string Name;
        public string SheetID;
        public List<GSTU_Data> Data;
        public GameData (string name, string SheetID, List<GSTU_Data> Data)
        {
            this.Name = name;
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
