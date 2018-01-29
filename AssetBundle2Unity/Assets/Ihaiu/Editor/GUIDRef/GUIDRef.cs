using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class GUIDRef 
{

    [MenuItem("Game/Analyze/生成GUID引用缓存 ")]  
    public static void GenerateGUIDRefCache()
    {
        GUIDRefCache cache = new GUIDRefCache();
        cache.Generate();
    }


    [MenuItem("Game/Analyze/打开GUID引用缓存 ")]
    public static void OpenGUIDRefCache()
    {
        cache.Open();
    }



    static GUIDRefCache _cache;
    static GUIDRefCache cache
    {
        get
        {
            if (_cache == null)
            {
                LoadGUIDRefCache();
            }
            return _cache;
        }
    }

    [MenuItem("Game/Analyze/加载GUID引用缓存 ")]
    public static void LoadGUIDRefCache()
    {
        if(!File.Exists(GUIDRefCache.outFile_data))
        {
            _cache =  new GUIDRefCache();
            return;
        }
        string json = File.ReadAllText(GUIDRefCache.outFile_data);
        _cache = JsonUtility.FromJson<GUIDRefCache>(json);
        _cache.List2Dict();
    }


}

public enum CacheFileRefType
{
    None,
    Resource,
    MResource,
    Editor,
    Ref,
    NoUse
}

public class CacheFileAssetType
{
    public const string Scene = "Scene";
    public const string Prefab = "Prefab";
    public const string Material = "Material";
    public const string Shader = "Shader";
    public const string Asset = "Asset";
    public const string Animation = "Animation";
    public const string Image = "Image";
    public const string Font = "Font";
    public const string Sound = "Sound";
    public const string Video = "Video";
    public const string Script = "Script";
    public const string Lua = "Lua";
    public const string Config = "Config";
    public const string Fbx = "Fbx";

    public static bool IsScriptOrConfig(string type)
    {
        return type == Script || type == Lua || type == Config;
    }


}


[System.Serializable]
public class GUIDRefCache
{
    [SerializeField]
    public List<CaheFile>               list;
    public Dictionary<string, CaheFile> dict = new Dictionary<string, CaheFile>();

    // 列表转字典
    public void List2Dict()
    {
        if(list != null)
        {
            foreach(CaheFile file in list)
            {
                dict.Add(file.guid, file);
            }


            foreach (CaheFile file in list)
            {
                foreach(string guid in file.refGUIDList)
                {
                    file.refList.Add(dict[guid]);
                }


                foreach (string guid in file.depGUIDList)
                {
                    file.depList.Add(dict[guid]);
                }
            }
        }

    }



    private List<string> unityExts = new List<string>(new string[]{ ".unity", ".prefab", ".mat",  ".asset"});
    private List<string> ignoreExts = new List<string>(new string[]{ ".meta", ".manifest"});
    private List<string> ignoreFiles = new List<string>(new string[]{ ".ds_store"});
    private List<string> scriptExts = new List<string>(new string[]{ ".cs", ".js", ".lua                       "});

    private static string outRoot              = "../GUIDRef/";
    public static string outFile_data     {   get     {   return outRoot + "data.json";   }   }
    public static string outFile_data_js  {   get     {   return outRoot + "data.js";   }   }
    public static string outFile_index    {   get     {   return outRoot + "index.html";   }   }

    private string guidEditorWebRoot    = "Assets/Ihaiu/Editor/GUIDRef/web/";
    private string[] copys = new string[]
        {   "index.html", 
            "app_data.javascript", "app_view.javascript", "config.javascript", "jquery.javascript", "main.javascript",
            "style1.css", "style2.css", "style3.css"
        };





