using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class OpenAssetLogHandler_Lua
{

    [OnOpenAssetAttribute(0)]
    static bool OnOpenAsset(int instanceID, int line)
    {
        string stackTrace = GetStackTrace();
        if (!string.IsNullOrEmpty(stackTrace) && (stackTrace.Contains("LUA:") || stackTrace.Contains("LuaException:")))
        {
            string pattern = @"[\s\t]+(.+\.lua):(\d+): in";
            System.Text.RegularExpressions.Match matches = System.Text.RegularExpressions.Regex.Match(stackTrace, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            string pathLine = "";
            while (matches.Success)
            {
                pathLine = matches.Groups[1].Value;
                line = System.Convert.ToInt32(matches.Groups[2].Value);

                if (!matches.Groups[0].Value.Contains("DebugUtil"))
                {
                    return OpenAssetSettings.Instance.Open(pathLine, line);
                    break;
                }
                matches = matches.NextMatch();
            }
            return true;
        }
        return false;
    }

    static string GetStackTrace()
    {
        var ConsoleWindowType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ConsoleWindow");
        var fieldInfo = ConsoleWindowType.GetField("ms_ConsoleWindow", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        var consoleInstance = fieldInfo.GetValue(null);
        if (consoleInstance != null)
        {
            if ((object)UnityEditor.EditorWindow.focusedWindow == consoleInstance)
            {
                var ListViewStateType = typeof(UnityEditor.EditorWindow).Assembly.GetType("UnityEditor.ListViewState");
                fieldInfo = ConsoleWindowType.GetField("m_ListView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                var listView = fieldInfo.GetValue(consoleInstance);
                fieldInfo = ListViewStateType.GetField("row", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                int row = (int)fieldInfo.GetValue(listView);
                fieldInfo = ConsoleWindowType.GetField("m_ActiveText", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                string activeText = fieldInfo.GetValue(consoleInstance).ToString();
                return activeText;
            }
        }
        return null;
    }
}
