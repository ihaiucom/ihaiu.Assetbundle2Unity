using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ihaiu.Editors
{
    public class AnalyzeFileAssetMenu
    {
        [MenuItem("GameObject/分析资源详情", false, 1)]
        [MenuItem("Assets/分析资源详情", false, 1)]
        public static void AnalyzeMenu()
        {
            AnalyzeFileInfo info = AnalyzeFileAsset.Generate(Selection.activeObject);
            AnalyzeFileInfoWindow.Open(info);
        }


        [MenuItem("GameObject/分析资源详情", true)]
        [MenuItem("Assets/分析资源详情", true)]
        public static bool AnalyzeMenu_Validate()
        {
            return Selection.objects.Length > 0;
        }
    }

}