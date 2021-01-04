using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.CoreScripts;
using UnityEngine;
using System.IO;

public class EndGameManager : DestroyableSingleton<EndGameManager>
{
	public TextRenderer WinText;

	public MeshRenderer BackgroundBar;

	public MeshRenderer Foreground;

	public FloatRange ForegroundRadius;

	public SpriteRenderer FrontMost;

	public SpriteRenderer PlayerPrefab;

	public Sprite GhostSprite;

	public SpriteRenderer PlayAgainButton;

	public SpriteRenderer ExitButton;

	public AudioClip DisconnectStinger;

	public AudioClip CrewStinger;

	public AudioClip ImpostorStinger;

	public float BaseY = -0.25f;

	private float stingerTime;

	public Dictionary<int, List<SpriteRenderer>> TeamHatGroups = new Dictionary<int, List<SpriteRenderer>>();

	public void Start()
	{
		SaveManager.LastGameStart = DateTime.MinValue;
		if (TempData.showAd)
		{
			AdPlayer.RequestInterstitial();
		}
		SetEverythingUp();
		StartCoroutine(CoBegin());
		Invoke("ShowButtons", 1.1f);
	}

	private void ShowButtons()
	{
		FrontMost.gameObject.SetActive(value: false);
		PlayAgainButton.gameObject.SetActive(value: true);
		ExitButton.gameObject.SetActive(value: true);
	}

	private void SetEverythingUp()
	{
		Debug.Log(TempData.EndReason);
		if (TempData.EndReason == GameOverReason.ImpostorDisconnect)
		{
			WinText.Text = "Impostor\r\nDisconnected";
			SoundManager.Instance.PlaySound(DisconnectStinger, loop: false);
			return;
		}
		if (TempData.EndReason == GameOverReason.HumansDisconnect)
		{
			WinText.Text = "Most Crewmates\r\nDisconnected";
			SoundManager.Instance.PlaySound(DisconnectStinger, loop: false);
			return;
		}
		StatsManager.Instance.GamesFinished++;
		if (TempData.winners.Any((WinningPlayerData h) => h.IsYou))
		{
			//StatsManager.Instance.AddWinReason(TempData.EndReason);
			//stats manager temporarily disabled, planning to rework it entirely to support custom gamemodes
			WinText.Text = "Victory";
			BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
			WinningPlayerData winningPlayerData = TempData.winners.FirstOrDefault((WinningPlayerData h) => h.IsYou);
			if (winningPlayerData != null)
			{
				DestroyableSingleton<Telemetry>.Instance.WonGame(winningPlayerData.ColorId, winningPlayerData.HatId);
			}
		}
		else
		{
			//StatsManager.Instance.AddLoseReason(TempData.EndReason);
			//no
			WinText.Text = "Defeat";
			WinText.Color = Color.red;
		}
		if (TempData.DidHumansWin(TempData.EndReason))
		{
			SoundManager.Instance.PlayDynamicSound("Stinger", CrewStinger, loop: false, GetStingerVol);
		}
		else if (TempData.DidJokerWin(TempData.EndReason))
		{
			SoundManager.Instance.PlayDynamicSound("Stinger", DisconnectStinger, loop: false, GetStingerVol);
		}
		else if (TempData.EndReason == GameOverReason.Custom)
		{
			if (TempData.CustomStinger == "default_impostor")
            {
				SoundManager.Instance.PlayDynamicSound("Stinger", ImpostorStinger, loop: false, GetStingerVol);
			}
			else if (TempData.CustomStinger == "default_crewmate")
            {
				SoundManager.Instance.PlayDynamicSound("Stinger", CrewStinger, loop: false, GetStingerVol);
			}
			else if (TempData.CustomStinger == "default_disconnect")
            {
				SoundManager.Instance.PlayDynamicSound("Stinger", DisconnectStinger, loop: false, GetStingerVol);
			}
			else
            {
				SoundManager.Instance.PlayDynamicSound("Stinger", CE_WavUtility.ToAudioClip(Path.Combine(Application.dataPath, "CE_Assets", "Audio", "Victories", TempData.CustomStinger + ".wav")), loop: false, GetStingerVol);
			}
		}
		else
        {
			SoundManager.Instance.PlayDynamicSound("Stinger", ImpostorStinger, loop: false, GetStingerVol);
		}
		List<WinningPlayerData> list = TempData.winners.OrderBy((WinningPlayerData b) => b.IsYou ? (-1) : 0).ToList();
		for (int i = 0; i < list.Count; i++)
		{
			WinningPlayerData winningPlayerData2 = list[i];
				SpriteRenderer spriteRenderer = InstantiatePlayerPrefab(i);
			spriteRenderer.flipX = i % 2 == 0;
			if (winningPlayerData2.IsDead)
			{
				spriteRenderer.sprite = GhostSprite;
				spriteRenderer.flipX = !spriteRenderer.flipX;
			}
			float d = 1.25f;
			int num = ((i % 2 != 0) ? 1 : (-1));
			int num2 = (i + 1) / 2;
			float num3 = 1f - (float)num2 * 0.075f;
			float num4 = 1f - (float)num2 * 0.035f;
			float num5 = ((i == 0) ? (-8) : (-1));
			PlayerControl.SetPlayerMaterialColors(winningPlayerData2.ColorId, spriteRenderer);
			spriteRenderer.transform.localPosition = new Vector3(0.8f * (float)num * (float)num2 * num4, BaseY - 0.25f + (float)num2 * 0.1f, num5 + (float)num2 * 0.01f) * d;
			Vector3 vector = new Vector3(num3, num3, num3) * d;
			spriteRenderer.transform.localScale = vector;
			TextRenderer componentInChildren = spriteRenderer.GetComponentInChildren<TextRenderer>();
			if (TempData.DidHumansWin(TempData.EndReason))
			{
				componentInChildren.gameObject.SetActive(value: false);
			}
			else
			{
				componentInChildren.Text = winningPlayerData2.Name;
				componentInChildren.transform.localScale = vector.Inv();
			}
			if (!winningPlayerData2.IsDead)
			{
				SpriteRenderer component = spriteRenderer.transform.Find("SkinLayer").GetComponent<SpriteRenderer>();
				component.flipX = !spriteRenderer.flipX;
				DestroyableSingleton<HatManager>.Instance.SetSkin(component, winningPlayerData2.SkinId);
			}
			HatBegin(ref spriteRenderer, winningPlayerData2.HatId, winningPlayerData2);
		}
	}

