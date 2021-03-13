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
        //CE_CommonUI.CreateModButton(false, CE_ModLoader.LMods[0]); put UI stuff here please
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
            }
            
        }
    }
}
