using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NoCodingEasyLocalization
{
    public class MasterWindow : EditorWindow
    {
        public LocalizeMaster LocalizeMaster;
        [MenuItem("Tools/NoCodingEasyLocalization")]
        static void Open()
        {
            var window = GetWindow<MasterWindow>();
            window.titleContent = new GUIContent("No Coding Easy Localization");
        }

        void OnGUI()
        {
            EditorGUILayout.LabelField("Languages", EditorStyles.boldLabel);
            Editor.CreateEditor(LocalizeMaster).DrawDefaultInspector();
        }
    }
}
