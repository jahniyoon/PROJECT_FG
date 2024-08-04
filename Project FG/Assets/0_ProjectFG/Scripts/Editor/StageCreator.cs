using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace JH
{
    [CustomEditor(typeof(LevelManager))]
    public class LevelManagerButton : Editor
    {
        public override void OnInspectorGUI() 
        { 
            base.OnInspectorGUI();
            LevelManager levelManager = (LevelManager)target;

            GUILayout.Label("\n");

            if (GUILayout.Button("Create Stage")) 
            { levelManager.CreateStage(); } }
    }
}