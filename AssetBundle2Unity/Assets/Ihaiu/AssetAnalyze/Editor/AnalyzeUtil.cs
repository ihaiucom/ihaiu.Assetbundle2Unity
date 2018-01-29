using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnalyzeProjectObjectInfo = Ihaiu.Editors.AnalyzeProjects.AnalyzeObjectInfo;

namespace Ihaiu.Editors
{
    public static class AnalyzeUtil
    {
        public static string GetValStr(this KeyValuePair<string, object> prop)
        {
            if (prop.Key == AnalyzePropertys.MemorySize)
            {
                return ((int)prop.Value).Byte2Str();
            }

            return prop.Value.ToString();
        }


        public static void SortToUp(this List<AnalyzeObjectInfo> list, string key)
        {
            list.Sort((AnalyzeObjectInfo a, AnalyzeObjectInfo b) =>
            {
                return CompareTo(a, b, key);
            });
        }


        public static void SortToDown(this List<AnalyzeObjectInfo> list, string key)
        {
            list.Sort((AnalyzeObjectInfo a, AnalyzeObjectInfo b) =>
            {
                return CompareTo(b, a, key);
            });
        }

        private static int CompareTo(AnalyzeObjectInfo a, AnalyzeObjectInfo b, string key)
        {
            System.Type type = a[key].GetType();

            if (type == typeof(int))
            {
                return ((int)a[key]).CompareTo((int)b[key]);
            }


            if (type == typeof(float))
            {
                return ((float)a[key]).CompareTo((float)b[key]);
            }


            if (type == typeof(bool))
            {
                return ((bool)a[key]).CompareTo((bool)b[key]);
            }

            string str = a[key].ToString();
            return str.CompareTo((string)b[key]);
        }



        public static void SortToUp(this List<AnalyzeProjectObjectInfo> list, string key)
        {
            list.Sort((AnalyzeProjectObjectInfo a, AnalyzeProjectObjectInfo b) =>
            {
                return CompareTo(a, b, key);
            });
        }


        public static void SortToDown(this List<AnalyzeProjectObjectInfo> list, string key)
        {
            list.Sort((AnalyzeProjectObjectInfo a, AnalyzeProjectObjectInfo b) =>
            {
                return CompareTo(b, a, key);
            });
        }

        private static int CompareTo(AnalyzeProjectObjectInfo a, AnalyzeProjectObjectInfo b, string key)
        {
            if (a[key] == null || b[key] == null)
                return 0;

            System.Type type = a[key].GetType();

            if (type == typeof(int))
            {
                return ((int)a[key]).CompareTo((int)b[key]);
            }


            if (type == typeof(float))
            {
                return ((float)a[key]).CompareTo((float)b[key]);
            }


            if (type == typeof(bool))
            {
                return ((bool)a[key]).CompareTo((bool)b[key]);
            }

            string str = a[key].ToString();
            return str.CompareTo((string)b[key]);
        }
    }
}