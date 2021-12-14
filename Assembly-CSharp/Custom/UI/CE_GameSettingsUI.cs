﻿using UnityEngine;

public class CE_GameSettingsUI : MonoBehaviour
{
	private Vector2 scrollPosition;

	private static CE_GameSettingsUI instance;

	private Color GameSettingsColor;

	public static bool IsShown;

	private static GameOptionsData gameOptions;

	private static bool ReadOnly
	{
		get
        {
			return !AmongUsClient.Instance.AmHost || AmongUsClient.Instance.GameMode == GameModes.FreePlay;
		}
	}

	private static bool ReadOnly_Freeplay
    {
		get
        {
			return !AmongUsClient.Instance.AmHost;
		}
    }

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
			CE_CommonUI.WindowHoverBounds = GUILayout.Window(-5, CE_CommonUI.GameSettingsRect(), CustomSettingsMenu, "", CE_CommonUI.WindowStyle_GS());
			if (CE_CommonUI.CreateCloseButton(CE_CommonUI.GameSettingsRect()))
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
			gameObject.AddComponent<CE_GameSettingsUI>();
			Object.DontDestroyOnLoad(gameObject);
		}
	}

	public CE_GameSettingsUI()
	{
		GameSettingsColor = new Color(0.631f, 0.749f, 0.639f);
	}

	private void CE_ListedItems()
    {
		CE_GeneralModifiersDropdown();
		CE_MeetingsDropdown();
		CE_TasksDropdown();
		CE_VentControlsDropdown();
		CE_MapControlsDropdown();
		CE_MiscControlsDropdown();
	}

	private void CE_CoreItems()
    {
		bool isDefaults = gameOptions.isDefaults;
		gameOptions.isDefaults = CE_CommonUI.CreateBoolButton_GS(gameOptions.isDefaults, "Recommended Settings\n(for Classic)", ReadOnly);
		if (!isDefaults && gameOptions.isDefaults && !ReadOnly)
		{
			gameOptions.SetRecommendations(GameData.Instance.PlayerCount, AmongUsClient.Instance.GameMode);
		}
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, CE_CommonUI.GameScrollbarStyleH(), CE_CommonUI.GameScrollbarStyleV(), CE_CommonUI.GameScrollViewStyle(), new GUILayoutOption[0]);
		gameOptions.MapId = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.MapId, GameOptionsData.MapNames, 0, 2, "Map", ReadOnly); // testing this thing
		gameOptions.Gamemode = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.Gamemode, GameOptionsData.Gamemodes, 0, 25, "Gamemode", ReadOnly);
		CE_ListedItems();
		GUILayout.EndScrollView();
	}

	private void CE_GeneralModifiersDropdown()
    {
		if (CE_CommonUI.CreateCollapsable_GS("General Modifiers", 0))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.NumImpostors = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.NumImpostors, 1f, 1f, 4f, "# Impostors", "", false, ReadOnly);
				gameOptions.PlayerSpeedMod = CE_CommonUI.CreateValuePicker_GS(gameOptions.PlayerSpeedMod, 0.25f, 0.25f, 10f, "Player Speed", "x", decmialView: true, ReadOnly_Freeplay);
				gameOptions.KillCooldown = CE_CommonUI.CreateValuePicker_GS(gameOptions.KillCooldown, 1.25f, 0f, 120f, "Kill Cooldown", "s", decmialView: true, ReadOnly);
				gameOptions.KillDistance = CE_CommonUI.CreateStringPicker_GS(gameOptions.KillDistance, GameOptionsData.KillDistanceStrings, 0, 5, "Kill Distance", ReadOnly);
				gameOptions.CrewLightMod = CE_CommonUI.CreateValuePicker_GS(gameOptions.CrewLightMod, 0.25f, 0f, 5f, "Crewmate Vision", "x", decmialView: true, ReadOnly);
				gameOptions.ImpostorLightMod = CE_CommonUI.CreateValuePicker_GS(gameOptions.ImpostorLightMod, 0.25f, 0f, 5f, "Impostor Vision", "x", decmialView: true, ReadOnly);
				gameOptions.SabControl = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.SabControl, GameOptionsData.SabControlStrings, 0, 4, "Sabotages", ReadOnly);
			}
		}
	}

	private void CE_MeetingsDropdown()
	{
		if (CE_CommonUI.CreateCollapsable_GS("Meetings", 1))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.DiscussionTime = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.DiscussionTime, 5f, 0f, 300f, "Discussion Time", "s", false, ReadOnly);
				gameOptions.VotingTime = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.VotingTime, 5f, 0f, 300f, "Voting Time", "s", false, ReadOnly);
				gameOptions.NumEmergencyMeetings = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.NumEmergencyMeetings, 1f, 0f, float.MaxValue, "# Emergency Meetings", "", false, ReadOnly);
				gameOptions.AnonVotes = CE_CommonUI.CreateBoolButton_GS(gameOptions.AnonVotes, "Anonymous Votes", ReadOnly);
				gameOptions.ConfirmEject = CE_CommonUI.CreateBoolButton_GS(gameOptions.ConfirmEject, "Confirm Ejects", ReadOnly);
			}
		}
	}

	private void CE_TasksDropdown()
	{
		if (CE_CommonUI.CreateCollapsable_GS("Tasks", 2))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.NumCommonTasks = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.NumCommonTasks, 1f, 0f, 2f, "# Common Tasks", "", false, ReadOnly);
				gameOptions.NumLongTasks = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.NumLongTasks, 1f, 0f, 3f, "# Long Tasks", "", false, ReadOnly);
				gameOptions.NumShortTasks = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.NumShortTasks, 1f, 0f, 5f, "# Short Tasks", "", false, ReadOnly);
				gameOptions.Visuals = CE_CommonUI.CreateBoolButton_GS(gameOptions.Visuals, "Visual Tasks", ReadOnly);
				gameOptions.TaskBarUpdates = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.TaskBarUpdates, GameOptionsData.TaskBarUpStrings, 0, 2, "Taskbar Updates", ReadOnly);
				gameOptions.GhostsDoTasks = CE_CommonUI.CreateBoolButton_GS(gameOptions.GhostsDoTasks, "Ghosts Do Tasks", ReadOnly);
			}
		}
	}

	private void CE_VentControlsDropdown()
	{
		if (CE_CommonUI.CreateCollapsable_GS("Vent Controls", 3))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.Venting = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.Venting, GameOptionsData.VentModeStrings, 0, 3, "Vents", ReadOnly);
				gameOptions.VentMode = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.VentMode, GameOptionsData.VentMode2Strings, 0, 5, "Vent Movement", ReadOnly);
			}

		}
	}

	private void CE_MapControlsDropdown()
	{
		if (CE_CommonUI.CreateCollapsable_GS("Map Scale And Rotation", 4))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.MapScaleX = CE_CommonUI.CreateValuePicker_GS(gameOptions.MapScaleX, 0.25f, -5f, 5f, "Map Scale X", "", false, ReadOnly);
				gameOptions.MapScaleY = CE_CommonUI.CreateValuePicker_GS(gameOptions.MapScaleY, 0.25f, -5f, 5f, "Map Scale Y", "", false, ReadOnly);
				gameOptions.MapRot = (int)CE_CommonUI.CreateValuePicker_GS(gameOptions.MapRot, 15f, -360f, 360f, "Map Rotation", "°", false, ReadOnly);
			}

		}
	}

	private void CE_MiscControlsDropdown()
	{
		if (CE_CommonUI.CreateCollapsable_GS("Misc Modifiers", 5))
		{
			using (new GUILayout.VerticalScope(CE_CommonUI.GameDropDownBGStyle()))
			{
				gameOptions.CanSeeGhosts = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.CanSeeGhosts, GameOptionsData.CanSeeGhostsStrings, 0, 3, "Ghost Visibility", ReadOnly);
				gameOptions.BodyEffect = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.BodyEffect, GameOptionsData.BodySett, 0, 2, "Body Effect", ReadOnly);
                if (gameOptions.BodyEffect == 1)
                {
                    gameOptions.BodyDecayTime = (byte)CE_CommonUI.CreateStringPicker_GS(gameOptions.BodyDecayTime, GameOptionsData.BodyDecayTimes, 0, 2, "Body Decay Time", ReadOnly);
                }
                gameOptions.ImpOnlyChat = CE_CommonUI.CreateBoolButton_GS(gameOptions.ImpOnlyChat, "Allow Impostor Only Chat", ReadOnly);
				gameOptions.ShowOtherVision = CE_CommonUI.CreateBoolButton_GS(gameOptions.ShowOtherVision, "Show All Vision", ReadOnly);
			}
		}
	}

	private void SetStyle()
    {
		GUI.color = GameSettingsColor;
		GUI.backgroundColor = GameSettingsColor;
		GUI.contentColor = Color.white;
	}

	private void CustomSettingsMenu(int windowID)
	{
		gameOptions = PlayerControl.GameOptions;
		CE_UIHelpers.LoadCommonAssets();
		CE_CoreItems();
		SetStyle();
		CE_CommonUI.SyncSettings_GS();
	}
}
