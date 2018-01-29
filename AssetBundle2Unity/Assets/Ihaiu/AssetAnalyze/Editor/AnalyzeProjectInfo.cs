using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ihaiu.Editors.AnalyzeProjects
{
    [SerializeField]
    public class AnalyzeProjectInfo
    {
        [SerializeField]
        public List<AnalyzeObjectInfo> list;


        [NonSerialized]
        public Dictionary<string, AnalyzeObjectInfo>        guidDict        = new Dictionary<string, AnalyzeObjectInfo>();

        [NonSerialized]
        public Dictionary<Type, List<AnalyzeObjectInfo>>    typeListDict    = new Dictionary<Type, List<AnalyzeObjectInfo>>();

        // 列表转字典
        public void List2Dict()
        {
            if (list != null)
            {
                foreach (AnalyzeObjectInfo item in list)
                {
                    item.Property2List();
                    Add(item);
                }
            }
        }


        // 字典转列表
        public void Dict2List()
        {
            list = new List<AnalyzeObjectInfo>(guidDict.Values);
            foreach(AnalyzeObjectInfo item in list)
            {
                item.Property2KVList();
            }
        }

        public AnalyzeObjectInfo Get(string guid)
        {
            if (guidDict.ContainsKey(guid))
                return guidDict[guid];
            return null;
        }


        public List<AnalyzeObjectInfo> GetList<T>()
        {
            return GetList(typeof(T));
        }

        public List<AnalyzeObjectInfo> GetList(Type type)
        {
            List<AnalyzeObjectInfo> list;
            if (!typeListDict.ContainsKey(type))
            {
                list = new List<AnalyzeObjectInfo>();
                typeListDict.Add(type, list);
            }
            else
            {
                list = typeListDict[type];
            }
            return list;
        }

        public void Add(AnalyzeObjectInfo item)
        {
            List<AnalyzeObjectInfo> list = GetList(item.type);
            list.Add(item);
            guidDict.Add(item.guid, item);
        }



    }


    [System.Serializable]
    public class AnalyzeObjectInfo
    {
        [SerializeField]
        public string               guid;
        [SerializeField]
        public Type                 type;
        [SerializeField]
        public string               path;


        [SerializeField]
        public List<string> propertykeys = new List<string>();
        [SerializeField]
        public List<string> propertyValues = new List<string>();



        [NonSerialized]
        public AnalyzeFileInfo fileInfo;


        /** 被引用列表 */
        [SerializeField]
        public List<string> refGUIDList = new List<string>();

        /** 依赖列表 */
        [SerializeField]
        public List<string> depGUIDList = new List<string>();


        /// <summary>
        /// 属性
        /// </summary>
        [NonSerialized]
        public List<KeyValuePair<string, object>> propertys;



        [NonSerialized]
        private Dictionary<string, KeyValuePair<string, object>> _propertydict;

        public Dictionary<string, KeyValuePair<string, object>> Propertydict
        {
            get
            {
                if (_propertydict == null)
                {
                    _propertydict = new Dictionary<string, KeyValuePair<string, object>>();
                    foreach (var item in propertys)
                    {
                        if (!_propertydict.ContainsKey(item.Key))
                            _propertydict.Add(item.Key, item);
                        else
                            Debug.LogError("already Key " + item.Key + ":" + item.Value);
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

        public void Property2KVList()
        {
            propertykeys.Clear();
            propertyValues.Clear();
            if (propertys != null)
            {
                foreach(KeyValuePair<string, object> item in propertys)
                {
                    propertykeys.Add(item.Key);
                    propertyValues.Add(item.Value.ToString());
                }
            }
        }


        public void Property2List()
        {
            propertys = new List<KeyValuePair<string, object>>();
            int count = propertykeys.Count;
            for(int i = 0; i < count; i ++)
            {
                propertys.Add(new KeyValuePair<string, object>(propertykeys[i], propertyValues[i]));
            }
        }


    }


}