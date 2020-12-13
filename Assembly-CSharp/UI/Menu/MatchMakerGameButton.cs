using System.Collections;
using InnerNet;
using PowerTools;
using UnityEngine;

public class MatchMakerGameButton : PoolableBehavior, IConnectButton
{
	public TextRenderer NameText;

	public TextRenderer PlayerCountText;

	public TextRenderer ImpostorCountText;

	public SpriteAnim connectIcon;

	public AnimationClip connectClip;

	public GameListing myListing;

	public void OnClick()
	{
		if (DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
		{
			AmongUsClient.Instance.GameMode = GameModes.OnlineGame;
			AmongUsClient.Instance.OnlineScene = "OnlineGame";
			AmongUsClient.Instance.GameId = myListing.GameId;
			AmongUsClient.Instance.JoinGame();
			StartCoroutine(ConnectForFindGame());
		}
	}

	private IEnumerator ConnectForFindGame()
	{
		yield return EndGameManager.WaitWithTimeout(() => AmongUsClient.Instance.ClientId >= 0 || AmongUsClient.Instance.LastDisconnectReason != DisconnectReasons.ExitGame);
		DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
	}

	public void StartIcon()
	{
		connectIcon.Play(connectClip);
	}

	public void StopIcon()
	{
		connectIcon.Stop();
		connectIcon.GetComponent<SpriteRenderer>().sprite = null;
	}

	public void SetGame(GameListing gameListing)
	{
		myListing = gameListing;
		NameText.Text = myListing.HostName;
		ImpostorCountText.Text = myListing.ImpostorCount.ToString();
		PlayerCountText.Text = $"{myListing.PlayerCount}/{myListing.MaxPlayers}";
	}
}
