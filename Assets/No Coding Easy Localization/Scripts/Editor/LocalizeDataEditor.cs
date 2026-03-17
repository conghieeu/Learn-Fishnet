using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NoCodingEasyLocalization
{
    [CustomEditor(typeof(LocalizeData))]
    public class LocalizeDataEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LocalizeData ld = target as LocalizeData;

            EditorGUILayout.HelpBox("Please adjust \"Easy Localization/MasterData/LocalizeMaster\" for your project", MessageType.Info, true);

            List<SystemLanguage> thisLangList = ld.lm.langs;
            for (int i = 0; i < thisLangList.Count; i++)
            {
                ld.langStrList.Add(null);
                EditorGUILayout.LabelField(thisLangList[i].ToString(), EditorStyles.boldLabel);
                ld.langStrList[i] = EditorGUILayout.TextArea(ld.langStrList[i], GUILayout.Height(50));
            }
            EditorGUILayout.LabelField("Default Text", EditorStyles.boldLabel);
            ld.defaultText = EditorGUILayout.TextArea(ld.defaultText, GUILayout.Height(50));

            EditorUtility.SetDirty(target);
        }
    }
}
