using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Ihaiu.Editors
{
    public class AnalyzeFileAsset
    {
        public static AnalyzeFileInfo Generate(Object obj)
        {
            AnalyzeFileInfo info = new AnalyzeFileInfo();
            info.obj = obj;
            info.type = obj.GetType();
            info.path = AssetDatabase.GetAssetPath(obj);

            Dictionary<Object, SerializedObject> dict = AnalyzeFileAsset.AnalyzeObjectReference(obj);

            foreach (var kvp in dict)
            {
                AnalyzeObjectInfo item = new AnalyzeObjectInfo();
                item.obj = kvp.Key;
                item.type = item.obj.GetType();
                item.path = AssetDatabase.GetAssetPath(item.obj);
                item.guid = AssetDatabase.AssetPathToGUID(item.path);
                item.propertys = AnalyzeFileAsset.AnalyzeObject(kvp.Key, kvp.Value);
                info.Add(item);
            }


            GameObject go = obj as GameObject;
            if (go != null)
            {
                SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(true);
                foreach (SkinnedMeshRenderer item in skinnedMeshRenderers)
                {
                    Mesh mesh = item.sharedMesh;
                    if (mesh != null)
                    {
                        info.TotalTriangleCount +=(int) (mesh.triangles.Length / 3f);
                        info.TotalVertexCount += mesh.vertexCount;
                    }

                    info.BonesCount += item.bones.Length;
                }


                MeshFilter[] meshFilters = go.GetComponentsInChildren<MeshFilter>(true);
                foreach (MeshFilter item in meshFilters)
                {
                    Mesh mesh = item.sharedMesh;
                    if (mesh != null)
                    {
                        info.TotalTriangleCount += (int)(mesh.triangles.Length / 3f);
                        info.TotalVertexCount += mesh.vertexCount;
                    }
                }


                ParticleSystem[] ParticleSystem = go.GetComponentsInChildren<ParticleSystem>(true);
                foreach (ParticleSystem particleSystem in ParticleSystem)
                {
                    info.TotalForecastParticleCount += particleSystem.GetForecastParticleCount();
                    info.TotalForecastTrianglesCount += particleSystem.GetForecastTrianglesCount();

                }


            }


            return info;
        }




        #region 分析对象的引用
        /// <summary>
        /// 分析对象的引用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>引用的对象</returns>
        public static Dictionary<Object, SerializedObject> AnalyzeObjectReference(Object obj)
        {
            Dictionary<Object, SerializedObject> dict = new Dictionary<Object, SerializedObject>();
            AnalyzeObjectReference(obj, ref dict);
            AnalyzeObjectComponent(obj, ref dict);
            return dict;
        }

        private static PropertyInfo inspectorMode;
        public static void AnalyzeObjectReference(Object obj, ref Dictionary<Object, SerializedObject> dict)
        {
            if (obj == null) return;
            var serializedObject = new SerializedObject(obj);
            if(!dict.ContainsKey(obj))
            {
                dict.Add(obj, serializedObject);
            }
            else
            {
                //Debug.Log("dict Contains: " + obj);
            }

            if (inspectorMode == null)
            {
                inspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            inspectorMode.SetValue(serializedObject, InspectorMode.Debug, null);


            var it = serializedObject.GetIterator();
            while (it.NextVisible(true))
            {
                if (it.propertyType == SerializedPropertyType.ObjectReference && it.objectReferenceValue != null)
                {
                    AnalyzeObjectReference(it.objectReferenceValue, ref dict);
                }
            }


            // 只能用另一种方式获取的引用
            AnalyzeObjectReference2(obj, ref dict);
        }


        /// <summary>
        /// 动画控制器比较特殊，不能通过序列化得到
        /// </summary>
        /// <param name="info"></param>
        /// <param name="o"></param>
        private static void AnalyzeObjectReference2(Object o, ref Dictionary<Object, SerializedObject> dict)
        {
            AnimatorController ac = o as AnimatorController;
            if (ac)
            {
                foreach (var clip in ac.animationClips)
                {
                    AnalyzeObjectReference(clip, ref dict);
                }
            }
        }
        #endregion


        #region 分析Component引用
        /// <summary>
        /// 分析脚本的引用（这只在脚本在工程里时才有效）
        /// </summary>
        /// <param name="info"></param>
        /// <param name="o"></param>
        public static void AnalyzeObjectComponent (Object o, ref Dictionary<Object, SerializedObject> dict)
        {
            var go = o as GameObject;
            if (!go)
            {
                return;
            }

            var components = go.GetComponentsInChildren<Component>(true);
            foreach (var component in components)
            {
                if (!component)
                {
                    continue;
                }

                AnalyzeObjectReference(component, ref dict);
            }
        }

        #endregion



        public static List<KeyValuePair<string, object>> AnalyzeObject(Object o, SerializedObject serializedObject)
        {
            Texture2D tex = o as Texture2D;
            if (tex)
            {
                //ExportTexture2D(tex, rootPath, name);
                return AnalyzeTexture2D(tex, serializedObject);
            }

            Mesh mesh = o as Mesh;
            if (mesh)
            {
                return AnalyzeMesh(mesh, serializedObject);
            }

            Material mat = o as Material;
            if (mat)
            {
                return AnalyzeMaterial(mat, serializedObject);
            }

            AudioClip audioClip = o as AudioClip;
            if (audioClip)
            {
                return AnalyzeAudioClip(audioClip, serializedObject);
            }

            AnimationClip clip = o as AnimationClip;
            if (clip)
            {
                return AnalyzeAnimationClip(clip, serializedObject);
            }


            SkinnedMeshRenderer skinnedMeshRenderer = o as SkinnedMeshRenderer;
            if (skinnedMeshRenderer)
            {
                return AnalyzeSkinnedMeshRenderer(skinnedMeshRenderer, serializedObject);
            }

            ParticleSystem particleSystem = o as ParticleSystem;
            if (particleSystem)
            {
                return AnalyzeParticleSystem(particleSystem, serializedObject);
            }



            //Transform transform = o as Transform;
            //if (transform)
            //{
            //    if(transform.name == "Bip001")
            //    {
            //        return AnalyzeBones(transform);
            //    }
            //}

            return null;
        }


        private static List<KeyValuePair<string, object>> AnalyzeRoot(GameObject go)
        {
            if(go.name == "Bip001")
            {
                return AnalyzeBones(go.transform);
            }
            else
            {
                Transform[] nodes = go.GetComponentsInChildren<Transform>();
                foreach(Transform item in nodes)
                {
                    if(item.name == "Bip001")
                    {
                        return AnalyzeBones(item);
                    }
                }
            }


            return null;
        }

        private static List<KeyValuePair<string, object>> AnalyzeBones(Transform transform)
        {
            Transform[] nodes = transform.GetComponentsInChildren<Transform>(true);
            var propertys = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>("骨骼数量", nodes.Length),
            };
            return propertys;
        }

        public static int GetBoonesCount(Object o)
        {

            Transform transform = o as Transform;
            if (transform)
            {
                return GetBoonesCount(transform);
            }


            GameObject go = o as GameObject;
            if (go)
            {
                return GetBoonesCount(go);
            }
            return 0;
        }
        public static int GetBoonesCount(GameObject go)
        {
            return GetBoonesCount(go.transform);
        }
        public static int GetBoonesCount(Transform transform)
        {
            int num = 0;
            Animator[] animators = transform.GetComponentsInChildren<Animator>(true);
            Dictionary<Transform, bool> dict = new Dictionary<Transform, bool>();
            foreach(Animator animator in animators)
            {
                Transform[] nodes = animator.transform.GetComponentsInChildren<Transform>(true);
                foreach(Transform node in nodes)
                {
                    if (dict.ContainsKey(node))
                        continue;

                    Component[] compontents = node.GetComponents<Component>();
                    if(compontents.Length == 1)
                    {
                        num++;
                    }

                    dict.Add(node, true);
                }
            }

            return num;

            //Transform root = null;
            //if (transform.name == "Bip001")
            //{
            //    root = transform;
            //}
            //else
            //{
            //    Transform[] nodes = transform.GetComponentsInChildren<Transform>();
            //    foreach (Transform item in nodes)
            //    {
            //        if (item.name == "Bip001")
            //        {
            //            root = item;
            //        }
            //    }
            //}

            //if(root != null)
            //{
            //    Transform[] nodes = root.GetComponentsInChildren<Transform>();
            //    return nodes.Length;
            //}

            //return 0;
        }


        private static List<KeyValuePair<string, object>> AnalyzeSkinnedMeshRenderer(SkinnedMeshRenderer tex, SerializedObject serializedObject)
        {
            var propertys = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(AnalyzePropertys.BonesCount, tex.bones.Length),
                new KeyValuePair<string, object>(AnalyzePropertys.SkinQuality, tex.quality.ToString()),
                new KeyValuePair<string, object>(AnalyzePropertys.SkinnedMotionVectors, tex.skinnedMotionVectors),
                new KeyValuePair<string, object>(AnalyzePropertys.UpdateWhenOffscreen, tex.updateWhenOffscreen),
                new KeyValuePair<string, object>(AnalyzePropertys.ReceiveShadows, tex.receiveShadows),
                new KeyValuePair<string, object>(AnalyzePropertys.ReflectionProbeUsage, tex.reflectionProbeUsage.ToString()),
                new KeyValuePair<string, object>(AnalyzePropertys.ShadowCastingMode, tex.shadowCastingMode.ToString()),
                new KeyValuePair<string, object>(AnalyzePropertys.LightProbeUsage, tex.lightProbeUsage.ToString()),
            };


            return propertys;
        }

        private static List<KeyValuePair<string, object>> AnalyzeTexture2D(Texture2D tex, SerializedObject serializedObject)
        {
            var propertys = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(AnalyzePropertys.Width, tex.width),
                new KeyValuePair<string, object>(AnalyzePropertys.Height, tex.height),
                new KeyValuePair<string, object>(AnalyzePropertys.Format, tex.format.ToString()),
                new KeyValuePair<string, object>(AnalyzePropertys.MiniMap, tex.mipmapCount > 1 ? "True" : "False")
            };

            var property = serializedObject.FindProperty("m_IsReadable");
            propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.ReadWrite, property.boolValue.ToString()));

            property = serializedObject.FindProperty("m_CompleteImageSize");
            propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.MemorySize, property.intValue));

            return propertys;
        }

        private static List<KeyValuePair<string, object>> AnalyzeMesh(Mesh mesh, SerializedObject serializedObject)
        {
            var propertys = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(AnalyzePropertys.VertexCount, mesh.vertexCount),
                new KeyValuePair<string, object>(AnalyzePropertys.TriangleCount, (mesh.triangles.Length / 3f)),
                new KeyValuePair<string, object>(AnalyzePropertys.SubMeshCount, mesh.subMeshCount),
                new KeyValuePair<string, object>(AnalyzePropertys.MeshCompression, MeshUtility.GetMeshCompression(mesh).ToString()),
                new KeyValuePair<string, object>(AnalyzePropertys.ReadWrite, mesh.isReadable.ToString())
            };
            return propertys;
        }


        private static List<KeyValuePair<string, object>> AnalyzeMaterial(Material mat, SerializedObject serializedObject)
        {
            var propertys = new List<KeyValuePair<string, object>>
            {
            };

            string texNames = System.String.Empty;

            var property = serializedObject.FindProperty("m_Shader");
            propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.Shader, property.objectReferenceValue ? property.objectReferenceValue.name : "[其他AB内]"));

            string[] dependencies = AssetDatabase.GetDependencies(AssetDatabase.GetAssetPath(mat), false);
            foreach(string dependencie in dependencies)
            {
                string ext = Path.GetExtension(dependencie);
                if (ext == ".shader")
                    continue;


                if (!string.IsNullOrEmpty(texNames))
                {
                    texNames += ", ";
                }
                texNames += Path.GetFileName(dependencie);
            }

            //property = serializedObject.FindProperty("m_SavedProperties");
            //var property2 = property.FindPropertyRelative("m_TexEnvs");
            //foreach (SerializedProperty property3 in property2)
            //{
            //    SerializedProperty property4 = property3.FindPropertyRelative("second");
            //    SerializedProperty property5 = property4.FindPropertyRelative("m_Texture");

            //    if (property5.objectReferenceValue)
            //    {
            //        if (!string.IsNullOrEmpty(texNames))
            //        {
            //            texNames += ", ";
            //        }
            //        texNames += property5.objectReferenceValue.name;
            //    }
            //    else
            //    {
            //        if (!string.IsNullOrEmpty(texNames))
            //        {
            //            texNames += ", ";
            //        }
            //        texNames += "[其他AB内]";
            //    }
            //}
            propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.Texture, texNames));

            return propertys;
        }

        private static List<KeyValuePair<string, object>> AnalyzeAudioClip(AudioClip audioClip, SerializedObject serializedObject)
        {
            var propertys = new List<KeyValuePair<string, object>>
            {
#if UNITY_5 || UNITY_5_3_OR_NEWER
                new KeyValuePair<string, object>(AnalyzePropertys.LoadType, audioClip.loadType.ToString()),
                new KeyValuePair<string, object>(AnalyzePropertys.PreloadAudioData, audioClip.preloadAudioData.ToString()),
#endif
                new KeyValuePair<string, object>(AnalyzePropertys.Frequency, audioClip.frequency),
                new KeyValuePair<string, object>(AnalyzePropertys.Length, audioClip.length)
            };

#if UNITY_5 || UNITY_5_3_OR_NEWER
            var property = serializedObject.FindProperty("m_CompressionFormat");
            propertys.Add(new KeyValuePair<string, object>(AnalyzePropertys.Format, ((AudioCompressionFormat)property.intValue).ToString()));
#else
            var property = serializedObject.FindProperty("m_Stream");
            propertys.Add(new KeyValuePair<string, object>("加载方式", ((AudioImporterLoadType)property.intValue).ToString()));
            property = serializedObject.FindProperty("m_Type");
            propertys.Add(new KeyValuePair<string, object>("格式", ((AudioType)property.intValue).ToString()));
#endif

            return propertys;
        }

        private static List<KeyValuePair<string, object>> AnalyzeParticleSystem(ParticleSystem particleSystem, SerializedObject serializedObject)
        {
            
            var propertys = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(AnalyzePropertys.ForecastParticleCount, particleSystem.GetForecastParticleCount() ),
                new KeyValuePair<string, object>(AnalyzePropertys.ForecastTrianglesCount, particleSystem.GetForecastTrianglesCount() ),
                new KeyValuePair<string, object>(AnalyzePropertys.Loop, particleSystem.main.loop),
                new KeyValuePair<string, object>(AnalyzePropertys.Duration, particleSystem.main.duration),
                new KeyValuePair<string, object>(AnalyzePropertys.StartLifetime, particleSystem.main.startLifetime.GetMaxVal()),
                new KeyValuePair<string, object>(AnalyzePropertys.RateOverTime, particleSystem.GetRateOverTimeMax()),
                new KeyValuePair<string, object>(AnalyzePropertys.MaxParticles, particleSystem.main.maxParticles),
                new KeyValuePair<string, object>(AnalyzePropertys.RenderMode, particleSystem.GetRenderMode() ),
                new KeyValuePair<string, object>(AnalyzePropertys.MeshName, particleSystem.GetMeshName() ),

        };
            return propertys;
        }

        private static List<KeyValuePair<string, object>> AnalyzeAnimationClip(AnimationClip clip, SerializedObject serializedObject)
        {
            var stats = AnimationClipStatsInfo.GetAnimationClipStats(clip);
            var propertys = new List<KeyValuePair<string, object>>
            {
                new KeyValuePair<string, object>(AnalyzePropertys.MemorySize, stats.size),
                new KeyValuePair<string, object>(AnalyzePropertys.TotalCurves, stats.totalCurves),
                new KeyValuePair<string, object>(AnalyzePropertys.ConstantCurves, stats.constantCurves),
                new KeyValuePair<string, object>(AnalyzePropertys.DenseCurves, stats.denseCurves),
                new KeyValuePair<string, object>(AnalyzePropertys.StreamCurves, stats.streamCurves),
#if UNITY_5 || UNITY_5_3_OR_NEWER
                new KeyValuePair<string, object>(AnalyzePropertys.EventsCount, clip.events.Length),
#else
                new KeyValuePair<string, object>("事件数", AnimationUtility.GetAnimationEvents(clip).Length),
#endif
            };
            return propertys;
        }

        private class AnimationClipStatsInfo
        {
            public int size;
            public int totalCurves;
            public int constantCurves;
            public int denseCurves;
            public int streamCurves;

            private static MethodInfo getAnimationClipStats;
            private static FieldInfo sizeInfo;
            private static FieldInfo totalCurvesInfo;
            private static FieldInfo constantCurvesInfo;
            private static FieldInfo denseCurvesInfo;
            private static FieldInfo streamCurvesInfo;

            public static AnimationClipStatsInfo GetAnimationClipStats(AnimationClip clip)
            {
                if (getAnimationClipStats == null)
                {
                    getAnimationClipStats = typeof(AnimationUtility).GetMethod("GetAnimationClipStats", BindingFlags.Static | BindingFlags.NonPublic);
                    var aniclipstats = typeof(AnimationUtility).Assembly.GetType("UnityEditor.AnimationClipStats");
                    sizeInfo = aniclipstats.GetField("size", BindingFlags.Public | BindingFlags.Instance);
                    totalCurvesInfo = aniclipstats.GetField("totalCurves", BindingFlags.Public | BindingFlags.Instance);
                    constantCurvesInfo = aniclipstats.GetField("constantCurves", BindingFlags.Public | BindingFlags.Instance);
                    denseCurvesInfo = aniclipstats.GetField("denseCurves", BindingFlags.Public | BindingFlags.Instance);
                    streamCurvesInfo = aniclipstats.GetField("streamCurves", BindingFlags.Public | BindingFlags.Instance);
                }

                var stats = getAnimationClipStats.Invoke(null, new object[] { clip });
                var stats2 = new AnimationClipStatsInfo
                {
                    size = (int)sizeInfo.GetValue(stats),
                    totalCurves = (int)totalCurvesInfo.GetValue(stats),
                    constantCurves = (int)constantCurvesInfo.GetValue(stats),
                    denseCurves = (int)denseCurvesInfo.GetValue(stats),
                    streamCurves = (int)streamCurvesInfo.GetValue(stats),
                };
                return stats2;
            }
        }

    }

}