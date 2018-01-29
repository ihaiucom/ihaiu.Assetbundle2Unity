using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ihaiu.Editors.AnalyzeProjects
{
    public class AnalyzeProjectEditorTexture
    {

        public static void Report(AnalyzeProjectInfo project)
        {
            string[] guids = AssetDatabase.FindAssets("t:texture2D", null);

            Type type = typeof(Texture2D);
            foreach(string guid in guids)
            {
                AnalyzeObjectInfo objectInfo = project.Get(guid);
                if(objectInfo == null)
                {
                    objectInfo = new AnalyzeObjectInfo();
                    objectInfo.guid = guid;
                    objectInfo.type = type;
                    objectInfo.path = AssetDatabase.GUIDToAssetPath(guid);



                    var importer = AssetImporter.GetAtPath(objectInfo.path) as TextureImporter;
                    if (importer == null)
                    {
                        continue;
                    }

                    string format = "未知";
                    int w, h;
                    var tex = GetTextureSize(importer, out w, out h) as Texture2D;
                    if (tex != null)
                    {
                        format = tex.format.ToString();
                    }

                    objectInfo.propertys = new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(AnalyzePropertys.Width, w),
                        new KeyValuePair<string, object>(AnalyzePropertys.Height, h),
                        new KeyValuePair<string, object>(AnalyzePropertys.IsPow2Size, IsPow2Size(w, h)),
                        new KeyValuePair<string, object>(AnalyzePropertys.MaxTextureSize, importer.maxTextureSize),
                        new KeyValuePair<string, object>(AnalyzePropertys.ReadWrite, importer.isReadable),
                        new KeyValuePair<string, object>(AnalyzePropertys.MiniMap, importer.mipmapEnabled),
                        new KeyValuePair<string, object>(AnalyzePropertys.TextureFormat, format),
                        new KeyValuePair<string, object>(AnalyzePropertys.TextureCompression, importer.textureCompression.ToString()),
                        new KeyValuePair<string, object>(AnalyzePropertys.SpriteImportMode, importer.spriteImportMode.ToString()),
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