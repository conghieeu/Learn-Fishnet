using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NoCodingEasyLocalization
{
    [CreateAssetMenu(fileName = "LocalizeData", menuName = "Tools/NoCodingEasyLocalization/LocalizeData", order = 1)]
    public class LocalizeData : ScriptableObject
    {
        public LocalizeMaster lm = null;
        public List<string> langStrList = new List<string>();
        public string defaultText = "";
    }
}
