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
		ServerManager.Instance.LastServer = ServerManager.Instance.availableServers[myListing.ListingID];
		if (DestroyableSingleton<MatchMaker>.Instance.Connecting(this))
		{
			DestroyableSingleton<MatchMaker>.Instance.NotConnecting();
		}
	}

	public void Start()
    {
		try
		{
			GameObject icon = NameText.transform.parent.gameObject.transform.Find("mapJourney_icon").gameObject;
			Texture2D tex = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "Icons", myListing.Icon));
			icon.GetComponent<SpriteRenderer>().sprite = CE_TextureNSpriteExtensions.ConvertToSprite(tex, new Vector2(0.5f, 0.5f));
		}
		catch(System.Exception E)
        {
			Debug.LogError(E.Message + "\n" + E.StackTrace);
        }
	}

	private IEnumerator ConnectForFindGame()
	{
		yield return EndGameManager.WaitWithTimeout(() => AmongUsClient.Instance.ClientId >= 0 || AmongUsClient.Instance.LastDisconnectReason != DisconnectReasons.ExitGame);
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

	public void SetGame(GameListing gameListing, bool isserver = true)
	{
		myListing = gameListing;
		NameText.Text = myListing.HostName;
		NameText.scale = 0.5f;
		ImpostorCountText.Text = myListing.ImpostorCount.ToString();
		PlayerCountText.Text = $"{myListing.PlayerCount}/{myListing.MaxPlayers}";
	}

	public void SetNoGame()
	{
		NameText.Text = "Public Lobbies are not supported!";
		ImpostorCountText.Text = "Please create a private game instead!";
		PlayerCountText.Text = "(The game is better with friends anyway)";
	}
}
