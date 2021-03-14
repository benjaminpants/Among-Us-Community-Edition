using UnityEngine;
using UnityEngine.SceneManagement;
public class CE_ModUI : MonoBehaviour
{
    public static CE_ModUI Instance;

    public static bool IsShown;

    private Vector2 scrollPosition;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    private void ModToggleMenu(int windowID)
    {
        CE_UIHelpers.LoadCommonAssets();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
        CE_CommonUI.CreateHeaderLabel("Enabling/Disabling any mods here requires a restart!");
        foreach (CE_Mod mod in CE_ModLoader.LMods)
        {
            CE_CommonUI.CreateSeperator();
            CE_CommonUI.CreateHeaderLabel(mod.ModName);
            CE_CommonUI.CreateHeaderLabel(mod.ModDesc + "\n ", FontStyle.Normal);
            mod.Enabled = CE_CommonUI.CreateBoolButtonNoCheck(mod.Enabled, "Enabled", "Disabled");
        }
        CE_CommonUI.CreateSeperator();
        if (CE_CommonUI.CreateBoolButtonNoCheck(false, "Enable All Mods", "Enable All Mods"))
        {
            foreach (CE_Mod mod in CE_ModLoader.LMods)
            {
                mod.Enabled = true;
            }
        }
        if (CE_CommonUI.CreateBoolButtonNoCheck(false, "Disable All Mods", "Disable All Mods"))
        {
            foreach (CE_Mod mod in CE_ModLoader.LMods)
            {
                mod.Enabled = false;
            }
        }
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
            GUILayout.Window(-6, CE_CommonUI.StockSettingsRect(), ModToggleMenu, "", CE_CommonUI.WindowStyle());
            if (CE_CommonUI.CreateCloseButton(CE_CommonUI.StockSettingsRect()))
            {
                IsShown = false;
                CE_ModLoader.UpdateDisabledMods();


            }
        }
    }
}
