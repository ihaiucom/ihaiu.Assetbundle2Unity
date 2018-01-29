using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Ihaiu.Editors.AnalyzeProjects
{
    public class AnalyzeProjectEditorSaveXLSX
    {
        public static void Save(string path, AnalyzeProjectInfo project, AnalyzeProjectInfo unitProject, AnalyzeProjectInfo fxProject, AnalyzeProjectInfo uiProject)
        {

            var xlsx = new FileInfo(path);
            using (var package = new ExcelPackage(xlsx))
            {
                //Texture2D
                CreateAndFillWorksheet<Texture2D>(package.Workbook.Worksheets, project, AnalyzePropertys.Width);
                //Mesh
                CreateAndFillWorksheet<Mesh>(package.Workbook.Worksheets, project, AnalyzePropertys.TriangleCount);

                //UI
                List<AnalyzeObjectInfo> list = uiProject.GetList<GameObject>();
                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "UI", AnalyzePropertys.TotalForecastTrianglesCount);

                Dictionary<string, bool> textureGuidDict = new Dictionary<string, bool>();
                Dictionary<string, bool> meshGuidDict = new Dictionary<string, bool>();


                foreach (AnalyzeObjectInfo item in list)
                {
                    List<Ihaiu.Editors.AnalyzeObjectInfo> mlist = item.fileInfo.GetList<Texture2D>();
                    foreach (Ihaiu.Editors.AnalyzeObjectInfo v in mlist)
                    {
                        if (!textureGuidDict.ContainsKey(v.guid))
                            textureGuidDict.Add(v.guid, true);
                    }

                    mlist = item.fileInfo.GetList<Mesh>();
                    foreach (Ihaiu.Editors.AnalyzeObjectInfo v in mlist)
                    {
                        if (!meshGuidDict.ContainsKey(v.guid))
                            meshGuidDict.Add(v.guid, true);
                    }
                }

                list = new List<AnalyzeObjectInfo>();
                foreach (var kvp in textureGuidDict)
                {
                    if (project.guidDict.ContainsKey(kvp.Key))
                        list.Add(project.guidDict[kvp.Key]);
                }

                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "UI图片", AnalyzePropertys.Width);


                list = new List<AnalyzeObjectInfo>();
                foreach (var kvp in meshGuidDict)
                {
                    if (project.guidDict.ContainsKey(kvp.Key))
                        list.Add(project.guidDict[kvp.Key]);
                }
                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "UI Mesh", AnalyzePropertys.TriangleCount);


                // 动态图片

                list = new List<AnalyzeObjectInfo>();
                List<string> files = new List<string>();
                string[] arr;
                foreach (string root in AnalyzeSettings.Instance.imageRoots)
                {
                    if (!Directory.Exists(root)) continue;

                    arr = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
                        .Where(s => Path.GetExtension(s).ToLower() != ".meta" && Path.GetFileName(s).ToLower() != ".ds_store").ToArray();
                    files.AddRange(arr);
                }

                for (int i = 0; i < files.Count; i++)
                {
                    string guid = AssetDatabase.AssetPathToGUID(files[i]);
                    if (project.guidDict.ContainsKey(guid))
                        list.Add(project.guidDict[guid]);
                }

                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "动态图片", AnalyzePropertys.TotalForecastTrianglesCount);



                //FX
                list = fxProject.GetList<GameObject>();
                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "特效", AnalyzePropertys.TotalForecastTrianglesCount);


                textureGuidDict = new Dictionary<string, bool>();
                meshGuidDict = new Dictionary<string, bool>();

                foreach (AnalyzeObjectInfo item in list)
                {
                    List<Ihaiu.Editors.AnalyzeObjectInfo> mlist = item.fileInfo.GetList<Texture2D>();
                    foreach(Ihaiu.Editors.AnalyzeObjectInfo v in mlist)
                    {
                        if(!textureGuidDict.ContainsKey(v.guid))
                            textureGuidDict.Add(v.guid, true);
                    }

                    mlist = item.fileInfo.GetList<Mesh>();
                    foreach (Ihaiu.Editors.AnalyzeObjectInfo v in mlist)
                    {
                        if (!meshGuidDict.ContainsKey(v.guid))
                            meshGuidDict.Add(v.guid, true);
                    }
                }

                list = new List<AnalyzeObjectInfo>();
                foreach(var kvp in textureGuidDict)
                {
                    if(project.guidDict.ContainsKey(kvp.Key))
                        list.Add(project.guidDict[kvp.Key]);
                }

                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "特效贴图", AnalyzePropertys.Width);


                list = new List<AnalyzeObjectInfo>();
                foreach (var kvp in meshGuidDict)
                {
                    if (project.guidDict.ContainsKey(kvp.Key))
                        list.Add(project.guidDict[kvp.Key]);
                }
                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "特效Mesh", AnalyzePropertys.TriangleCount);

                //Unit
                list = unitProject.GetList<GameObject>();
                CreateAndFillWorksheet(package.Workbook.Worksheets, list, "所有单位", AnalyzePropertys.TotalTriangleCount);

                textureGuidDict = new Dictionary<string, bool>();
                meshGuidDict = new Dictionary<string, bool>();

                //Unit -- Hero
                List<AnalyzeObjectInfo> heroList = new List<AnalyzeObjectInfo>();
                Dictionary<string, bool> heroTextureGuidDict = new Dictionary<string, bool>();
                Dictionary<string, bool> heroMeshGuidDict = new Dictionary<string, bool>();

                //Unit -- Solider
                List<AnalyzeObjectInfo> soliderList = new List<AnalyzeObjectInfo>();
                Dictionary<string, bool> soliderTextureGuidDict = new Dictionary<string, bool>();
                Dictionary<string, bool> soliderMeshGuidDict = new Dictionary<string, bool>();
                //Unit -- Tower
                List<AnalyzeObjectInfo> towerList = new List<AnalyzeObjectInfo>();
                Dictionary<string, bool> towerTextureGuidDict = new Dictionary<string, bool>();
                Dictionary<string, bool> towerMeshGuidDict = new Dictionary<string, bool>();
                //Unit -- Other
                List<AnalyzeObjectInfo> otherList = new List<AnalyzeObjectInfo>();
                Dictionary<string, bool> otherTextureGuidDict = new Dictionary<string, bool>();
                Dictionary<string, bool> otherMeshGuidDict = new Dictionary<string, bool>();


                Dictionary<string, bool> tTextureGuidDict;
                Dictionary<string, bool> tMeshGuidDict ;

                foreach (AnalyzeObjectInfo item in list)
                {
                    string name = Path.GetFileName(item.path).ToLower();
                    if(name.StartsWith("hero_"))
                    {
                        heroList.Add(item);
                        tTextureGuidDict = heroTextureGuidDict;
                        tMeshGuidDict = heroMeshGuidDict;
                    }
                    else if(name.StartsWith("solider_"))
                    {
                        soliderList.Add(item);
                        tTextureGuidDict = soliderTextureGuidDict;
                        tMeshGuidDict = soliderMeshGuidDict;
                    }
                    else if (name.StartsWith("tower_"))
                    {
                        towerList.Add(item);
                        tTextureGuidDict = towerTextureGuidDict;
                        tMeshGuidDict = towerMeshGuidDict;
                    }
                    else
                    {
                        otherList.Add(item);
                        tTextureGuidDict = otherTextureGuidDict;
                        tMeshGuidDict = otherMeshGuidDict;
                    }



                    List<Ihaiu.Editors.AnalyzeObjectInfo> mlist = item.fileInfo.GetList<Texture2D>();
                    foreach (Ihaiu.Editors.AnalyzeObjectInfo v in mlist)
                    {
                        if (!textureGuidDict.ContainsKey(v.guid))
                            textureGuidDict.Add(v.guid, true);

                        if (!tTextureGuidDict.ContainsKey(v.guid))
                            tTextureGuidDict.Add(v.guid, true);
                    }

                    mlist = item.fileInfo.GetList<Mesh>();
                    foreach (Ihaiu.Editors.AnalyzeObjectInfo v in mlist)
                    {
                        if (!meshGuidDict.ContainsKey(v.guid))
                            meshGuidDict.Add(v.guid, true);


                        if (!tMeshGuidDict.ContainsKey(v.guid))
                            tMeshGuidDict.Add(v.guid, true);
                    }
                }

                CreateAndFillWorksheet(package.Workbook.Worksheets, heroList, "英雄", AnalyzePropertys.TotalTriangleCount);
                CreateAndFillWorksheet(package.Workbook.Worksheets, soliderList, "士兵", AnalyzePropertys.TotalTriangleCount);
                CreateAndFillWorksheet(package.Workbook.Worksheets, towerList, "机关", AnalyzePropertys.TotalTriangleCount);
                CreateAndFillWorksheet(package.Workbook.Worksheets, otherList, "其他单位", AnalyzePropertys.TotalTriangleCount);


                string[] names = new string[] { "所有单位", "英雄", "士兵", "机关", "其他" };
                Dictionary<string, bool>[] textures = new Dictionary<string, bool>[] {textureGuidDict, heroTextureGuidDict, soliderTextureGuidDict, towerTextureGuidDict, otherTextureGuidDict };
                Dictionary<string, bool>[] meshs = new Dictionary<string, bool>[] { meshGuidDict, heroMeshGuidDict, soliderMeshGuidDict, towerMeshGuidDict, otherMeshGuidDict };


                for(int i = 0; i < names.Length; i ++)
                {

                    list = new List<AnalyzeObjectInfo>();
                    foreach (var kvp in textures[i])
                    {
                        if (project.guidDict.ContainsKey(kvp.Key))
                            list.Add(project.guidDict[kvp.Key]);
                    }

                    CreateAndFillWorksheet(package.Workbook.Worksheets, list, names[i] + "~贴图", AnalyzePropertys.Width);


                    list = new List<AnalyzeObjectInfo>();
                    foreach (var kvp in meshs[i])
                    {
                        if (project.guidDict.ContainsKey(kvp.Key))
                            list.Add(project.guidDict[kvp.Key]);
                    }
                    CreateAndFillWorksheet(package.Workbook.Worksheets, list, names[i] + "~Mesh", AnalyzePropertys.TriangleCount);
                }


                package.Save();
            }
        }


        public static void CreateAndFillWorksheet<T>(ExcelWorksheets wss, AnalyzeProjectInfo project, string sortKey)
        {
            Type type = typeof(T);
            string[] arr = type.Name.Split('.');
            string typeName = arr[arr.Length - 1];

            List<AnalyzeObjectInfo> list = project.GetList<T>();
            if (list.Count == 0) return;
            CreateAndFillWorksheet(wss, list, typeName, sortKey);
        }


        public static void CreateAndFillWorksheet(ExcelWorksheets wss, List<AnalyzeObjectInfo> list, string typeName, string sortKey)
        {
            if (list.Count == 0) return;


            list.SortToDown(sortKey);

            List<string> columnNames = new List<string>();
            //columnNames.Add("guid");
            columnNames.Add("path");
            //columnNames.Add("type");
            foreach (var property in list[0].propertys)
            {
                columnNames.Add(property.Key);
            }


            string titleName = typeName;
            ExcelWorksheet ws = wss.Add(titleName);

            int colCount = columnNames.Count;

            // 标签颜色
            ws.TabColor = ColorTranslator.FromHtml("#b490f5");
            CreateWorksheetBase(ws, titleName, colCount);

            // 列头
            for (int i = 0; i < columnNames.Count; i++)
            {
                ws.Cells[2, i + 1].Value = AnalyzePropertys.GetName(columnNames[i]);
            }


            using (var range = ws.Cells[2, 1, 2, colCount])
            {
                // 字体样式
                range.Style.Font.Bold = true;

                // 背景颜色
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#DDEBF7"));

                // 开启自动筛选
                range.AutoFilter = true;
            }


            // 冻结前两行
            ws.View.FreezePanes(3, 1);

            int startRow = 3;

            foreach (AnalyzeObjectInfo item in list)
            {
                //ws.Cells[startRow, 1].Value = item.guid;
                ws.Cells[startRow, 1].Value = item.path;
                //ws.Cells[startRow, 3].Value = item.type;
                for (int i = 0; i < item.propertys.Count; i++)
                {
                    ws.Cells[startRow, 1 + 1 + i].Value = item.propertys[i].Value;
                }
                startRow++;
            }

            ws.Cells[1, 1].Value = ws.Cells[1, 1].Value + " (" + (startRow - 3) + ")";

            // 列宽
            for (int i = 0; i < columnNames.Count; i++)
            {
                ws.Column(i + 1).Width = AnalyzePropertys.GetWidth(columnNames[i]) / 4;
                Type columnType = AnalyzePropertys.GetType(columnNames[i]);
                if (columnType == AnalyzePropertys.typeInt || columnType == AnalyzePropertys.typeFloat)
                {
                    ws.Column(i + 1).Style.Numberformat.Format = "#,##0";
                }
            }
            ws.Column(1).Width = 100;
        }


        public static void CreateWorksheetBase(ExcelWorksheet ws, string title, int colCount)
        {
            // 全体颜色
            ws.Cells.Style.Font.Color.SetColor(ColorTranslator.FromHtml("#3d4d65"));
            {
                // 边框样式
                var border = ws.Cells.Style.Border;
                border.Bottom.Style = border.Top.Style = border.Left.Style = border.Right.Style
                    = ExcelBorderStyle.Thin;

                // 边框颜色
                var clr = ColorTranslator.FromHtml("#B2C6C9");
                border.Bottom.Color.SetColor(clr);
                border.Top.Color.SetColor(clr);
                border.Left.Color.SetColor(clr);
                border.Right.Color.SetColor(clr);
            }

            // 标题
            ws.Cells[1, 1].Value = title;
            using (var range = ws.Cells[1, 1, 1, colCount])
            {
                range.Merge = true;
                range.Style.Font.Bold = true;
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            }
            ws.Row(1).Height = 30;
        }


    }
}