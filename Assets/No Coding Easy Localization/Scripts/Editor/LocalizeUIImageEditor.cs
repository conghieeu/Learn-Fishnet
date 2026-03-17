using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace NoCodingEasyLocalization
{
    [CustomEditor(typeof(LocalizeUIImage))]
    public class LocalizeUIImageEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            LocalizeUIImage imageManager = target as LocalizeUIImage;

            EditorGUILayout.HelpBox("Please adjust \"Easy Localization/MasterData/LocalizeMaster\" for your project", MessageType.Info, true);

            List<SystemLanguage> thisLangList = imageManager.LocalizeMaster.langs;
            for (int i = 0; i < thisLangList.Count; i++)
            {
                imageManager.langSpriteList.Add(null);
                EditorGUILayout.LabelField(thisLangList[i].ToString(), EditorStyles.boldLabel);
                imageManager.langSpriteList[i] = (Sprite)EditorGUILayout.ObjectField("Sprite", imageManager.langSpriteList[i], typeof(Sprite), allowSceneObjects: true);
            }
            EditorGUILayout.LabelField("Default Sprite", EditorStyles.boldLabel);
            imageManager.defaultSprite = (Sprite)EditorGUILayout.ObjectField("Sprite", imageManager.defaultSprite, typeof(Sprite), allowSceneObjects: true);

            EditorUtility.SetDirty(target);
        }
    }
}