	private void GetStingerVol(AudioSource source, float dt)
	{
		stingerTime += dt * 0.75f;
		source.volume = Mathf.Clamp(1f / stingerTime, 0f, 1f);
	}

	public IEnumerator CoBegin()
	{
		Color c = WinText.Color;
		Color fade = Color.black;
		_ = Color.white;
		Vector3 titlePos = WinText.transform.localPosition;
		float timer = 0f;
		while (timer < 3f)
		{
			timer += Time.deltaTime;
			float num = Mathf.Min(1f, timer / 3f);
			Foreground.material.SetFloat("_Rad", ForegroundRadius.ExpOutLerp(num * 2f));
			fade.a = Mathf.Lerp(1f, 0f, num * 3f);
			FrontMost.color = fade;
			c.a = Mathf.Clamp(FloatRange.ExpOutLerp(num, 0f, 1f), 0f, 1f);
			WinText.Color = c;
			titlePos.y = 2.7f - num * 0.3f;
			WinText.transform.localPosition = titlePos;
			yield return null;
		}
		FrontMost.gameObject.SetActive(value: false);
	}

	public void NextGame()
	{
		PlayAgainButton.gameObject.SetActive(value: false);
		ExitButton.gameObject.SetActive(value: false);
		if (TempData.showAd && !SaveManager.BoughtNoAds)
		{
			TempData.showAd = false;
			TempData.playAgain = true;
			AdPlayer.ShowInterstitial(this);
		}
		else
		{
			StartCoroutine(CoJoinGame());
		}
	}

