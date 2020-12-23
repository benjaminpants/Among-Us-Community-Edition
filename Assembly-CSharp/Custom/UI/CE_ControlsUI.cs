using UnityEngine;

public class CE_ControlsUI : MonoBehaviour
{
	private Vector2 scrollPosition;

	private static CE_ControlsUI instance;

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
			GUILayout.Window(-3, CE_CommonUI.StockSettingsRect(), GlobalSettingsMenu, "", CE_CommonUI.WindowStyle());
		}
	}

	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<CE_ControlsUI>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void GlobalSettingsMenu(int windowID)
	{
		_ = PlayerControl.GameOptions;
		CE_UIHelpers.LoadCommonAssets();
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		CE_CommonUI.CreateHeaderLabel("Keyboard Controls:");
		CE_CommonUI.CreateButtonLabel("Controls1", "ASDW / ArrowKeys", "Move Player");
		CE_CommonUI.CreateButtonLabel("Controls2", "R", "Report");
		CE_CommonUI.CreateButtonLabel("Controls3", "Space / E", "Use");
		CE_CommonUI.CreateButtonLabel("Controls4", "Tab", "Open Map");
		CE_CommonUI.CreateButtonLabel("Controls5", "Q", "Kill");
		CE_CommonUI.CreateButtonLabel("Controls5", "F", "Open Sabotage Map");
		GUILayout.FlexibleSpace();
		GUILayout.EndScrollView();
		if (CE_CommonUI.CreateExitButton())
		{
			IsShown = false;
		}
		GUI.color = Color.black;
		GUI.backgroundColor = Color.black;
		GUI.contentColor = Color.white;
	}
}
