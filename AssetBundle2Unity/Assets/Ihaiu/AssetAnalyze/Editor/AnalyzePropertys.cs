using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ihaiu.Editors
{
    public class AnalyzePropertys
    {
        public const string Width = "Width";
        public const string Height = "Height";
        public const string Format = "Format";
        public const string TextureFormat = "TextureFormat";
        public const string MiniMap = "MiniMap";
        public const string ReadWrite = "ReadWrite";
        public const string MemorySize = "MemorySize";

        public const string TextureType = "TextureType";
        public const string TextureCompression = "TextureCompression";
        public const string SpriteImportMode = "SpriteImportMode";
        public const string IsPow2Size = "IsPow2Size";
        public const string MaxTextureSize = "MaxTextureSize";
        

        public const string VertexCount = "VertexCount";
        public const string TriangleCount = "TriangleCount";
        public const string SubMeshCount = "SubMeshCount";
        public const string MeshCompression = "MeshCompression";

        public const string OptimizeMesh = "OptimizeMesh";
        public const string OptimizeGameObjects = "OptimizeGameObjects";
        public const string GenerateCollider = "GenerateCollider";
        public const string ImportBlendShapes = "ImportBlendShapes";
        public const string ImportMaterials = "ImportMaterials";
        public const string TransformPathsCount = "TransformPathsCount";
        public const string ExtraExposedTransformPathsCount = "ExtraExposedTransformPathsCount";
        

        public const string Shader = "Shader";
        public const string Texture = "Texture";


        public const string LoadType = "LoadType";
        public const string PreloadAudioData = "PreloadAudioData";
        public const string Frequency = "Frequency";
        public const string Length = "Length";


        public const string Loop = "Loop";
        public const string Duration = "Duration";
        public const string StartLifetime = "StartLifetime";
        public const string RateOverTime = "rateOverTime";
        public const string MaxParticles = "MaxParticles";
        public const string ForecastParticleCount = "ForecastParticleCount";
        public const string ForecastTrianglesCount = "ForecastTrianglesCount";
        public const string RenderMode = "RenderMode";
        public const string MeshName = "MeshName";


        public const string TotalCurves = "TotalCurves";
        public const string ConstantCurves = "ConstantCurves";
        public const string DenseCurves = "DenseCurves";
        public const string StreamCurves = "streamCurves";
        public const string EventsCount = "EventsCount";



        public const string ScaleFactor = "ScaleFactor";
        public const string FileScale = "FileScale";
        public const string UseFileUnits = "UseFileUnits";


        public const string MeshFilterCount = "MeshFilterCount";
        public const string SkinnedMeshRendererCount = "SkinnedMeshRendererCount";
        public const string MeshRendererCount = "MeshRendererCount";
        public const string MaterialCount = "MaterialCount";
        public const string ParticleSystemCount = "ParticleSystemCount";
        public const string Texture2DCount = "Texture2DCount";
        public const string AnimatorCount = "AnimatorCount";
        public const string AnimationClipCount = "AnimationClipCount";
        public const string ObjectCount = "ObjectCount";
        public const string TransformCount = "TransformCount";

        public const string BonesCount = "BonesCount";
        public const string SkinQuality = " SkinQuality";
        public const string UpdateWhenOffscreen = " UpdateWhenOffscreen";
        public const string SkinnedMotionVectors = " SkinnedMotionVectors";
        public const string ReflectionProbeUsage = " ReflectionProbeUsage";
        public const string ShadowCastingMode = " ShadowCastingMode";
        public const string LightProbeUsage = " LightProbeUsage";
        public const string ReceiveShadows = " ReceiveShadows";
        public const string TotalTriangleCount = " TotalTriangleCount";
        public const string TotalVertexCount = " TotalVertexCount";
        public const string TotalForecastParticleCount = " TotalForecastParticleCount";
        public const string TotalForecastTrianglesCount = " TotalForecastTrianglesCount";

        public static string GetName(string key)
        {
            if(names.ContainsKey(key))
            {
                return names[key];
            }
            return key;
        }

        private static Dictionary<string, string> _names;
        public static Dictionary<string, string> names
        {
            get
            {
                if (_names == null)
                {
                    _names = new Dictionary<string, string>();
                    _names.Add(Width, "宽度");
                    _names.Add(Height, "高度");
                    _names.Add(Format, "格式");
                    _names.Add(TextureFormat, "贴图格式");
                    _names.Add(MiniMap, "MipMap功能");
                    _names.Add(ReadWrite, "读写");
                    _names.Add(MemorySize, "内存占用");


                    _names.Add(VertexCount, "顶点数");
                    _names.Add(TriangleCount, "面数");
                    _names.Add(SubMeshCount, "子网格数");
                    _names.Add(MeshCompression, "网格压缩");


                    _names.Add(Shader, "依赖Shader");
                    _names.Add(Texture, "依赖Texture");

                    _names.Add(LoadType, "加载方式");
                    _names.Add(PreloadAudioData, "预加载");
                    _names.Add(Frequency, "频率");
                    _names.Add(Length, "长度");


                    _names.Add(Loop, "是否循环");
                    _names.Add(Duration, "持续时间");
                    _names.Add(StartLifetime, "生命时间");
                    _names.Add(RateOverTime, "rateOverTime");
                    _names.Add(MaxParticles, "最大粒子数量");
                    _names.Add(ForecastParticleCount, "预测粒子数量");
                    _names.Add(ForecastTrianglesCount, "预测三角面数量");
                    _names.Add(RenderMode, "渲染模式");
                    _names.Add(MeshName, "MeshName");


                    _names.Add(TotalCurves, "总曲线数");
                    _names.Add(ConstantCurves, "Constant曲线数");
                    _names.Add(DenseCurves, "Dense曲线数");
                    _names.Add(StreamCurves, "Stream曲线数");
                    _names.Add(EventsCount, "事件数");


                    _names.Add(MeshFilterCount, "MeshFilter数量");
                    _names.Add(MeshRendererCount, "MeshRenderer");
                    _names.Add(SkinnedMeshRendererCount, "SkinnedMeshRenderer数量");
                    _names.Add(MaterialCount, "Material数量");
                    _names.Add(ParticleSystemCount, "ParticleSystem数量");
                    _names.Add(Texture2DCount, "Texture2D数量");
                    _names.Add(AnimatorCount, "Animator数量");
                    _names.Add(AnimationClipCount, "AnimationClip数量");
                    _names.Add(ObjectCount, "Object数量");
                    _names.Add(TransformCount, "Transform数量");

                    _names.Add(BonesCount, "骨骼数量");
                    _names.Add(TotalTriangleCount, "总面数");
                    _names.Add(TotalVertexCount, "总顶点数");
                    _names.Add(TotalForecastParticleCount, "预计总粒子数");
                    _names.Add(TotalForecastTrianglesCount, "预计总粒子面数");

    }
                return _names;
            }
        }

        public static int GetWidth(string key)
        {
            if (widths.ContainsKey(key))
            {
                return widths[key];
            }
            return 100;
        }

        private static Dictionary<string, int> _widths;
        public static Dictionary<string, int> widths
        {
            get
            {
                if (_widths == null)
                {
                    _widths = new Dictionary<string, int>();
                    _widths.Add(Width, 100);
                    _widths.Add(Height, 100);
                    _widths.Add(Format, 200);
                    _widths.Add(TextureFormat, 200);
                    _widths.Add(MiniMap, 100);
                    _widths.Add(ReadWrite, 100);
                    _widths.Add(MemorySize, 100);


                    _widths.Add(VertexCount, 100);
                    _widths.Add(TriangleCount, 100);
                    _widths.Add(SubMeshCount, 100);
                    _widths.Add(MeshCompression, 100);


                    _widths.Add(Shader, 400);
                    _widths.Add(Texture, 400);

                    _widths.Add(LoadType, 100);
                    _widths.Add(PreloadAudioData, 100);
                    _widths.Add(Frequency, 100);
                    _widths.Add(Length, 100);


                    _widths.Add(Loop, 100);
                    _widths.Add(Duration, 100);
                    _widths.Add(StartLifetime, 200);
                    _widths.Add(RateOverTime, 200);
                    _widths.Add(MaxParticles, 200);
                    _widths.Add(ForecastParticleCount, 200);
                    _widths.Add(ForecastTrianglesCount, 200);
                    _widths.Add(RenderMode, 200);
                    _widths.Add(MeshName, 400);


                    _widths.Add(TotalCurves, 200);
                    _widths.Add(ConstantCurves, 200);
                    _widths.Add(DenseCurves, 200);
                    _widths.Add(StreamCurves, 200);
                    _widths.Add(EventsCount, 100);


                    _widths.Add(BonesCount, 100);

                }
                return _widths;
            }
        }




        public static System.Type typeInt = typeof(int);
        public static System.Type typeFloat = typeof(float);
        public static System.Type typeString = typeof(string);
        public static System.Type typeBool = typeof(bool);
        public static System.Type GetType(string key)
        {
            if (types.ContainsKey(key))
            {
                return types[key];
            }
            return typeof(object);
        }

        private static Dictionary<string, System.Type> _types;
        public static Dictionary<string, System.Type> types
        {
            get
            {
                if (_types == null)
                {
                    _types = new Dictionary<string, System.Type>();
                    _types.Add(Width, typeInt);
                    _types.Add(Height, typeInt);
                    _types.Add(MaxTextureSize, typeInt); 
                    _types.Add(MiniMap, typeBool);
                    _types.Add(ReadWrite, typeBool);
                    _types.Add(MemorySize, typeInt);


                    _types.Add(VertexCount, typeInt);
                    _types.Add(TriangleCount, typeInt);
                    _types.Add(SubMeshCount, typeInt);



                    _types.Add(Length, typeFloat);


                    _types.Add(Duration, typeFloat);
                    _types.Add(StartLifetime, typeFloat);
                    _types.Add(RateOverTime, typeFloat);
                    _types.Add(MaxParticles, typeInt);
                    _types.Add(ForecastParticleCount, typeInt);
                    _types.Add(ForecastTrianglesCount, typeInt);


                    _types.Add(TotalCurves, typeInt);
                    _types.Add(ConstantCurves, typeInt);
                    _types.Add(DenseCurves, typeInt);
                    _types.Add(StreamCurves, typeInt);
                    _types.Add(EventsCount, typeInt);


                    _types.Add(BonesCount, typeInt);

                }
                return _types;
            }
        }
    }
}