	public IEnumerator CoJoinGame()
	{
		AmongUsClient.Instance.JoinGame();
		yield return WaitWithTimeout(() => AmongUsClient.Instance.ClientId >= 0);
		if (AmongUsClient.Instance.ClientId < 0)
		{
			AmongUsClient.Instance.ExitGame(AmongUsClient.Instance.LastDisconnectReason);
		}
	}

	public void Exit()
	{
		PlayAgainButton.gameObject.SetActive(value: false);
		ExitButton.gameObject.SetActive(value: false);
		if (TempData.showAd && !SaveManager.BoughtNoAds)
		{
			TempData.showAd = false;
			TempData.playAgain = false;
			AdPlayer.ShowInterstitial(this);
		}
		else
		{
			AmongUsClient.Instance.ExitGame();
		}
	}

	public static IEnumerator WaitWithTimeout(Func<bool> success)
	{
		for (float timer = 0f; timer < 5f; timer += Time.deltaTime)
		{
			if (success())
			{
				break;
			}
			yield return null;
		}
	}
	private SpriteRenderer InstantiatePlayerPrefab(int index)
	{
		var prefab = UnityEngine.Object.Instantiate(PlayerPrefab, base.transform);
		TeamHatGroups.Add(index, new List<SpriteRenderer>());
		TeamHatGroups[index].Add(prefab.transform.Find("HatSlot").GetComponent<SpriteRenderer>());
		TeamHatGroups[index].Add(CE_WardrobeManager.CreateExtHatCutscenes(prefab, 0));
		TeamHatGroups[index].Add(CE_WardrobeManager.CreateExtHatCutscenes(prefab, 1));
		TeamHatGroups[index].Add(CE_WardrobeManager.CreateExtHatCutscenes(prefab, 2));
		TeamHatGroups[index].Add(CE_WardrobeManager.CreateExtHatCutscenes(prefab, 3));
		return prefab;
	}
	private void Update()
	{
		for (int i = 0; i < TeamHatGroups.Count; i++)
		{
			var list = TeamHatGroups[i];
			SpriteRenderer refrence = null;
			for (int j = 0; j < list.Count; j++)
			{
				if (j != 0 && refrence)
				{
					CE_WardrobeManager.UpdateSpriteRenderer(TeamHatGroups[i][j], refrence);
				}
				else
				{
					refrence = TeamHatGroups[i][j];
				}

			}
		}
	}
	private void HatBegin(ref SpriteRenderer spriteRenderer, uint HatId, WinningPlayerData winningPlayerData2)
	{
		try
		{
			
			SpriteRenderer component2 = spriteRenderer.transform.Find("HatSlot").GetComponent<SpriteRenderer>();
			Vector3 localPosition = component2.transform.localPosition;
			if (winningPlayerData2.IsDead)
			{
				component2.flipX = spriteRenderer.flipX;
				component2.color = new Color(1f, 1f, 1f, 0.5f);
				localPosition.y = 0.725f;
			}
			else
			{
				component2.flipX = !spriteRenderer.flipX;
			}
			if (spriteRenderer.flipX)
			{
				localPosition.x = 0f - localPosition.x;
			}
			component2.transform.localPosition = localPosition;
			PlayerControl.SetHatImage(HatId, component2);
			

			for (int i = 0; i < 4; i++)
			{
				int index = i + 1;
				string childName = string.Format("ExtHatSlot{0}", index);
				SpriteRenderer component3 = spriteRenderer.transform.Find(childName).GetComponent<SpriteRenderer>();
				Vector3 localPosition2 = component3.transform.localPosition;
				if (winningPlayerData2.IsDead)
				{
					component3.flipX = spriteRenderer.flipX;
					component3.color = new Color(1f, 1f, 1f, 0.5f);
					localPosition2.y = 0.725f;
				}
				else
				{
					component3.flipX = !spriteRenderer.flipX;
				}
				if (spriteRenderer.flipX)
				{
					localPosition2.x = 0f - localPosition2.x;
				}
				component3.transform.localPosition = localPosition2;
				PlayerControl.SetHatImage(HatId, component3, index);
			}
		}
		catch(System.Exception E)
        {
			Debug.LogError(E.Message);
        }
	}
}
