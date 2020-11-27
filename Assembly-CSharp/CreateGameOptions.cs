using System;
using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class CreateGameOptions : MonoBehaviour, IConnectButton
{
	// Token: 0x0600020E RID: 526 RVA: 0x00011F7C File Offset: 0x0001017C
	public void Show()
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
		base.gameObject.SetActive(true);
		this.Content.SetActive(false);
		base.StartCoroutine(this.CoShow());
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000034EF File Offset: 0x000016EF
	private IEnumerator CoShow()
	{
		yield return Effects.ColorFade(this.Foreground, Color.clear, Color.black, 0.1f);
		this.Content.SetActive(true);
		yield return Effects.ColorFade(this.Foreground, Color.black, Color.clear, 0.1f);
		yield break;
	}

	// Token: 0x06000210 RID: 528 RVA: 0x000034FE File Offset: 0x000016FE
	public void StartIcon()
	{
		if (!this.connectIcon)
		{
			return;
		}
		this.connectIcon.Play(this.connectClip, 1f);
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00003524 File Offset: 0x00001724
	public void StopIcon()
	{
		if (!this.connectIcon)
		{
			return;
		}
		this.connectIcon.Stop();
		this.connectIcon.GetComponent<SpriteRenderer>().sprite = null;
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00003550 File Offset: 0x00001750
	public void Hide()
	{
		base.StartCoroutine(this.CoHide());
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000355F File Offset: 0x0000175F
	private IEnumerator CoHide()
	{
		yield return Effects.ColorFade(this.Foreground, Color.clear, Color.black, 0.1f);
		this.Content.SetActive(false);
		yield return Effects.ColorFade(this.Foreground, Color.black, Color.clear, 0.1f);
		base.gameObject.SetActive(false);
		yield break;
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0000356E File Offset: 0x0000176E
	public void Confirm()
	{
		if (!DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
		{
			return;
		}
		base.StartCoroutine(this.CoStartGame());
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000358B File Offset: 0x0000178B
	private IEnumerator CoStartGame()
	{
		SoundManager.Instance.CrossFadeSound("MainBG", null, 0.5f, 1.5f);
		yield return Effects.ColorFade(this.Foreground, Color.clear, Color.black, 0.2f);
		AmongUsClient.Instance.GameMode = GameModes.OnlineGame;
		AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, 25565);
		AmongUsClient.Instance.MainMenuScene = "MMOnline";
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		AmongUsClient.Instance.Connect(MatchMakerModes.HostAndClient);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		if (AmongUsClient.Instance.mode == MatchMakerModes.None)
		{
			SoundManager.Instance.CrossFadeSound("MainBG", this.IntroMusic, 0.5f, 1.5f);
			yield return Effects.ColorFade(this.Foreground, Color.black, Color.clear, 0.2f);
		}
		yield break;
	}

	// Token: 0x040001FC RID: 508
	public AudioClip IntroMusic;

	// Token: 0x040001FD RID: 509
	public GameObject Content;

	// Token: 0x040001FE RID: 510
	public SpriteRenderer Foreground;

	// Token: 0x040001FF RID: 511
	public SpriteAnim connectIcon;

	// Token: 0x04000200 RID: 512
	public AnimationClip connectClip;
}
