using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
/** 
* ==============================================================================
*  @Author      	曾峰(zengfeng75@qq.com) 
*  @Web      		http://blog.ihaiu.com
*  @CreateTime      1/18/2018 11:13:00 AM
*  @Description:    
* ==============================================================================
*/
namespace Ihaiu.Editors.AnalyzeProjects
{
    public class AnalyzeProjectEditorForUnit
    {
        public static void Report(AnalyzeProjectInfo project, List<string> roots)
        {
            List<string> files = new List<string>();
            string[] arr;
            foreach(string root in roots)
            {
                if (!Directory.Exists(root))
                    continue;
                arr = Directory.GetFiles(root, "*.Prefab", SearchOption.AllDirectories);
                files.AddRange(arr);
            }

            for(int i = 0; i < files.Count; i ++)
            {
                if(i % 30 == 0)
                EditorUtility.DisplayProgressBar("AnalyzeProjectEditorForUnit", "AnalyzeProjectEditorForUnit" + i + "/" + files.Count, i / (files.Count * 1f));

                string path = files[i];
                Object o = AssetDatabase.LoadAssetAtPath(path, typeof(Object) );
                string guid = AssetDatabase.AssetPathToGUID(path);

                AnalyzeObjectInfo objectInfo = project.Get(guid);

                if (objectInfo == null)
                {
                    objectInfo = new AnalyzeObjectInfo();
                    objectInfo.guid = guid;
                    objectInfo.path = path;
                    objectInfo.type = o.GetType();
                    objectInfo.fileInfo = AnalyzeFileAsset.Generate(o);
                    AnalyzeFileInfo f = objectInfo.fileInfo;
                    
                    objectInfo.propertys = new List<KeyValuePair<string, object>>();
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.MaterialCount, f.MaterialCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.Texture2DCount, f.Texture2DCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.TotalTriangleCount, f.TotalTriangleCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.TotalVertexCount, f.TotalVertexCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.BonesCount, f.BonesCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.ParticleSystemCount, f.ParticleSystemCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.TotalForecastParticleCount, f.TotalForecastParticleCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.TotalForecastTrianglesCount, f.TotalForecastTrianglesCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.MeshFilterCount, f.MeshFilterCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.SkinnedMeshRendererCount, f.SkinnedMeshRendererCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.MeshRendererCount, f.MeshRendererCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.AnimatorCount, f.AnimatorCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.TransformCount, f.TransformCount));
                    objectInfo.propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.ObjectCount, f.ObjectCount));

                    project.Add(objectInfo);
                }
            }

        }
    }
}
