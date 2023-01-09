using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


public class EditorWindowBuilder : EditorWindow
{
    protected bool isDirty = false;

    public void Header(string text, int x, int y, int width = 100, int height = 20)
    {
        GUI.Label(new Rect(x, y, width, height), text, EditorStyles.boldLabel);
    }

    public void HorizontalLine(int x, int y, int width, int thickness = 2)
    {
        GUI.Box(new Rect(x, y, width, thickness), "");
    }

    public void Label(string text, int x, int y, int width = 100, int height = 20, string tooltip = "")
    {
        if (!string.IsNullOrEmpty(tooltip))
        {
            GUI.Label(new Rect(x, y, width, height), new GUIContent(text, tooltip));
        }
        else
        {
            GUI.Label(new Rect(x, y, width, height), text, EditorStyles.label);
        }
    }

    public void VerticalLine(int x, int y, int Height, int thickness = 2)
    {
        GUI.Box(new Rect(x, y, thickness, Height), "");
    }

    public string TextInput(string text, int x, int y, int width = 100, int height = 20, bool isDirtiable = true)
    {
        text = GUI.TextField(new Rect(x, y, width, height), text);
        if (isDirtiable && GUI.changed)
        {
            isDirty = true;
        }
        
        GUI.changed = false;
        return text;
    }
}
