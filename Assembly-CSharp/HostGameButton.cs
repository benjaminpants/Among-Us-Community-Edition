using System;
using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

// Token: 0x0200013C RID: 316
public class HostGameButton : MonoBehaviour, IConnectButton
{
	// Token: 0x060006A7 RID: 1703 RVA: 0x00006291 File Offset: 0x00004491
	public void Start()
	{
		if (DestroyableSingleton<MatchMaker>.InstanceExists)
		{
			DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		}
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x0002762C File Offset: 0x0002582C
	public void OnClick()
	{
		if (this.GameMode == GameModes.FreePlay)
		{
			if (!NameTextBehaviour.IsValidName(SaveManager.PlayerName))
			{
				SaveManager.PlayerName = "Player";
			}
		}
		else
		{
			if (NameTextBehaviour.Instance.ShakeIfInvalid())
			{
				return;
			}
			if (SaveManager.AmBanned)
			{
				AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.IntentionalLeaving;
				DestroyableSingleton<DisconnectPopup>.Instance.Show();
				return;
			}
			if (!DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
			{
				return;
			}
		}
		base.StartCoroutine(this.CoStartGame());
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000062A4 File Offset: 0x000044A4
	public void StartIcon()
	{
		if (!this.connectIcon)
		{
			return;
		}
		this.connectIcon.Play(this.connectClip, 1f);
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x000062CA File Offset: 0x000044CA
	public void StopIcon()
	{
		if (!this.connectIcon)
		{
			return;
		}
		this.connectIcon.Stop();
		this.connectIcon.GetComponent<SpriteRenderer>().sprite = null;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x000062F6 File Offset: 0x000044F6
	private IEnumerator CoStartGame()
	{
		try
		{
			SoundManager.Instance.StopAllSound();
			AmongUsClient.Instance.GameMode = this.GameMode;
			switch (this.GameMode)
			{
			case GameModes.LocalGame:
				DestroyableSingleton<InnerNetServer>.Instance.StartAsServer();
				AmongUsClient.Instance.SetEndpoint("127.0.0.1", 22023);
				AmongUsClient.Instance.MainMenuScene = "MatchMaking";
				break;
			case GameModes.OnlineGame:
				AmongUsClient.Instance.SetEndpoint("24.181.130.52", 25565);
				AmongUsClient.Instance.MainMenuScene = "MMOnline";
				break;
			case GameModes.FreePlay:
				DestroyableSingleton<InnerNetServer>.Instance.StartAsServer();
				AmongUsClient.Instance.SetEndpoint("127.0.0.1", 22023);
				AmongUsClient.Instance.MainMenuScene = "MainMenu";
				break;
			}
		}
		catch (Exception ex)
		{
			DestroyableSingleton<DisconnectPopup>.Instance.ShowCustom(ex.Message);
			DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
			yield break;
		}
		yield return new WaitForSeconds(0.1f);
		if (this.FillScreen)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", null, 0.5f, 1.5f);
			this.FillScreen.gameObject.SetActive(true);
			for (float time = 0f; time < 0.25f; time += Time.deltaTime)
			{
				this.FillScreen.color = Color.Lerp(Color.clear, Color.black, time / 0.25f);
				yield return null;
			}
			this.FillScreen.color = Color.black;
		}
		AmongUsClient.Instance.OnlineScene = this.targetScene;
		AmongUsClient.Instance.Connect(MatchMakerModes.HostAndClient);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		if (AmongUsClient.Instance.mode == MatchMakerModes.None && this.FillScreen)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", this.IntroMusic, 0.5f, 1.5f);
			for (float time = 0f; time < 0.25f; time += Time.deltaTime)
			{
				this.FillScreen.color = Color.Lerp(Color.black, Color.clear, time / 0.25f);
				yield return null;
			}
			this.FillScreen.color = Color.clear;
		}
		yield break;
	}

	// Token: 0x0400066C RID: 1644
	public AudioClip IntroMusic;

	// Token: 0x0400066D RID: 1645
	public string targetScene;

	// Token: 0x0400066E RID: 1646
	public SpriteRenderer FillScreen;

	// Token: 0x0400066F RID: 1647
	public SpriteAnim connectIcon;

	// Token: 0x04000670 RID: 1648
	public AnimationClip connectClip;

	// Token: 0x04000671 RID: 1649
	public GameModes GameMode;
}
