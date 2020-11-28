using System;
using UnityEngine;

// Token: 0x020001C0 RID: 448
public class OptionsMenuBehaviour : MonoBehaviour
{
	// Token: 0x1700017D RID: 381
	// (get) Token: 0x060009B7 RID: 2487 RVA: 0x00004ED0 File Offset: 0x000030D0
	public bool IsOpen
	{
		get
		{
			return base.isActiveAndEnabled;
		}
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x00033250 File Offset: 0x00031450
	public void OpenTabGroup(TabGroup selected)
	{
		selected.Open();
		for (int i = 0; i < this.Tabs.Length; i++)
		{
			TabGroup tabGroup = this.Tabs[i];
			if (!(tabGroup == selected))
			{
				tabGroup.Close();
			}
		}
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x00007DC1 File Offset: 0x00005FC1
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			this.Close();
		}
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x00033290 File Offset: 0x00031490
	public void Open()
	{
		this.JoystickButton.transform.parent.GetComponentInChildren<TextRenderer>().Text = "Mouse";
		this.TouchButton.transform.parent.GetComponentInChildren<TextRenderer>().Text = "Keyboard+Mouse";
		this.JoystickSizeSlider.gameObject.SetActive(false);
		if (base.gameObject.activeSelf)
		{
			if (this.Toggle)
			{
				base.GetComponent<TransitionOpen>().Close();
			}
			return;
		}
		this.OpenTabGroup(this.Tabs[0]);
		this.UpdateButtons();
		base.gameObject.SetActive(true);
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x00007DD2 File Offset: 0x00005FD2
	public void SetControlType(int i)
	{
		SaveManager.TouchConfig = i;
		this.UpdateButtons();
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.SetTouchType(i);
		}
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x00007DF2 File Offset: 0x00005FF2
	public void UpdateJoystickSize(SlideBar slider)
	{
		SaveManager.JoystickSize = this.JoystickSizes.Lerp(slider.Value);
		if (DestroyableSingleton<HudManager>.InstanceExists)
		{
			DestroyableSingleton<HudManager>.Instance.SetJoystickSize(SaveManager.JoystickSize);
		}
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x00007E20 File Offset: 0x00006020
	public void ToggleSendTelemetry()
	{
		SaveManager.SendTelemetry = !SaveManager.SendTelemetry;
		this.UpdateButtons();
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x00007E35 File Offset: 0x00006035
	public void ToggleSendName()
	{
		SaveManager.SendName = !SaveManager.SendName;
		this.UpdateButtons();
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x00007E4A File Offset: 0x0000604A
	public void UpdateSfxVolume(SlideBar button)
	{
		SaveManager.SfxVolume = button.Value;
		SoundManager.Instance.ChangeSfxVolume(button.Value);
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x00007E67 File Offset: 0x00006067
	public void UpdateMusicVolume(SlideBar button)
	{
		SaveManager.MusicVolume = button.Value;
		SoundManager.Instance.ChangeMusicVolume(button.Value);
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x00033330 File Offset: 0x00031530
	public void TogglePersonalizedAd()
	{
		ShowAdsState showAdsState = SaveManager.ShowAdsScreen & (ShowAdsState)127;
		if (showAdsState != ShowAdsState.Personalized)
		{
			if (showAdsState == ShowAdsState.NonPersonalized)
			{
				SaveManager.ShowAdsScreen = ShowAdsState.Accepted;
				goto IL_30;
			}
			if (showAdsState == ShowAdsState.Purchased)
			{
				goto IL_30;
			}
		}
		SaveManager.ShowAdsScreen = (ShowAdsState)129;
		IL_30:
		this.UpdateButtons();
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x00033374 File Offset: 0x00031574
	public void UpdateButtons()
	{
		if (SaveManager.TouchConfig == 0)
		{
			this.JoystickButton.color = new Color32(0, byte.MaxValue, 42, byte.MaxValue);
			this.TouchButton.color = Color.white;
			this.JoystickSizeSlider.enabled = true;
			this.JoystickSizeSlider.OnEnable();
		}
		else
		{
			this.JoystickButton.color = Color.white;
			this.TouchButton.color = new Color32(0, byte.MaxValue, 42, byte.MaxValue);
			this.JoystickSizeSlider.enabled = false;
			this.JoystickSizeSlider.OnDisable();
		}
		this.JoystickSizeSlider.Value = this.JoystickSizes.ReverseLerp(SaveManager.JoystickSize);
		this.SoundSlider.Value = SaveManager.SfxVolume;
		this.MusicSlider.Value = SaveManager.MusicVolume;
		this.SendNameButton.UpdateText(SaveManager.SendName);
		this.SendTelemButton.UpdateText(SaveManager.SendTelemetry);
		if (this.PersonalizedAdsButton)
		{
			if (SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.Purchased) || SaveManager.BoughtNoAds)
			{
				this.PersonalizedAdsButton.transform.parent.gameObject.SetActive(false);
				return;
			}
			this.PersonalizedAdsButton.UpdateText(!SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.NonPersonalized));
		}
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x00005EC7 File Offset: 0x000040C7
	public void Close()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x04000955 RID: 2389
	public SpriteRenderer Background;

	// Token: 0x04000956 RID: 2390
	public SpriteRenderer CloseButton;

	// Token: 0x04000957 RID: 2391
	public SpriteRenderer JoystickButton;

	// Token: 0x04000958 RID: 2392
	public SpriteRenderer TouchButton;

	// Token: 0x04000959 RID: 2393
	public SlideBar JoystickSizeSlider;

	// Token: 0x0400095A RID: 2394
	public FloatRange JoystickSizes = new FloatRange(0.5f, 1.5f);

	// Token: 0x0400095B RID: 2395
	public SlideBar SoundSlider;

	// Token: 0x0400095C RID: 2396
	public SlideBar MusicSlider;

	// Token: 0x0400095D RID: 2397
	public ToggleButtonBehaviour SendNameButton;

	// Token: 0x0400095E RID: 2398
	public ToggleButtonBehaviour SendTelemButton;

	// Token: 0x0400095F RID: 2399
	public ToggleButtonBehaviour PersonalizedAdsButton;

	// Token: 0x04000960 RID: 2400
	public bool Toggle = true;

	// Token: 0x04000961 RID: 2401
	public TabGroup[] Tabs;
}
