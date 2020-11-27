using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.CoreScripts;
using InnerNet;
using UnityEngine;

// Token: 0x02000068 RID: 104
public class EndGameManager : DestroyableSingleton<EndGameManager>
{
	// Token: 0x06000230 RID: 560 RVA: 0x0000361A File Offset: 0x0000181A
	public void Start()
	{
		SaveManager.LastGameStart = DateTime.MinValue;
		if (TempData.showAd)
		{
			AdPlayer.RequestInterstitial();
		}
		this.SetEverythingUp();
		base.StartCoroutine(this.CoBegin());
		base.Invoke("ShowButtons", 1.1f);
	}

	// Token: 0x06000231 RID: 561 RVA: 0x00003655 File Offset: 0x00001855
	private void ShowButtons()
	{
		this.FrontMost.gameObject.SetActive(false);
		this.PlayAgainButton.gameObject.SetActive(true);
		this.ExitButton.gameObject.SetActive(true);
	}

	// Token: 0x06000232 RID: 562 RVA: 0x000124EC File Offset: 0x000106EC
	private void SetEverythingUp()
	{
		if (TempData.EndReason == GameOverReason.ImpostorDisconnect)
		{
			this.WinText.Text = "Impostor\r\nDisconnected";
			SoundManager.Instance.PlaySound(this.DisconnectStinger, false, 1f);
			return;
		}
		if (TempData.EndReason == GameOverReason.HumansDisconnect)
		{
			this.WinText.Text = "Most Crewmates\r\nDisconnected";
			SoundManager.Instance.PlaySound(this.DisconnectStinger, false, 1f);
			return;
		}
		StatsManager instance = StatsManager.Instance;
		uint gamesFinished = instance.GamesFinished;
		instance.GamesFinished = gamesFinished + 1U;
		if (TempData.winners.Any((WinningPlayerData h) => h.IsYou))
		{
			StatsManager.Instance.AddWinReason(TempData.EndReason);
			this.WinText.Text = "Victory";
			this.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
			WinningPlayerData winningPlayerData = TempData.winners.FirstOrDefault((WinningPlayerData h) => h.IsYou);
			if (winningPlayerData != null)
			{
				DestroyableSingleton<Telemetry>.Instance.WonGame(winningPlayerData.ColorId, winningPlayerData.HatId);
			}
		}
		else
		{
			StatsManager.Instance.AddLoseReason(TempData.EndReason);
			this.WinText.Text = "Defeat";
			this.WinText.Color = Color.red;
		}
		bool flag = TempData.DidHumansWin(TempData.EndReason);
		if (flag)
		{
			SoundManager.Instance.PlayDynamicSound("Stinger", this.CrewStinger, false, new DynamicSound.GetDynamicsFunction(this.GetStingerVol), false);
		}
		else
		{
			SoundManager.Instance.PlayDynamicSound("Stinger", this.ImpostorStinger, false, new DynamicSound.GetDynamicsFunction(this.GetStingerVol), false);
		}
		List<WinningPlayerData> list = TempData.winners.OrderBy(delegate(WinningPlayerData b)
		{
			if (!b.IsYou)
			{
				return 0;
			}
			return -1;
		}).ToList<WinningPlayerData>();
		for (int i = 0; i < list.Count; i++)
		{
			WinningPlayerData winningPlayerData2 = list[i];
			SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(this.PlayerPrefab, base.transform);
			spriteRenderer.flipX = (i % 2 == 0);
			if (winningPlayerData2.IsDead)
			{
				spriteRenderer.sprite = this.GhostSprite;
				spriteRenderer.flipX = !spriteRenderer.flipX;
			}
			float d = 1.25f;
			int num = (i % 2 == 0) ? -1 : 1;
			int num2 = (i + 1) / 2;
			float num3 = 1f - (float)num2 * 0.075f;
			float num4 = 1f - (float)num2 * 0.035f;
			float num5 = (float)((i == 0) ? -8 : -1);
			PlayerControl.SetPlayerMaterialColors(winningPlayerData2.ColorId, spriteRenderer);
			spriteRenderer.transform.localPosition = new Vector3(0.8f * (float)num * (float)num2 * num4, this.BaseY - 0.25f + (float)num2 * 0.1f, num5 + (float)num2 * 0.01f) * d;
			Vector3 vector = new Vector3(num3, num3, num3) * d;
			spriteRenderer.transform.localScale = vector;
			TextRenderer componentInChildren = spriteRenderer.GetComponentInChildren<TextRenderer>();
			if (flag)
			{
				componentInChildren.gameObject.SetActive(false);
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
				localPosition.x = -localPosition.x;
			}
			component2.transform.localPosition = localPosition;
			PlayerControl.SetHatImage(winningPlayerData2.HatId, component2);
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x0000368A File Offset: 0x0000188A
	private void GetStingerVol(AudioSource source, float dt)
	{
		this.stingerTime += dt * 0.75f;
		source.volume = Mathf.Clamp(1f / this.stingerTime, 0f, 1f);
	}

	// Token: 0x06000234 RID: 564 RVA: 0x000036C1 File Offset: 0x000018C1
	public IEnumerator CoBegin()
	{
		Color c = this.WinText.Color;
		Color fade = Color.black;
		Color white = Color.white;
		Vector3 titlePos = this.WinText.transform.localPosition;
		float timer = 0f;
		while (timer < 3f)
		{
			timer += Time.deltaTime;
			float num = Mathf.Min(1f, timer / 3f);
			this.Foreground.material.SetFloat("_Rad", this.ForegroundRadius.ExpOutLerp(num * 2f));
			fade.a = Mathf.Lerp(1f, 0f, num * 3f);
			this.FrontMost.color = fade;
			c.a = Mathf.Clamp(FloatRange.ExpOutLerp(num, 0f, 1f), 0f, 1f);
			this.WinText.Color = c;
			titlePos.y = 2.7f - num * 0.3f;
			this.WinText.transform.localPosition = titlePos;
			yield return null;
		}
		this.FrontMost.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00012928 File Offset: 0x00010B28
	public void NextGame()
	{
		this.PlayAgainButton.gameObject.SetActive(false);
		this.ExitButton.gameObject.SetActive(false);
		if (TempData.showAd && !SaveManager.BoughtNoAds)
		{
			TempData.showAd = false;
			TempData.playAgain = true;
			AdPlayer.ShowInterstitial(this);
			return;
		}
		base.StartCoroutine(this.CoJoinGame());
	}

	// Token: 0x06000236 RID: 566 RVA: 0x000036D0 File Offset: 0x000018D0
	public IEnumerator CoJoinGame()
	{
		AmongUsClient.Instance.JoinGame();
		yield return EndGameManager.WaitWithTimeout(() => AmongUsClient.Instance.ClientId >= 0);
		if (AmongUsClient.Instance.ClientId < 0)
		{
			AmongUsClient.Instance.ExitGame(AmongUsClient.Instance.LastDisconnectReason);
		}
		yield break;
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00012988 File Offset: 0x00010B88
	public void Exit()
	{
		this.PlayAgainButton.gameObject.SetActive(false);
		this.ExitButton.gameObject.SetActive(false);
		if (TempData.showAd && !SaveManager.BoughtNoAds)
		{
			TempData.showAd = false;
			TempData.playAgain = false;
			AdPlayer.ShowInterstitial(this);
			return;
		}
		AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
	}

	// Token: 0x06000238 RID: 568 RVA: 0x000036D8 File Offset: 0x000018D8
	public static IEnumerator WaitWithTimeout(Func<bool> success)
	{
		float timer = 0f;
		while (timer < 5f && !success())
		{
			yield return null;
			timer += Time.deltaTime;
		}
		yield break;
	}

	// Token: 0x04000214 RID: 532
	public TextRenderer WinText;

	// Token: 0x04000215 RID: 533
	public MeshRenderer BackgroundBar;

	// Token: 0x04000216 RID: 534
	public MeshRenderer Foreground;

	// Token: 0x04000217 RID: 535
	public FloatRange ForegroundRadius;

	// Token: 0x04000218 RID: 536
	public SpriteRenderer FrontMost;

	// Token: 0x04000219 RID: 537
	public SpriteRenderer PlayerPrefab;

	// Token: 0x0400021A RID: 538
	public Sprite GhostSprite;

	// Token: 0x0400021B RID: 539
	public SpriteRenderer PlayAgainButton;

	// Token: 0x0400021C RID: 540
	public SpriteRenderer ExitButton;

	// Token: 0x0400021D RID: 541
	public AudioClip DisconnectStinger;

	// Token: 0x0400021E RID: 542
	public AudioClip CrewStinger;

	// Token: 0x0400021F RID: 543
	public AudioClip ImpostorStinger;

	// Token: 0x04000220 RID: 544
	public float BaseY = -0.25f;

	// Token: 0x04000221 RID: 545
	private float stingerTime;
}
