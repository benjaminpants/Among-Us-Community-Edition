using UnityEngine;

public class CE_GameSettingsUI : MonoBehaviour
{
	private Vector2 scrollPosition;

	private static CE_GameSettingsUI instance;

	private Color GameSettingsColor;

	public static bool IsShown;

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
			CE_CommonUI.GUIDrawRect(CE_CommonUI.GameSettingsRect(), GameSettingsColor);
			GUILayout.Window(-5, CE_CommonUI.GameSettingsRect(), CustomSettingsMenu, "");
		}
	}

	public static void Init()
	{
		if (!(instance != null))
		{
			GameObject gameObject = new GameObject();
			gameObject.AddComponent<CE_GameSettingsUI>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	public CE_GameSettingsUI()
	{
		GameSettingsColor = new Color(0.631f, 0.749f, 0.639f);
	}

	private void CustomSettingsMenu(int windowID)
	{
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		CE_UIHelpers.LoadCommonAssets();
		bool isDefaults = gameOptions.isDefaults;
		gameOptions.isDefaults = CE_CommonUI.CreateBoolButtonG(gameOptions.isDefaults, "Recommended Settings (Classic)");
		if (!isDefaults && gameOptions.isDefaults)
		{
			gameOptions.SetRecommendations(GameData.Instance.PlayerCount, AmongUsClient.Instance.GameMode);
		}
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true);
		gameOptions.MapId = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.MapId, GameOptionsData.MapNames, 0, 1, "Map");
		gameOptions.NumImpostors = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumImpostors, 1f, 1f, 4f, "# Impostors", "");
		gameOptions.NumEmergencyMeetings = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumEmergencyMeetings, 1f, 0f, float.MaxValue, "# Emergency Meetings", "");
		gameOptions.DiscussionTime = (int)CE_CommonUI.CreateValuePickerG(gameOptions.DiscussionTime, 5f, 0f, 300f, "Discussion Time", "s");
		gameOptions.VotingTime = (int)CE_CommonUI.CreateValuePickerG(gameOptions.VotingTime, 5f, 0f, 300f, "Voting Time", "s");
		gameOptions.PlayerSpeedMod = CE_CommonUI.CreateValuePickerG(gameOptions.PlayerSpeedMod, 0.25f, 0.25f, 10f, "Player Speed", "x", decmialView: true);
		gameOptions.CrewLightMod = CE_CommonUI.CreateValuePickerG(gameOptions.CrewLightMod, 0.25f, 0.25f, 5f, "Crewmate Vision", "x", decmialView: true);
		gameOptions.ImpostorLightMod = CE_CommonUI.CreateValuePickerG(gameOptions.ImpostorLightMod, 0.25f, 0.25f, 5f, "Impostor Vision", "x", decmialView: true);
		gameOptions.KillCooldown = CE_CommonUI.CreateValuePickerG(gameOptions.KillCooldown, 0.25f, 0f, 120f, "Kill Cooldown", "s", decmialView: true);
		gameOptions.KillDistance = CE_CommonUI.CreateStringPickerG(gameOptions.KillDistance, GameOptionsData.KillDistanceStrings, 0, 5, "Kill Distance");
		gameOptions.NumCommonTasks = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumCommonTasks, 1f, 0f, 2f, "# Common Tasks", "");
		gameOptions.NumLongTasks = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumLongTasks, 1f, 0f, 3f, "# Long Tasks", "");
		gameOptions.NumShortTasks = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumShortTasks, 1f, 0f, 5f, "# Short Tasks", "");
		if (CE_CommonUI.CreateCollapsable("Vent Controls", 0))
		{
			CE_CommonUI.CreateSeperator();
			gameOptions.Venting = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.Venting, GameOptionsData.VentModeStrings, 0, 3, "Vents");
			gameOptions.VentMode = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.VentMode, GameOptionsData.VentMode2Strings, 0, 3, "Vent Movement");
			CE_CommonUI.CreateSeperator();
		}
		gameOptions.AnonVotes = CE_CommonUI.CreateBoolButtonG(gameOptions.AnonVotes, "Anonymous Votes");
		gameOptions.ConfirmEject = CE_CommonUI.CreateBoolButtonG(gameOptions.ConfirmEject, "Confirm Ejects");
		gameOptions.Visuals = CE_CommonUI.CreateBoolButtonG(gameOptions.Visuals, "Visual Tasks");
		gameOptions.Gamemode = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.Gamemode, GameOptionsData.Gamemodes, 0, 25, "Gamemode");
        gameOptions.SabControl = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.SabControl, GameOptionsData.SabControlStrings, 0, 4, "Sabotages");
		if (CE_CommonUI.CreateCollapsable("Map Scale And Rotation", 1))
		{
			CE_CommonUI.CreateSeperator();
			gameOptions.MapScaleX = CE_CommonUI.CreateValuePickerG(gameOptions.MapScaleX, 0.25f, -5f, 5f, "Map Scale X", "");
            gameOptions.MapScaleY = CE_CommonUI.CreateValuePickerG(gameOptions.MapScaleY, 0.25f, -5f, 5f, "Map Scale Y", "");
			gameOptions.MapRot = (int)CE_CommonUI.CreateValuePickerG(gameOptions.MapRot, 15f, -360f, 360f, "Map Rotation", "°");
			CE_CommonUI.CreateSeperator();
		}
		GUILayout.EndScrollView();
		if (CE_CommonUI.CreateExitButton())
		{
			IsShown = false;
		}
		GUI.color = GameSettingsColor;
		GUI.backgroundColor = GameSettingsColor;
		GUI.contentColor = Color.white;
		CE_CommonUI.SyncSettings();
	}

	static CE_GameSettingsUI()
	{
	}
}
