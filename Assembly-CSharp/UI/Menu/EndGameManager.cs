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
			SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate(PlayerPrefab, base.transform);
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
			PlayerControl.SetHatImage(winningPlayerData2.HatId, component2);
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
}
