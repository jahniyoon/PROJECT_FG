using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JH
{
    [System.Serializable]
	public class SOData : ScriptableObject
	{
	        
        public virtual void SetData(GameData gamedata)
        {

        }

        public virtual void UpdateGameData()
        {

        }

        protected GSTU_Data SetData(string ColumnID, string Value)
        {
            GSTU_Data data = new GSTU_Data();
            data.ColumnID = ColumnID;
            data.Value = Value;
            return data;
        }
    }
}
