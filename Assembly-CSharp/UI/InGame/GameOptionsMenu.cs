using UnityEngine;

public class GameOptionsMenu : MonoBehaviour
{
	private GameOptionsData cachedData;

	public GameObject ResetButton;

	private OptionBehaviour[] Children;

	public int selectedcustom;

	public CE_GameSettingsUI CustomMenu;

	public void Start()
	{
		CustomMenu = Object.Instantiate(new GameObject()).AddComponent<CE_GameSettingsUI>();
		Children = GetComponentsInChildren<OptionBehaviour>();
		cachedData = PlayerControl.GameOptions;
		for (int i = 0; i < Children.Length; i++)
		{
			OptionBehaviour optionBehaviour = Children[i];
			optionBehaviour.OnValueChanged = ValueChanged;

			bool clientDisabled = optionBehaviour.Title != "Recommended Settings" && !AmongUsClient.Instance.AmHost;

			if ((bool)AmongUsClient.Instance && clientDisabled)
			{
				optionBehaviour.SetAsPlayer();
			}
		}
	}

	public void Update()
	{
		if (cachedData != PlayerControl.GameOptions)
		{
			cachedData = PlayerControl.GameOptions;
			RefreshChildren();
		}
		_ = AmongUsClient.Instance.AmHost;
	}

	private void RefreshChildren()
	{
		for (int i = 0; i < Children.Length; i++)
		{
			OptionBehaviour obj = Children[i];
			obj.enabled = false;
			obj.enabled = true;
		}
	}

	public void ValueChanged(OptionBehaviour option)
	{
		if (option.Title == "Recommended Settings")
		{
			CE_GameSettingsUI.IsShown = true;
		}
		if (!AmongUsClient.Instance || !AmongUsClient.Instance.AmHost)
		{
			return;
		}
		else
		{
			GameOptionsData gameOptions = PlayerControl.GameOptions;
			switch (option.Title)
			{
			case "Kill Cooldown":
				gameOptions.KillCooldown = option.GetFloat();
				break;
			case "# Common Tasks":
				gameOptions.NumCommonTasks = option.GetInt();
				break;
			case "# Impostors":
				gameOptions.NumImpostors = option.GetInt();
				break;
			case "# Long Tasks":
				gameOptions.NumLongTasks = option.GetInt();
				break;
			case "Map":
				gameOptions.MapId = (byte)option.GetInt();
				break;
			case "Discussion Time":
				gameOptions.DiscussionTime = option.GetInt();
				break;
			case "Impostor Vision":
				gameOptions.ImpostorLightMod = option.GetFloat();
				break;
			case "Crewmate Vision":
				gameOptions.CrewLightMod = option.GetFloat();
				break;
			case "# Emergency Meetings":
				gameOptions.NumEmergencyMeetings = option.GetInt();
				break;
			case "Voting Time":
				gameOptions.VotingTime = option.GetInt();
				break;
			case "Kill Distance":
				gameOptions.KillDistance = option.GetInt();
				break;
			case "# Short Tasks":
				gameOptions.NumShortTasks = option.GetInt();
				break;
			case "Player Speed":
				gameOptions.PlayerSpeedMod = option.GetFloat();
				break;
			case "Gamemode":
				gameOptions.Gamemode = (byte)option.GetInt();
				break;
			default:
				Debug.Log("Ono, unrecognized setting: " + option.Title);
				break;
			}
			if (gameOptions.isDefaults && option.Title != "Map")
			{
				gameOptions.isDefaults = false;
				RefreshChildren();
			}
		}
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		if (!(localPlayer == null))
		{
			localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
		}
	}

	public void Sync()
	{
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		if (!(localPlayer == null))
		{
			localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
		}
	}

	private void SetCustomSettingsState(bool value)
	{
		OptionBehaviour[] children = Children;
		for (int i = 0; i < children.Length; i++)
		{
			children[i].gameObject.active = value;
		}
	}

	private void LateUpdate()
	{
		for (int i = 0; i < Children.Length; i++)
		{
			OptionBehaviour optionBehaviour = Children[i];
			if (optionBehaviour.Title == "Recommended Settings")
			{
				(optionBehaviour as ToggleOption).TitleText.Text = "All Settings...";
				(optionBehaviour as ToggleOption).CheckMark.sprite = null;
			}
		}
	}
}
