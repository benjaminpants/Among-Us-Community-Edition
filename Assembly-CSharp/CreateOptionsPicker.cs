using System;
using InnerNet;
using UnityEngine;

// Token: 0x020001BA RID: 442
public class CreateOptionsPicker : MonoBehaviour
{
	// Token: 0x0600098C RID: 2444 RVA: 0x00031C14 File Offset: 0x0002FE14
	public void Start()
	{
		//this.MapButtons[1].gameObject.SetActive(TempData.IsDo2Enabled);
		GameOptionsData targetOptions = this.GetTargetOptions();
		this.UpdateImpostorsButtons(targetOptions.NumImpostors);
		this.UpdateMaxPlayersButtons(targetOptions);
		this.UpdateLanguageButtons(targetOptions.Keywords & GameKeywords.AllLanguages);
		this.UpdateMapButtons((int)targetOptions.MapId);
	}

	// Token: 0x0600098D RID: 2445 RVA: 0x00031C70 File Offset: 0x0002FE70
	private GameOptionsData GetTargetOptions()
	{
		if (this.mode == SettingsMode.Host)
		{
			return SaveManager.GameHostOptions;
		}
		GameOptionsData gameSearchOptions = SaveManager.GameSearchOptions;
		if (gameSearchOptions.MapId == 0)
		{
			gameSearchOptions.ToggleMapFilter(0);
			SaveManager.GameSearchOptions = gameSearchOptions;
		}
		return gameSearchOptions;
	}

	// Token: 0x0600098E RID: 2446 RVA: 0x00007CF9 File Offset: 0x00005EF9
	private void SetTargetOptions(GameOptionsData data)
	{
		if (this.mode == SettingsMode.Host)
		{
			SaveManager.GameHostOptions = data;
			return;
		}
		SaveManager.GameSearchOptions = data;
	}

	// Token: 0x0600098F RID: 2447 RVA: 0x00031CA8 File Offset: 0x0002FEA8
	public void SetMaxPlayersButtons(int maxPlayers)
	{
		GameOptionsData targetOptions = this.GetTargetOptions();
		bool addCount = false;


		if (maxPlayers == 5) addCount = true;
		else if (maxPlayers == 4) addCount = false;
		else return;

		if (maxPlayers < targetOptions.NumImpostors) return;
		if (addCount && targetOptions.MaxPlayers + 1 > 20) return;
		if (!addCount && targetOptions.MaxPlayers - 1 < 4) return;

		targetOptions.MaxPlayers = (addCount ? targetOptions.MaxPlayers + 1 : targetOptions.MaxPlayers - 1);
		this.SetTargetOptions(targetOptions);
		if (DestroyableSingleton<FindAGameManager>.InstanceExists)
		{
			DestroyableSingleton<FindAGameManager>.Instance.ResetTimer();
		}
		this.UpdateMaxPlayersButtons(targetOptions);
	}

	// Token: 0x06000990 RID: 2448 RVA: 0x00031CF4 File Offset: 0x0002FEF4
	public void SetImpostorButtons(int numImpostors)
	{
		GameOptionsData targetOptions = this.GetTargetOptions();
		targetOptions.NumImpostors = numImpostors;
		this.SetTargetOptions(targetOptions);
		this.UpdateImpostorsButtons(numImpostors);
		this.UpdateMaxPlayersButtons(targetOptions);
	}

	// Token: 0x06000991 RID: 2449 RVA: 0x00031D20 File Offset: 0x0002FF20
	private void UpdateImpostorsButtons(int numImpostors)
	{
		for (int i = 0; i < this.ImpostorButtons.Length; i++)
		{
			SpriteRenderer spriteRenderer = this.ImpostorButtons[i];
			spriteRenderer.enabled = (spriteRenderer.name == numImpostors.ToString());
		}
	}

