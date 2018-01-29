using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ihaiu.Editors
{
    public class AnalyzeFileInfo
    {
        public Type                     type;
        public UnityEngine.Object       obj;
        public string                   path;

        public Dictionary<Type, List<AnalyzeObjectInfo>> dict = new Dictionary<Type, List<AnalyzeObjectInfo>>();


        public List<AnalyzeObjectInfo> GetList<T>()
        {
            return GetList(typeof(T));
        }

        public List<AnalyzeObjectInfo> GetList(Type type)
        {
            List<AnalyzeObjectInfo> list;
            if (!dict.ContainsKey(type))
            {
                list = new List<AnalyzeObjectInfo>();
                dict.Add(type, list);
            }
            else
            {
                list = dict[type];
            }
            return list;
        }

        public void Add(AnalyzeObjectInfo item)
        {
            List<AnalyzeObjectInfo> list = GetList(item.type);
            list.Add(item);
        }

        public int GameObjectCount
        {
            get
            {
                return GetList<GameObject>().Count;
            }
        }


        public int TransformCount
        {
            get
            {
                return GetList<Transform>().Count;
            }
        }


        public int ParticleSystemCount
        {
            get
            {
                return GetList<ParticleSystem>().Count;
            }
        }


        public int SkinnedMeshRendererCount
        {
            get
            {
                return GetList<SkinnedMeshRenderer>().Count;
            }
        }

        public int MeshRendererCount
        {
            get
            {
                return GetList<MeshRenderer>().Count;
            }
        }


        public int MeshFilterCount
        {
            get
            {
                return GetList<MeshFilter>().Count;
            }
        }



        public int MaterialCount
        {
            get
            {
                return GetList<Material>().Count;
            }
        }



        public int Texture2DCount
        {
            get
            {
                return GetList<Texture2D>().Count;
            }
        }

        
        public int AnimatorCount
        {
            get
            {
                return GetList<Animator>().Count;
            }
        }
        public int AnimationClipCount
        {
            get
            {
                return GetList<AnimationClip>().Count;
            }
        }








        public int ObjectCount
        {
            get
            {
                int num = 0;
                foreach(var kvp in dict)
                {
                    num += kvp.Value.Count;
                }
                return num;
            }
        }

        public int BonesCount = 0;
        public int TotalTriangleCount = 0;
        public int TotalVertexCount = 0;
        public int TotalForecastParticleCount = 0;
        public int TotalForecastTrianglesCount = 0;

    }


    public class AnalyzeObjectInfo
    {
        public Type                     type;
        public UnityEngine.Object       obj;
        public string                   path;
        public string                   guid;

        /// <summary>
        /// 属性
        /// </summary>
        public List<KeyValuePair<string, object>> propertys;



        private Dictionary<string, KeyValuePair<string, object>> _propertydict;
        public Dictionary<string, KeyValuePair<string, object>> Propertydict
        {
            get
            {
                if(_propertydict == null)
                {
                    _propertydict = new Dictionary<string, KeyValuePair<string, object>>();
                    foreach(var item in propertys)
                    {
                        _propertydict.Add(item.Key, item);
                    }
                }
                return _propertydict;
            }
        }

        public object GetValue(string key)
        {
            if (Propertydict.ContainsKey(key))
                return Propertydict[key].Value;
            return null;
        }

        public object this[string key]
        {
            get
            {
                return GetValue(key);
            }
        }


    }
}