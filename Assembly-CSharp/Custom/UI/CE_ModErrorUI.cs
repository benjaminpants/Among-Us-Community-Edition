using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
public class CE_ModErrorUI : MonoBehaviour
{
    public static CE_ModErrorUI Instance;

    public static List<CE_Error> Errors = new List<CE_Error>();

    public static bool IsShown;

    private Vector2 scrollPosition;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public static void RequestOpen()
    {
        Debug.Log("Being requested to show mod errors, checking how many exist:" + Errors.Count);
        if (Errors.Count != 0)
        {
            IsShown = true;
        }
    }

    public static void AddError(CE_Error error)
    {
        Errors.Add(error);
        Errors.Sort(delegate (CE_Error c1, CE_Error c2)
        {
            return c1.ErrorType.CompareTo(c2.ErrorType);

        });
    }

    private void ErrorToggleMenu(int windowID)
    {
        CE_UIHelpers.LoadCommonAssets();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        CE_CommonUI.CreateHeaderLabel("The following problems require your attention.");
        foreach (CE_Error error in Errors)
        {
            CE_CommonUI.CreateSeperator();
            CE_CommonUI.CreateHeaderLabel(error.Text,error.ErrorType == ErrorTypes.Critical ? FontStyle.Bold : FontStyle.Normal);
            CE_CommonUI.CreateHeaderLabel(error.ImportantText,FontStyle.BoldAndItalic);
        }
        CE_CommonUI.CreateSeperator();
        GUILayout.FlexibleSpace();
        GUILayout.EndScrollView();
        GUI.color = Color.black;
        GUI.backgroundColor = Color.black;
        GUI.contentColor = Color.white;
    }
    private void OnGUI()
    {
        if (IsShown)
        {
            GUILayout.Window(-6, CE_CommonUI.StockSettingsRect(), ErrorToggleMenu, "", CE_CommonUI.WindowStyle());
            if (CE_CommonUI.CreateCloseButton(CE_CommonUI.StockSettingsRect()))
            {
                IsShown = false;
                Errors = new List<CE_Error>();


            }
        }
    }
}

public enum ErrorTypes
{
    Error,
    Warning,
    Critical
}
public class CE_Error
{
    public string Text = "Invalid";
    public string ImportantText = "The content will not be loaded!";
    public ErrorTypes ErrorType = ErrorTypes.Error;

    public CE_Error(string txt, string imptxt, ErrorTypes error)
    {
        Text = txt;
        ImportantText = imptxt;
        ErrorType = error;
    }
}