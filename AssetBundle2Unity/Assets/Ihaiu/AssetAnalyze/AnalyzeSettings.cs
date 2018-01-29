using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class AnalyzeSettings : ScriptableObject
{
    const string AssetName = "Settings/AnalyzeSettings";

    private static AnalyzeSettings instance = null;
    public static AnalyzeSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load(AssetName) as AnalyzeSettings;
                if (instance == null)
                {
                    UnityEngine.Debug.Log("没有找到AnalyzeSettings");
                    instance = CreateInstance<AnalyzeSettings>();
                    instance.name = "AnalyzeSettings";
                    instance.unitRoots = new List<string> { "Assets/Game/Resources/PrefabUnit"};
                    instance.fxRoots = new List<string> { "Assets/Game/Resources/PrefabFx" };
                    instance.mapRoots = new List<string> { "Assets/Game/ScenesStage" };
                    instance.uiPrefabRoots = new List<string> { "Assets/Game/Resources/PrefabUI" };
                    instance.imageRoots = new List<string> { "Assets/Game/Resources/ImageSprites",
                                                               "Assets/Game/Resources/ImageTextures"};


#if UNITY_EDITOR
                    string path = "Assets/Game/Resources/" + AssetName + ".asset";
                    CheckPath(path);
                    AssetDatabase.CreateAsset(instance, path);
#endif
                }
            }
            return instance;
        }
    }


    public static void CheckPath(string path, bool isFile = true)
    {
        if (isFile) path = path.Substring(0, path.LastIndexOf('/'));
        string[] dirs = path.Split('/');
        string target = "";

        bool first = true;
        foreach (string dir in dirs)
        {
            if (first)
            {
                first = false;
                target += dir;
                continue;
            }

            if (string.IsNullOrEmpty(dir)) continue;
            target += "/" + dir;
            if (!Directory.Exists(target))
            {
                Directory.CreateDirectory(target);
            }
        }
    }


#if UNITY_EDITOR
    public static void EditSettings()
    {
        Selection.activeObject = Instance;
        EditorApplication.ExecuteMenuItem("Window/Inspector");
    }

#endif


    [Header("单位路径")]
    public List<string> unitRoots = new List<string>();

    [Header("特效路径")]
    public List<string> fxRoots = new List<string>();

    [Header("地图路径")]
    public List<string> mapRoots = new List<string>();

    [Header("UI预设路径")]
    public List<string> uiPrefabRoots = new List<string>();

    [Header("动态UI图片路径")]
    public List<string> imageRoots = new List<string>();
}


