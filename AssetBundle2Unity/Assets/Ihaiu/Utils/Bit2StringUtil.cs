using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Bit2StringUtil
{

    public static string Bit2Str(this int bit)
    {
        float kb = bit / 8f / 1024f;
        if (kb < 1024)
        {
            return kb.ToString("F2") + " KB";
        }
        else
        {
            float mb = kb / 1024f;
            return mb.ToString("F2") + " MB";
        }
    }

    public static string Byte2Str(this int bytesSize)
    {
        float kb = bytesSize / 1024f;
        if(kb < 1024)
        {
            return kb.ToString("F2") + " KB";
        }
        else
        {
            float mb = kb / 1024f;
            return mb.ToString("F2") + " MB";
        }
    }
}