	// Token: 0x06000992 RID: 2450 RVA: 0x00031D60 File Offset: 0x0002FF60
	public void SetMap(int mapid)
	{
		GameOptionsData targetOptions = this.GetTargetOptions();
		if (this.mode == SettingsMode.Host)
		{
			targetOptions.MapId = (byte)mapid;
		}
		else
		{
			targetOptions.ToggleMapFilter((byte)mapid);
		}
		this.SetTargetOptions(targetOptions);
		if (DestroyableSingleton<FindAGameManager>.InstanceExists)
		{
			DestroyableSingleton<FindAGameManager>.Instance.ResetTimer();
		}
		this.UpdateMapButtons(mapid);
	}

	// Token: 0x06000993 RID: 2451 RVA: 0x00031DB0 File Offset: 0x0002FFB0
	private void UpdateMapButtons(int mapid)
	{
		if (this.mode == SettingsMode.Host)
		{
			if (this.CrewArea)
			{
				this.CrewArea.SetMap(mapid);
			}
			for (int i = 0; i < this.MapButtons.Length; i++)
			{
				SpriteRenderer spriteRenderer = this.MapButtons[i];
				spriteRenderer.color = ((spriteRenderer.name == mapid.ToString()) ? Color.white : Palette.DisabledGrey);
			}
			return;
		}
		GameOptionsData targetOptions = this.GetTargetOptions();
		for (int j = 0; j < this.MapButtons.Length; j++)
		{
			SpriteRenderer spriteRenderer2 = this.MapButtons[j];
			spriteRenderer2.color = (targetOptions.FilterContainsMap(byte.Parse(spriteRenderer2.name)) ? Color.white : Palette.DisabledGrey);
		}
	}

	// Token: 0x06000994 RID: 2452 RVA: 0x00031E68 File Offset: 0x00030068
	public void SetLanguageFilter(int keyword)
	{
		GameOptionsData targetOptions = this.GetTargetOptions();
		targetOptions.Keywords &= ~GameKeywords.AllLanguages;
		targetOptions.Keywords |= (GameKeywords)keyword;
		this.SetTargetOptions(targetOptions);
		if (DestroyableSingleton<FindAGameManager>.InstanceExists)
		{
			DestroyableSingleton<FindAGameManager>.Instance.ResetTimer();
		}
		this.UpdateLanguageButtons((GameKeywords)keyword);
	}

	// Token: 0x06000995 RID: 2453 RVA: 0x00031EB8 File Offset: 0x000300B8
	private void UpdateLanguageButtons(GameKeywords button)
	{
		for (int i = 0; i < this.LanguageButtons.Length; i++)
		{
			SpriteRenderer spriteRenderer = this.LanguageButtons[i];
			spriteRenderer.enabled = (spriteRenderer.name == button.ToString());
		}
	}

	// Token: 0x06000997 RID: 2455 RVA: 0x00031F00 File Offset: 0x00030100
	private void UpdateMaxPlayersButtons(GameOptionsData opts)
	{
		if (this.CrewArea)
		{
			this.CrewArea.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);
		}
		for (int i = 0; i < this.MaxPlayerButtons.Length; i++)
		{
			SpriteRenderer spriteRenderer = this.MaxPlayerButtons[i];
			if (spriteRenderer.name == "5") spriteRenderer.GetComponentInChildren<TextRenderer>().Text = "+";
			else if (spriteRenderer.name == "4") spriteRenderer.GetComponentInChildren<TextRenderer>().Text = "-";
			else
            {
				spriteRenderer.GetComponentInChildren<TextRenderer>().Text = "";
				spriteRenderer.enabled = false;
				spriteRenderer.GetComponentInChildren<TextRenderer>().Color = Palette.DisabledGrey;
			}

		}
	}

	// Token: 0x0400091D RID: 2333
	public SpriteRenderer[] MaxPlayerButtons;

	// Token: 0x0400091E RID: 2334
	public SpriteRenderer[] ImpostorButtons;

	// Token: 0x0400091F RID: 2335
	public SpriteRenderer[] LanguageButtons;

	// Token: 0x04000920 RID: 2336
	public SpriteRenderer[] MapButtons;

	// Token: 0x04000921 RID: 2337
	public SettingsMode mode;

	// Token: 0x04000922 RID: 2338
	public CrewVisualizer CrewArea;
}
