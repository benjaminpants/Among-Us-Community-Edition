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
			GUILayout.Window(-5, CE_CommonUI.GameSettingsRect(), CustomSettingsMenu, "", CE_CommonUI.WindowStyle(true));
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
		bool readOnly = !AmongUsClient.Instance.AmHost;
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		CE_UIHelpers.LoadCommonAssets();
		bool isDefaults = gameOptions.isDefaults;
		gameOptions.isDefaults = CE_CommonUI.CreateBoolButtonG(gameOptions.isDefaults, "Recommended Settings (Classic Only)", readOnly);
		if (!isDefaults && gameOptions.isDefaults && !readOnly)
		{
			gameOptions.SetRecommendations(GameData.Instance.PlayerCount, AmongUsClient.Instance.GameMode);
		}
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, CE_CommonUI.GameScrollbarStyleH(), CE_CommonUI.GameScrollbarStyleV(), CE_CommonUI.GameScrollViewStyle(), new GUILayoutOption[0]);
		gameOptions.MapId = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.MapId, GameOptionsData.MapNames, 0, 1, "Map", readOnly);
		gameOptions.Gamemode = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.Gamemode, GameOptionsData.Gamemodes, 0, 25, "Gamemode", readOnly);
		if (CE_CommonUI.CreateCollapsable("General Modifiers", 0, true))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.NumImpostors = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumImpostors, 1f, 1f, 4f, "# Impostors", "", false, readOnly);
				gameOptions.PlayerSpeedMod = CE_CommonUI.CreateValuePickerG(gameOptions.PlayerSpeedMod, 0.25f, 0.25f, 10f, "Player Speed", "x", decmialView: true, readOnly);
				gameOptions.KillCooldown = CE_CommonUI.CreateValuePickerG(gameOptions.KillCooldown, 0.25f, 0f, 120f, "Kill Cooldown", "s", decmialView: true, readOnly);
				gameOptions.KillDistance = CE_CommonUI.CreateStringPickerG(gameOptions.KillDistance, GameOptionsData.KillDistanceStrings, 0, 5, "Kill Distance", readOnly);
				gameOptions.CrewLightMod = CE_CommonUI.CreateValuePickerG(gameOptions.CrewLightMod, 0.25f, 0.25f, 5f, "Crewmate Vision", "x", decmialView: true, readOnly);
				gameOptions.ImpostorLightMod = CE_CommonUI.CreateValuePickerG(gameOptions.ImpostorLightMod, 0.25f, 0.25f, 5f, "Impostor Vision", "x", decmialView: true, readOnly);
				gameOptions.SabControl = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.SabControl, GameOptionsData.SabControlStrings, 0, 4, "Sabotages", readOnly);
			}
		}
		if (CE_CommonUI.CreateCollapsable("Meetings", 1, true))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.DiscussionTime = (int)CE_CommonUI.CreateValuePickerG(gameOptions.DiscussionTime, 5f, 0f, 300f, "Discussion Time", "s", false, readOnly);
				gameOptions.VotingTime = (int)CE_CommonUI.CreateValuePickerG(gameOptions.VotingTime, 5f, 0f, 300f, "Voting Time", "s", false, readOnly);
				gameOptions.NumEmergencyMeetings = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumEmergencyMeetings, 1f, 0f, float.MaxValue, "# Emergency Meetings", "", false, readOnly);
				gameOptions.AnonVotes = CE_CommonUI.CreateBoolButtonG(gameOptions.AnonVotes, "Anonymous Votes", readOnly);
				gameOptions.ConfirmEject = CE_CommonUI.CreateBoolButtonG(gameOptions.ConfirmEject, "Confirm Ejects", readOnly);
			}
		}
		if (CE_CommonUI.CreateCollapsable("Tasks", 2, true))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.NumCommonTasks = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumCommonTasks, 1f, 0f, 2f, "# Common Tasks", "", false, readOnly);
				gameOptions.NumLongTasks = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumLongTasks, 1f, 0f, 3f, "# Long Tasks", "", false, readOnly);
				gameOptions.NumShortTasks = (int)CE_CommonUI.CreateValuePickerG(gameOptions.NumShortTasks, 1f, 0f, 5f, "# Short Tasks", "", false, readOnly);
				gameOptions.Visuals = CE_CommonUI.CreateBoolButtonG(gameOptions.Visuals, "Visual Tasks", readOnly);
			}
		}
		if (CE_CommonUI.CreateCollapsable("Vent Controls", 3, true))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.Venting = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.Venting, GameOptionsData.VentModeStrings, 0, 3, "Vents", readOnly);
				gameOptions.VentMode = (byte)CE_CommonUI.CreateStringPickerG(gameOptions.VentMode, GameOptionsData.VentMode2Strings, 0, 3, "Vent Movement", readOnly);
			}

		}
		if (CE_CommonUI.CreateCollapsable("Map Scale And Rotation", 4, true))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
            {
				gameOptions.MapScaleX = CE_CommonUI.CreateValuePickerG(gameOptions.MapScaleX, 0.25f, -5f, 5f, "Map Scale X", "", false, readOnly);
				gameOptions.MapScaleY = CE_CommonUI.CreateValuePickerG(gameOptions.MapScaleY, 0.25f, -5f, 5f, "Map Scale Y", "", false, readOnly);
				gameOptions.MapRot = (int)CE_CommonUI.CreateValuePickerG(gameOptions.MapRot, 15f, -360f, 360f, "Map Rotation", "°", false, readOnly);
			}

		}
		GUILayout.EndScrollView();
		if (CE_CommonUI.CreateExitButton(true))
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
