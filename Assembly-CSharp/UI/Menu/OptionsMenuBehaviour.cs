using UnityEngine;

public class OptionsMenuBehaviour : MonoBehaviour
{
	public SpriteRenderer Background;

	public SpriteRenderer CloseButton;

	public SpriteRenderer JoystickButton;

	public SpriteRenderer TouchButton;

	public SlideBar JoystickSizeSlider;

	public FloatRange JoystickSizes = new FloatRange(0.5f, 1.5f);

	public SlideBar SoundSlider;

	public SlideBar MusicSlider;

	public ToggleButtonBehaviour SendNameButton;

	public ToggleButtonBehaviour SendTelemButton;

	public ToggleButtonBehaviour PersonalizedAdsButton;

	public bool Toggle = true;

	public TabGroup[] Tabs;

	private CE_GlobalSettingsUI CustomMenu;

	private CE_ControlsUI ControlsMenu;

	public bool IsOpen => base.isActiveAndEnabled;

	public void OpenTabGroup(TabGroup selected)
	{
		selected.Open();
		for (int i = 0; i < Tabs.Length; i++)
		{
			TabGroup tabGroup = Tabs[i];
			if (!(tabGroup == selected))
			{
				tabGroup.Close();
			}
		}
	}

	private void Update()
	{
		CE_ModifyMenu();
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Close();
		}
	}

	public void Open()
	{
		CustomMenu = Object.Instantiate(new GameObject()).AddComponent<CE_GlobalSettingsUI>();
		ControlsMenu = Object.Instantiate(new GameObject()).AddComponent<CE_ControlsUI>();
		JoystickButton.transform.parent.GetComponentInChildren<TextRenderer>().Text = "Mouse";
		TouchButton.transform.parent.GetComponentInChildren<TextRenderer>().Text = "Keyboard+Mouse";
		JoystickSizeSlider.gameObject.SetActive(value: false);
		if (base.gameObject.activeSelf)
		{
			if (Toggle)
			{
				GetComponent<TransitionOpen>().Close();
			}
		}
		else
		{
			OpenTabGroup(Tabs[0]);
			UpdateButtons();
			base.gameObject.SetActive(value: true);
		}
	}

	public void SetControlType(int i)
	{
		SaveManager.TouchConfig = i;
		UpdateButtons();
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.SetTouchType(i);
		}
	}

	public void UpdateJoystickSize(SlideBar slider)
	{
		SaveManager.JoystickSize = JoystickSizes.Lerp(slider.Value);
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.SetJoystickSize(SaveManager.JoystickSize);
		}
	}

	public void ToggleSendTelemetry()
	{
		CE_OpenControlsUI();
	}

	public void ToggleSendName()
	{
		CE_OpenGlobalSettings();
	}

	public void UpdateSfxVolume(SlideBar button)
	{
		SaveManager.SfxVolume = button.Value;
		SoundManager.Instance.ChangeSfxVolume(button.Value);
	}

	public void UpdateMusicVolume(SlideBar button)
	{
		SaveManager.MusicVolume = button.Value;
		SoundManager.Instance.ChangeMusicVolume(button.Value);
	}

	public void TogglePersonalizedAd()
	{
		switch (SaveManager.ShowAdsScreen & (ShowAdsState)127)
		{
		case ShowAdsState.NonPersonalized:
			SaveManager.ShowAdsScreen = ShowAdsState.Accepted;
			break;
		default:
			SaveManager.ShowAdsScreen = (ShowAdsState)129;
			break;
		case ShowAdsState.Purchased:
			break;
		}
		UpdateButtons();
	}

	public void UpdateButtons()
	{
		if (SaveManager.TouchConfig == 0)
		{
			JoystickButton.color = new Color32(0, byte.MaxValue, 42, byte.MaxValue);
			TouchButton.color = Color.white;
			JoystickSizeSlider.enabled = true;
			JoystickSizeSlider.OnEnable();
		}
		else
		{
			JoystickButton.color = Color.white;
			TouchButton.color = new Color32(0, byte.MaxValue, 42, byte.MaxValue);
			JoystickSizeSlider.enabled = false;
			JoystickSizeSlider.OnDisable();
		}
		JoystickSizeSlider.Value = JoystickSizes.ReverseLerp(SaveManager.JoystickSize);
		SoundSlider.Value = SaveManager.SfxVolume;
		MusicSlider.Value = SaveManager.MusicVolume;
		CE_CustomButtonSettings();
		if ((bool)PersonalizedAdsButton)
		{
			if (SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.Purchased) || SaveManager.BoughtNoAds)
			{
				PersonalizedAdsButton.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				PersonalizedAdsButton.UpdateText(!SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.NonPersonalized));
			}
		}
	}

	public void Close()
	{
		base.gameObject.SetActive(value: false);
		CE_UIHelpers.CollapseAll();
		CustomMenu.gameObject.SetActive(value: false);
	}

	private void CE_Original_ToggleSendName()
	{
		SaveManager.SendName = !SaveManager.SendName;
		UpdateButtons();
	}

	private void CE_OpenGlobalSettings()
	{
		CE_GlobalSettingsUI.IsShown = true;
	}

	private void CE_CustomButtonSettings()
	{
		SendTelemButton.Text.Text = "Controls...";
		SendNameButton.Text.Text = "More Options...";
	}

	private void OriginalButtonSettings()
	{
		SendNameButton.UpdateText(SaveManager.SendName);
		SendTelemButton.UpdateText(SaveManager.SendTelemetry);
	}

	public void CE_Original_ToggleSendTelemetry()
	{
		SaveManager.SendTelemetry = !SaveManager.SendTelemetry;
		UpdateButtons();
	}

	private void CE_OpenControlsUI()
	{
		CE_ControlsUI.IsShown = true;
	}

	private void CE_ModifyMenu()
	{
		GameObject[] array = Object.FindObjectsOfType<GameObject>();
		for (int i = 0; i < array.Length; i++)
		{
			TextRenderer component = array[i].GetComponent<TextRenderer>();
			if ((bool)component && component.name == "TelemText")
			{
				component.Text = "";
			}
		}
	}
}
