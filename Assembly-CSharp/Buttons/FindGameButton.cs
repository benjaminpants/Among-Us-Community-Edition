using System;
using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000138 RID: 312
public class FindGameButton : MonoBehaviour, IConnectButton
{
	// Token: 0x06000697 RID: 1687 RVA: 0x00027354 File Offset: 0x00025554
	public void OnClick()
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
		AmongUsClient.Instance.GameMode = GameModes.OnlineGame;
		AmongUsClient.Instance.MainMenuScene = "MMOnline";
		base.StartCoroutine(this.ConnectForFindGame());
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x000061F4 File Offset: 0x000043F4
	private IEnumerator ConnectForFindGame()
	{
		AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, 25565);
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		AmongUsClient.Instance.mode = MatchMakerModes.Client;
		yield return AmongUsClient.Instance.CoConnect();
		if (AmongUsClient.Instance.LastDisconnectReason != DisconnectReasons.ExitGame)
		{
			DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		}
		else
		{
			AmongUsClient.Instance.HostId = AmongUsClient.Instance.ClientId;
			SceneManager.LoadScene("FindAGame");
		}
		yield break;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x000061FC File Offset: 0x000043FC
	public void StartIcon()
	{
		this.connectIcon.Play(this.connectClip, 1f);
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x00006214 File Offset: 0x00004414
	public void StopIcon()
	{
		this.connectIcon.Stop();
		this.connectIcon.GetComponent<SpriteRenderer>().sprite = null;
	}

	// Token: 0x0400065F RID: 1631
	public SpriteAnim connectIcon;

	// Token: 0x04000660 RID: 1632
	public AnimationClip connectClip;
}
