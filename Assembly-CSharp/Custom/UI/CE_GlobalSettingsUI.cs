using UnityEngine;
using UnityEngine.SceneManagement;

public class CE_GlobalSettingsUI : MonoBehaviour
{
	private Vector2 scrollPosition;

	private static CE_GlobalSettingsUI instance;

	public static bool IsShown;

	private bool NewFeaturesReadyYet;

	private void OnEnable()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Update()
	{
	}

	private void OnGUI()
	{
		if (IsShown)
		{
			GUILayout.Window(-4, CE_CommonUI.StockSettingsRect(), GlobalSettingsMenu, "", CE_CommonUI.WindowStyle());
			if (CE_CommonUI.CreateCloseButton(CE_CommonUI.StockSettingsRect()))
			{
				IsShown = false;
			}
		}
	}

	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<CE_GlobalSettingsUI>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void GlobalSettingsMenu(int windowID)
	{
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		CE_UIHelpers.LoadCommonAssets();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		if (SceneManager.GetActiveScene().name == "MainMenu")
		{
			CE_CommonUI.CreateHeaderLabel("Host Options");
			gameOptions.NumImpostors = (int)CE_CommonUI.CreateValuePicker(gameOptions.NumImpostors, 1f, 1f, 3f, "Impostors", "");
			gameOptions.MaxPlayers = (int)CE_CommonUI.CreateValuePicker(gameOptions.MaxPlayers, 1f, 4f, 20f, "Max Players", "");
		}
		CE_CommonUI.CreateHeaderLabel("Extra Options");
		if (NewFeaturesReadyYet)
		{
			SaveManager.ColorBlindMode = CE_CommonUI.CreateBoolButton(SaveManager.ColorBlindMode, "Color Blind Mode");
		}
		SaveManager.LobbyShake = CE_CommonUI.CreateBoolButton(SaveManager.LobbyShake, "Lobby Shaking");
		SaveManager.EnableProHUDMode = CE_CommonUI.CreateBoolButton(SaveManager.EnableProHUDMode, "Pro HUD Mode");
		SaveManager.EnableAnimationTestingMode = CE_CommonUI.CreateBoolButton(SaveManager.EnableAnimationTestingMode, "Sprite Debug Mode");
		SaveManager.HideIntro = CE_CommonUI.CreateBoolButton(SaveManager.HideIntro, "Disable Extra Intros");
		SaveManager.UseLegacyVoteIcons = CE_CommonUI.CreateBoolButton(SaveManager.UseLegacyVoteIcons, "Use Legacy Vote Icons");
		GUILayout.FlexibleSpace();
		GUILayout.EndScrollView();
		GUI.color = Color.black;
		GUI.backgroundColor = Color.black;
		GUI.contentColor = Color.white;
	}
}
