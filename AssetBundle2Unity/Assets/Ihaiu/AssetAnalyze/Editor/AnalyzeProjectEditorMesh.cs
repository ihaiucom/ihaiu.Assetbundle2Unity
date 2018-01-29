using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ihaiu.Editors.AnalyzeProjects
{
    public class AnalyzeProjectEditorMesh
    {

        public static void Report(AnalyzeProjectInfo project)
        {
            string[] guids = AssetDatabase.FindAssets("t:Mesh", null);

            Type type = typeof(Mesh);
            foreach(string guid in guids)
            {
                AnalyzeObjectInfo objectInfo = project.Get(guid);
                if(objectInfo == null)
                {
                    objectInfo = new AnalyzeObjectInfo();
                    objectInfo.guid = guid;
                    objectInfo.type = type;
                    objectInfo.path = AssetDatabase.GUIDToAssetPath(guid);



                    var importer = AssetImporter.GetAtPath(objectInfo.path) as ModelImporter;
                    if (importer == null)
                    {
                        continue;
                    }

                    var mesh = AssetDatabase.LoadAssetAtPath<Mesh>(objectInfo.path);

                    objectInfo.propertys = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(AnalyzePropertys.VertexCount, mesh.vertexCount),
                        new KeyValuePair<string, object>(AnalyzePropertys.TriangleCount, (mesh.triangles.Length / 3f)),
                        new KeyValuePair<string, object>(AnalyzePropertys.SubMeshCount, mesh.subMeshCount),
                        new KeyValuePair<string, object>(AnalyzePropertys.ScaleFactor, importer.globalScale),
                        new KeyValuePair<string, object>(AnalyzePropertys.UseFileUnits, importer.useFileUnits),
                        new KeyValuePair<string, object>(AnalyzePropertys.FileScale, importer.fileScale),
                        new KeyValuePair<string, object>(AnalyzePropertys.MeshCompression, importer.meshCompression.ToString()),
                        new KeyValuePair<string, object>(AnalyzePropertys.ReadWrite, importer.isReadable),
                        new KeyValuePair<string, object>(AnalyzePropertys.OptimizeMesh, importer.optimizeMesh),
                        new KeyValuePair<string, object>(AnalyzePropertys.OptimizeGameObjects, importer.optimizeGameObjects),
                        new KeyValuePair<string, object>(AnalyzePropertys.GenerateCollider, importer.addCollider),
                        new KeyValuePair<string, object>(AnalyzePropertys.ImportBlendShapes, importer.importBlendShapes),
                        new KeyValuePair<string, object>(AnalyzePropertys.ImportMaterials, importer.importMaterials),
                        new KeyValuePair<string, object>(AnalyzePropertys.TransformPathsCount, importer.transformPaths.Length),
                        new KeyValuePair<string, object>(AnalyzePropertys.ExtraExposedTransformPathsCount, importer.extraExposedTransformPaths.Length),
                    };

                    project.Add(objectInfo);

                }
            }
        }


        private static Texture GetTextureSize(TextureImporter import, out int w, out int h)
        {
            w = h = 0;
            var tex = AssetDatabase.LoadAssetAtPath<Texture>(import.assetPath);
            if (tex != null)
            {
                w = tex.width;
                h = tex.height;
            }
            return tex;
        }

        private static bool IsPow2Size(int width, int height)
        {
            bool wFlag = false;
            bool hFlag = false;
            for (int i = 0; i < 31; ++i)
            {
                int tmp = (1 << i);
                wFlag |= (tmp == width);
                hFlag |= (tmp == height);
                if (tmp >= width && tmp >= height)
                {
                    break;
                }
            }
            return (wFlag & hFlag);
        }

    }
}