    public string GetType(string ext)
    {
        switch(ext.ToLower())
        {
            case ".unity":
                return CacheFileAssetType.Scene;
            case ".prefab":
                return CacheFileAssetType.Prefab;
            case ".mat":
                return CacheFileAssetType.Material;
            case ".shader":
                return CacheFileAssetType.Shader;
            case ".asset":
                return CacheFileAssetType.Asset;
            case ".anim":
            case ".controller":
            case ".overrideController":
                return CacheFileAssetType.Animation;
            case ".png":
            case ".jpg":
            case ".jpeg":
            case ".bmp":
            case ".gif":
            case ".tga":
            case ".psd":
            case ".tiff":
                return CacheFileAssetType.Image;
            case ".ttf":
            case ".fnt":
            case ".fontsettings":
                return CacheFileAssetType.Font;
            case ".mp3":
            case ".wav":
            case ".aiff":
            case ".ogg":
                return CacheFileAssetType.Sound;
            case ".mov":
            case ".mpg":
            case ".mpeg":
            case ".mp4":
            case ".avi":
            case ".asf":
                return CacheFileAssetType.Video;
            case ".cs":
            case ".js":
                return CacheFileAssetType.Script;
            case ".lua":
                return CacheFileAssetType.Lua;
            case ".csv":
            case ".json":
            case ".txt":
                return CacheFileAssetType.Config;
            case ".fbx":
                return CacheFileAssetType.Fbx;
        }
        return ext;
    }

    public void Generate()
    {

        string mresourceRoot = "Assets/Game/MResources";
        string[] files;

        // MResources Unity
        if (Directory.Exists(mresourceRoot))
        {
            files = Directory.GetFiles(mresourceRoot, "*.*", SearchOption.AllDirectories)
            .Where(s => unityExts.Contains(Path.GetExtension(s).ToLower())).ToArray();
        
            GenerateFiles(files, CacheFileRefType.MResource);
        }

        // Resource Unity
        string[] resourcesDirs = Directory.GetDirectories("Assets", "Resources", SearchOption.AllDirectories);
        for(int i = 0; i < resourcesDirs.Length; i ++)
        {

            files = Directory.GetFiles(resourcesDirs[i], "*.*", SearchOption.AllDirectories)
                .Where(s => unityExts.Contains(Path.GetExtension(s).ToLower())).ToArray();
            GenerateFiles(files, CacheFileRefType.Resource);
        }

        // Editor
        string[] editorDirs = Directory.GetDirectories("Assets", "Editor", SearchOption.AllDirectories);
        for(int i = 0; i < editorDirs.Length; i ++)
        {
            files = Directory.GetFiles(editorDirs[i], "*.*", SearchOption.AllDirectories)
                .Where(s => Path.GetExtension(s).ToLower() != ".meta"  && !ignoreFiles.Contains(Path.GetFileName(s).ToLower()) ).ToArray();


            GenerateFiles(files, CacheFileRefType.Editor);
        }


        // Ref
        list = new List<CaheFile>(dict.Values);

        for(int i = 0; i < list.Count; i ++)
        {
            GenerateDependencies(list[i], CacheFileRefType.Ref);
        }

        // NoUse Unity
        files = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories)
            .Where(s => unityExts.Contains(Path.GetExtension(s).ToLower()) && !ignoreFiles.Contains(Path.GetFileName(s).ToLower()) ).ToArray();

        for(int i = 0; i < files.Length; i ++)
        {
            string guid = AssetDatabase.AssetPathToGUID(files[i]);
            if (dict.ContainsKey(guid))
            {
                continue;
            }
            
            CaheFile file = new CaheFile();
            file.filename   = files[i];
            file.guid       = guid;
            file.type       = GetType(Path.GetExtension(file.filename));
            if (!CacheFileAssetType.IsScriptOrConfig(file.type))
            {
                file.refType = CacheFileRefType.NoUse;
            }
            dict.Add(file.guid, file);

            GenerateDependencies(file, CacheFileRefType.NoUse);
        }

