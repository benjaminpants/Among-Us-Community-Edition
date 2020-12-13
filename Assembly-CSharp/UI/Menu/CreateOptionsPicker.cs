using InnerNet;
using UnityEngine;

public class CreateOptionsPicker : MonoBehaviour
{
	public SpriteRenderer[] MaxPlayerButtons;

	public SpriteRenderer[] ImpostorButtons;

	public SpriteRenderer[] LanguageButtons;

	public SpriteRenderer[] MapButtons;

	public SettingsMode mode;

	public CrewVisualizer CrewArea;

	public void Start()
	{
		MapButtons[1].gameObject.SetActive(TempData.IsDo2Enabled);
		GameOptionsData targetOptions = GetTargetOptions();
		UpdateImpostorsButtons(targetOptions.NumImpostors);
		UpdateMaxPlayersButtons(targetOptions);
		UpdateLanguageButtons(targetOptions.Keywords & GameKeywords.AllLanguages);
		UpdateMapButtons(targetOptions.MapId);
	}

	private GameOptionsData GetTargetOptions()
	{
		if (mode == SettingsMode.Host)
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

	private void SetTargetOptions(GameOptionsData data)
	{
		if (mode == SettingsMode.Host)
		{
			SaveManager.GameHostOptions = data;
		}
		else
		{
			SaveManager.GameSearchOptions = data;
		}
	}

	public void SetMaxPlayersButtons(int maxPlayers)
	{
		GameOptionsData targetOptions = GetTargetOptions();
		if (maxPlayers >= GameOptionsData.MinPlayers[targetOptions.NumImpostors])
		{
			targetOptions.MaxPlayers = maxPlayers;
			SetTargetOptions(targetOptions);
			if (DestroyableSingleton<FindAGameManager>.InstanceExists)
			{
				DestroyableSingleton<FindAGameManager>.Instance.ResetTimer();
			}
			UpdateMaxPlayersButtons(targetOptions);
		}
	}

	private void UpdateMaxPlayersButtons(GameOptionsData opts)
	{
		if ((bool)CrewArea)
		{
			CrewArea.SetCrewSize(opts.MaxPlayers, opts.NumImpostors);
		}
		for (int i = 0; i < MaxPlayerButtons.Length; i++)
		{
			SpriteRenderer spriteRenderer = MaxPlayerButtons[i];
			spriteRenderer.enabled = spriteRenderer.name == opts.MaxPlayers.ToString();
			spriteRenderer.GetComponentInChildren<TextRenderer>().Color = ((int.Parse(spriteRenderer.name) < GameOptionsData.MinPlayers[opts.NumImpostors]) ? Palette.DisabledGrey : Color.white);
		}
	}

	public void SetImpostorButtons(int numImpostors)
	{
		GameOptionsData targetOptions = GetTargetOptions();
		targetOptions.NumImpostors = numImpostors;
		SetTargetOptions(targetOptions);
		SetMaxPlayersButtons(Mathf.Max(targetOptions.MaxPlayers, GameOptionsData.MinPlayers[numImpostors]));
		UpdateImpostorsButtons(numImpostors);
	}

	private void UpdateImpostorsButtons(int numImpostors)
	{
		for (int i = 0; i < ImpostorButtons.Length; i++)
		{
			SpriteRenderer obj = ImpostorButtons[i];
			obj.enabled = obj.name == numImpostors.ToString();
		}
	}

	public void SetMap(int mapid)
	{
		GameOptionsData targetOptions = GetTargetOptions();
		if (mode == SettingsMode.Host)
		{
			targetOptions.MapId = (byte)mapid;
		}
		else
		{
			targetOptions.ToggleMapFilter((byte)mapid);
		}
		SetTargetOptions(targetOptions);
		if (DestroyableSingleton<FindAGameManager>.InstanceExists)
		{
			DestroyableSingleton<FindAGameManager>.Instance.ResetTimer();
		}
		UpdateMapButtons(mapid);
	}

	private void UpdateMapButtons(int mapid)
	{
		if (mode == SettingsMode.Host)
		{
			if ((bool)CrewArea)
			{
				CrewArea.SetMap(mapid);
			}
			for (int i = 0; i < MapButtons.Length; i++)
			{
				SpriteRenderer obj = MapButtons[i];
				obj.color = ((obj.name == mapid.ToString()) ? Color.white : Palette.DisabledGrey);
			}
		}
		else
		{
			GameOptionsData targetOptions = GetTargetOptions();
			for (int j = 0; j < MapButtons.Length; j++)
			{
				SpriteRenderer spriteRenderer = MapButtons[j];
				spriteRenderer.color = (targetOptions.FilterContainsMap(byte.Parse(spriteRenderer.name)) ? Color.white : Palette.DisabledGrey);
			}
		}
	}

	public void SetLanguageFilter(int keyword)
	{
		GameOptionsData targetOptions = GetTargetOptions();
		targetOptions.Keywords &= ~GameKeywords.AllLanguages;
		targetOptions.Keywords |= (GameKeywords)keyword;
		SetTargetOptions(targetOptions);
		if (DestroyableSingleton<FindAGameManager>.InstanceExists)
		{
			DestroyableSingleton<FindAGameManager>.Instance.ResetTimer();
		}
		UpdateLanguageButtons((GameKeywords)keyword);
	}

	private void UpdateLanguageButtons(GameKeywords button)
	{
		for (int i = 0; i < LanguageButtons.Length; i++)
		{
			SpriteRenderer obj = LanguageButtons[i];
			obj.enabled = obj.name == button.ToString();
		}
	}
}
