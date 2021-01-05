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

			bool clientDisabled = optionBehaviour.Title != "Recommended Settings";

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
	}

	public void Sync()
	{
		PlayerControl localPlayer = PlayerControl.LocalPlayer;
		if (!(localPlayer == null))
		{
			localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
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
