using Games;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Ihaiu.Editors
{
    public class AnalyzeFileInfoWindow : EditorWindow
    {

        public static AnalyzeFileInfoWindow window;
        public static void Open(AnalyzeFileInfo analyzeInfo)
        {
            if (window == null)
                window = EditorWindow.GetWindow<AnalyzeFileInfoWindow>("资源分析");

            window.analyzeInfo = analyzeInfo;
            window.Show();
            window.Repaint();
        }

        public AnalyzeFileInfo analyzeInfo;
        private Vector2 scrollPos;

        private System.Type[] types = new System.Type[] { typeof(Texture2D), typeof(Mesh), typeof(Material), typeof(AudioClip), typeof(AnimationClip), typeof(ParticleSystem) };

        public virtual void OnGUI()
        {
            if (analyzeInfo == null) return;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("基本信息");
            GUILayout.BeginVertical(HStyle.boxMarginLeftStyle);
            EditorGUILayout.LabelField("类型", analyzeInfo.type.ToString());
            if(!string.IsNullOrEmpty(analyzeInfo.path))
                EditorGUILayout.LabelField("路径", analyzeInfo.path);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("对象", analyzeInfo.obj.ToString());
            if(GUILayout.Button("选择"))
            {
                Selection.activeObject = analyzeInfo.obj;
            }
            EditorGUILayout.EndHorizontal();

            if (analyzeInfo.ObjectCount > 0)
            {
                EditorGUILayout.LabelField("对象数量", analyzeInfo.ObjectCount.ToString());
            }

            if (analyzeInfo.TransformCount > 0)
            {
                EditorGUILayout.LabelField("Transform 数量", analyzeInfo.TransformCount.ToString());
            }

            if (analyzeInfo.ParticleSystemCount > 0)
            {
                EditorGUILayout.LabelField("粒子系统数量", analyzeInfo.ParticleSystemCount.ToString());
            }

            if (analyzeInfo.MaterialCount > 0)
            {
                EditorGUILayout.LabelField("材质球数量", analyzeInfo.MaterialCount.ToString());
            }


            if (analyzeInfo.Texture2DCount > 0)
            {
                EditorGUILayout.LabelField("贴图数量", analyzeInfo.Texture2DCount.ToString());
            }



            if (analyzeInfo.MeshRendererCount > 0)
            {
                EditorGUILayout.LabelField("MeshRenderer 数量", analyzeInfo.MeshRendererCount.ToString());
            }


            if (analyzeInfo.SkinnedMeshRendererCount > 0)
            {
                EditorGUILayout.LabelField("SkinnedMeshRenderer 数量", analyzeInfo.SkinnedMeshRendererCount.ToString());
            }


            if (analyzeInfo.MeshFilterCount > 0)
            {
                EditorGUILayout.LabelField("MeshFilter 数量", analyzeInfo.MeshFilterCount.ToString());
            }

            if (analyzeInfo.AnimatorCount > 0)
            {
                EditorGUILayout.LabelField("Animator 数量", analyzeInfo.AnimatorCount.ToString());
            }


            if (analyzeInfo.AnimationClipCount > 0)
            {
                EditorGUILayout.LabelField("动画动作 数量", analyzeInfo.AnimationClipCount.ToString());
            }

            if (analyzeInfo.BonesCount > 0)
            {
                EditorGUILayout.LabelField("骨骼 数量", analyzeInfo.BonesCount.ToString());
            }


            if (analyzeInfo.TotalTriangleCount > 0)
            {
                EditorGUILayout.LabelField("总面 数量", analyzeInfo.TotalTriangleCount.ToString());
            }

            if (analyzeInfo.TotalVertexCount > 0)
            {
                EditorGUILayout.LabelField("总顶点 数量", analyzeInfo.TotalVertexCount.ToString());
            }


            if (analyzeInfo.TotalForecastParticleCount > 0)
            {
                EditorGUILayout.LabelField("预估总粒子 数量", analyzeInfo.TotalForecastParticleCount.ToString());
            }

            if (analyzeInfo.TotalForecastTrianglesCount > 0)
            {
                EditorGUILayout.LabelField("预估总粒子面 数量", analyzeInfo.TotalForecastTrianglesCount.ToString());
            }

            GUILayout.EndVertical();



            GUILayout.Space(20);

            List<AnalyzeObjectInfo> list;
            foreach(System.Type type in types)
            {
                list = analyzeInfo.GetList(type);
                bool isShowPath = type != typeof(ParticleSystem);
                if (list.Count > 0)
                {
                    EditorGUILayout.LabelField(string.Format("{0} ({1})", type.Name, list.Count));
                    GUILayout.BeginVertical(HStyle.boxMarginLeftStyle);

                    GUILayout.BeginHorizontal(HStyle.boxColumnStyle);

                    EditorGUILayout.LabelField("Index", HStyle.labelMiddleCenterStyle, GUILayout.Width(100));
                    EditorGUILayout.LabelField("Object", HStyle.labelMiddleCenterStyle, GUILayout.Width(150));
                    if(isShowPath)
                        EditorGUILayout.LabelField("Path", HStyle.labelMiddleCenterStyle, GUILayout.Width(400));

                    foreach (KeyValuePair<string, object> prop in list[0].propertys)
                    {
                        EditorGUILayout.LabelField(AnalyzePropertys.GetName(prop.Key), HStyle.labelMiddleRightStyle, GUILayout.Width(AnalyzePropertys.GetWidth(prop.Key)));

                        if (GUILayout.Button("↑", GUILayout.Width(20)))
                        {
                            list.SortToUp(prop.Key);
                        }

                        if (GUILayout.Button("↓", GUILayout.Width(20)))
                        {
                            list.SortToDown(prop.Key);
                        }
                    }
                    GUILayout.EndHorizontal();


                    for (int i = 0; i < list.Count; i ++)
                    {
                        GUILayout.BeginHorizontal(HStyle.boxColumnStyle);
                        AnalyzeObjectInfo item = list[i];
                        EditorGUILayout.LabelField((i + 1).ToString(),  GUILayout.Width(100));

                        if(GUILayout.Button(item.obj.name, HStyle.buttonLabelLeftStyle, GUILayout.Width(150)))
                        {
                            Selection.activeObject = item.obj;
                        }

                        if (isShowPath && GUILayout.Button(item.path, HStyle.buttonLabelLeftStyle, GUILayout.Width(400)))
                        {
                            Selection.activeObject = item.obj;
                        }

                        foreach (KeyValuePair<string, object> prop in item.propertys)
                        {
                            if(!prop.GetValStr().StartsWith("Assets/"))
                            {
                                EditorGUILayout.LabelField(prop.GetValStr(), HStyle.labelMiddleRightStyleGray10, GUILayout.Width(AnalyzePropertys.GetWidth(prop.Key) + 50));
                            }
                            else
                            {
                                if (GUILayout.Button(prop.GetValStr(), HStyle.buttonLabelLeftStyle, GUILayout.Width(AnalyzePropertys.GetWidth(prop.Key) + 50)))
                                {
                                    Selection.activeObject = AssetDatabase.LoadAssetAtPath(prop.GetValStr(), typeof(UnityEngine.Object));
                                }
                            }
                          
                        }
                        GUILayout.EndHorizontal();
                    }

                    GUILayout.EndVertical();
                    GUILayout.Space(20);
                }
            }
            EditorGUILayout.EndScrollView();
        }

    }
}