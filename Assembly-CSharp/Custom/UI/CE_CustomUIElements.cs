using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;
using GUI = UnityEngine.GUI;

public class CE_CustomUIElements
{
    public static void Label(string text, GUIStyle style)
    {
        DrawTextWithOutline(text, style, Color.black, Color.white, 2);
    }

    public static bool Button(string text, GUIStyle style)
    {
        bool state = GUILayout.Button("", style);
        GUIStyle textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = style.fontSize;
        textStyle.alignment = style.alignment;
        var rect = GUILayoutUtility.GetLastRect();
        DrawTextWithOutline(rect, text, textStyle, Color.black, Color.white, 2);
        return state;
    }

    public static Rect GetLabelRect(string text, GUIStyle style)
    {
        var content = new GUIContent(text);
        var r = GUILayoutUtility.GetRect(content, style);
        return r;
    }

    public static void DrawTextWithOutline(Rect r, string text, GUIStyle style, Color borderColor, Color innerColor, int borderWidth)
    {
        // assign the border color
        style.normal.textColor = borderColor;

        int i;
        for (i = -borderWidth; i <= borderWidth; i++)
        {
            GUI.Label(new Rect(r.x - borderWidth, r.y + i, r.width, r.height), text, style);
            GUI.Label(new Rect(r.x + borderWidth, r.y + i, r.width, r.height), text, style);
        }
        for (i = -borderWidth + 1; i <= borderWidth - 1; i++)
        {
            GUI.Label(new Rect(r.x + i, r.y - borderWidth, r.width, r.height), text, style);
            GUI.Label(new Rect(r.x + i, r.y + borderWidth, r.width, r.height), text, style);
        }

        // draw the inner color version in the center
        style.normal.textColor = innerColor;
        GUI.Label(r, text, style);
    }

    public static void DrawTextWithOutline(string text, GUIStyle style, Color borderColor, Color innerColor, int borderWidth)
    {
        var r = GetLabelRect(text, style);
        DrawTextWithOutline(r, text, style, borderColor, innerColor, borderWidth);

    }
}
