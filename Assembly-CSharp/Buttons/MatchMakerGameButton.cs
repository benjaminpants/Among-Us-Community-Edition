using System;
using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

// Token: 0x02000151 RID: 337
public class MatchMakerGameButton : PoolableBehavior, IConnectButton
{
	// Token: 0x060006FE RID: 1790 RVA: 0x00028CE0 File Offset: 0x00026EE0
	public void OnClick()
	{
		if (!DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
		{
			return;
		}
		AmongUsClient.Instance.GameMode = GameModes.OnlineGame;
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		AmongUsClient.Instance.GameId = this.myListing.GameId;
		AmongUsClient.Instance.JoinGame();
		base.StartCoroutine(this.ConnectForFindGame());
	}

	// Token: 0x060006FF RID: 1791 RVA: 0x00006537 File Offset: 0x00004737
	private IEnumerator ConnectForFindGame()
	{
		yield return EndGameManager.WaitWithTimeout(() => AmongUsClient.Instance.ClientId >= 0 || AmongUsClient.Instance.LastDisconnectReason > DisconnectReasons.ExitGame);
		DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		yield break;
	}

	// Token: 0x06000700 RID: 1792 RVA: 0x0000653F File Offset: 0x0000473F
	public void StartIcon()
	{
		this.connectIcon.Play(this.connectClip, 1f);
	}

	// Token: 0x06000701 RID: 1793 RVA: 0x00006557 File Offset: 0x00004757
	public void StopIcon()
	{
		this.connectIcon.Stop();
		this.connectIcon.GetComponent<SpriteRenderer>().sprite = null;
	}

	// Token: 0x06000702 RID: 1794 RVA: 0x00028D44 File Offset: 0x00026F44
	public void SetGame(GameListing gameListing)
	{
		this.myListing = gameListing;
		this.NameText.Text = this.myListing.HostName;
		this.ImpostorCountText.Text = this.myListing.ImpostorCount.ToString();
		this.PlayerCountText.Text = string.Format("{0}/{1}", this.myListing.PlayerCount, this.myListing.MaxPlayers);
	}

	// Token: 0x06000704 RID: 1796 RVA: 0x00006575 File Offset: 0x00004775
	public void SetNoGame()
	{
		this.NameText.Text = "Public Lobbies are not supported!";
		this.ImpostorCountText.Text = "Please create a private game instead!";
		this.PlayerCountText.Text = "(The game is better with friends anyway)";
	}

	// Token: 0x040006C5 RID: 1733
	public TextRenderer NameText;

	// Token: 0x040006C6 RID: 1734
	public TextRenderer PlayerCountText;

	// Token: 0x040006C7 RID: 1735
	public TextRenderer ImpostorCountText;

	// Token: 0x040006C8 RID: 1736
	public SpriteAnim connectIcon;

	// Token: 0x040006C9 RID: 1737
	public AnimationClip connectClip;

	// Token: 0x040006CA RID: 1738
	public GameListing myListing;
}