        // Other
        files = Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories)
            .Where(s => !ignoreExts.Contains(Path.GetExtension(s).ToLower()) && s.IndexOf("Assets/StreamingAssets") == -1 && !ignoreFiles.Contains(Path.GetFileName(s).ToLower())).ToArray();

        for(int i = 0; i < files.Length; i ++)
        {
            string guid = AssetDatabase.AssetPathToGUID(files[i]);
            if (dict.ContainsKey(guid))
            {
                continue;
            }

            CaheFile file = new CaheFile();
            file.filename   = files[i];
            file.guid       = guid;
            file.type       = GetType(Path.GetExtension(file.filename));
            if (file.filename.StartsWith(mresourceRoot))
            {
                file.refType = CacheFileRefType.MResource;
            }
            else if (file.filename.IndexOf("/Resources/") != -1)
            {
                file.refType = CacheFileRefType.Resource;
            }
            else
            {
                if (!CacheFileAssetType.IsScriptOrConfig(file.type))
                {
                    file.refType = CacheFileRefType.NoUse;
                }
            }
            dict.Add(file.guid, file);
        }







        list = new List<CaheFile>(dict.Values);
        for(int i = 0; i < list.Count; i ++)
        {
            list[i].ToGUIDList();
        }

        string json = JsonUtility.ToJson(this, true);

        if (!Directory.Exists(outRoot))
        {
            Directory.CreateDirectory(outRoot);
        }

        File.WriteAllText(outFile_data, json);
        File.WriteAllText(outFile_data_js, "var guidData = " + json);

        if (!File.Exists(outFile_index))
        {
            foreach(string filename in copys)
            {
                File.Copy(guidEditorWebRoot + filename, outRoot + filename.Replace(".javascript", ".js"));
            }
        }

        Open();
    }

    public void Open()
    {
        string url = "file:///" + Path.GetFullPath(outFile_index);
        Debug.Log(url);
        Application.OpenURL(url);
    }

    void GenerateFiles(string[] files, CacheFileRefType refType)
    {

        for(int i = 0; i < files.Length; i ++)
        {
            CaheFile file = new CaheFile();
            file.filename   = files[i];
            file.guid       = AssetDatabase.AssetPathToGUID(file.filename);
            file.type       = GetType(Path.GetExtension(file.filename));

            if (CacheFileAssetType.IsScriptOrConfig(file.type) && refType == CacheFileRefType.NoUse)
            {
            }
            else
            {
                file.refType = refType;
            }

            if (dict.ContainsKey(file.guid))
            {
                Debug.Log(refType + "  "+ file.filename);
                continue;
            }
            dict.Add(file.guid, file);
        }
    }

    void GenerateDependencies(CaheFile file, CacheFileRefType refType)
    {
        string[] dependencies = AssetDatabase.GetDependencies(file.filename, false);
        for(int i = 0; i < dependencies.Length; i ++)
        {
            string guid = AssetDatabase.AssetPathToGUID(dependencies[i]);
            bool has = dict.ContainsKey(guid);
            CaheFile def;
            if (has)
            {
                def = dict[guid];
            }
            else
            {
                def = new CaheFile();
                def.filename   = dependencies[i];
                def.guid       = guid;
                def.type       = GetType(Path.GetExtension(def.filename));
                if (def.refType == CacheFileRefType.None)
                {
                    if (CacheFileAssetType.IsScriptOrConfig(def.type) && refType == CacheFileRefType.NoUse)
                    {
                    }
                    else
                    {
                        def.refType = refType;
                    }
                }
                dict.Add(def.guid, def);
            }


            file.AddDep(def);
            def.AddRef(file);

            if (!has)
            {
                GenerateDependencies(def, refType);
            }
        }
    }
}


[System.Serializable]
public class CaheFile
{
    public string               guid;
    public string               filename;
    public string               type;
    public CacheFileRefType     refType;

    [SerializeField]
    public List<string> refGUIDList = new List<string>();
    [SerializeField]
    public List<string> depGUIDList = new List<string>();

    /** 被引用列表 */
    [System.NonSerialized]
    public List<CaheFile> refList = new List<CaheFile>();
    /** 依赖列表 */
    [System.NonSerialized]
    public List<CaheFile> depList = new List<CaheFile>();

    public void AddRef(CaheFile item)
    {
        if (!refList.Contains(item))
            refList.Add(item);
    }

    public void AddDep(CaheFile item)
    {
        if (!depList.Contains(item))
            depList.Add(item);
    }


    public void ToGUIDList()
    {
        foreach(CaheFile item in refList)
        {
            refGUIDList.Add(item.guid);
        }


        foreach(CaheFile item in depList)
        {
            depGUIDList.Add(item.guid);
        }
    }
}