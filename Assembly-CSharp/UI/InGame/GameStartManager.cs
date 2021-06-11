using InnerNet;
using UnityEngine;

public class GameStartManager : DestroyableSingleton<GameStartManager>, IDisconnectHandler
{
	public int MinPlayers = 4; //yes

	public TextRenderer PlayerCounter;

	private int LastPlayerCount = -1;

	public GameObject GameSizePopup;

	public TextRenderer GameRoomName;

	public PlayerControl PlayerPrefab;

	public LobbyBehaviour LobbyPrefab;

	public SpriteRenderer StartButton;

	public SpriteRenderer MakePublicButton;

	public Sprite PublicGameImage;

	public Sprite PrivateGameImage;

	public bool starting;

	public Gradient PlayerCountGradient = new Gradient();

	public void Start()
	{
		MinPlayers = 2;
		if (DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		string text = InnerNetClient.IntToGameName(AmongUsClient.Instance.GameId);
		if (text != null)
		{
			GameRoomName.Text = "Room\r\n[r0]" + text + "[]";
		}
		else
		{
			StartButton.transform.localPosition = new Vector3(0f, -0.2f, 0f);
			PlayerCounter.transform.localPosition = new Vector3(0f, -0.8f, 0f);
		}
		AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
		MakePublicButton.gameObject.SetActive(false);
		if (!AmongUsClient.Instance.AmHost)
		{
			StartButton.gameObject.SetActive(value: false);
		}
		else
		{
			LobbyBehaviour.Instance = Object.Instantiate(LobbyPrefab);
			AmongUsClient.Instance.Spawn(LobbyBehaviour.Instance);
			MakePublicButton.gameObject.SetActive(false); //no
		}
        GradientColorKey[] colorkey = new GradientColorKey[3];
		GradientAlphaKey[] alphakeys = new GradientAlphaKey[1];
		colorkey[0].color = Color.red;
        colorkey[0].time = 0.0f;
		colorkey[1].color = new Color(1.0f,1.0f,0f,1f);
        colorkey[1].time = 0.5f;
		colorkey[2].color = Color.green;
		colorkey[2].time = 1f;
		alphakeys[0].alpha = 1.0f;
		alphakeys[0].time = 0.0f;
		PlayerCountGradient.colorKeys = colorkey;
		PlayerCountGradient.alphaKeys = alphakeys;
	}

	public void MakePublic()
	{
		if (AmongUsClient.Instance.AmHost)
		{
			AmongUsClient.Instance.ChangeGamePublic(!AmongUsClient.Instance.IsGamePublic);
		}
	}

	public void Update()
	{
		if (!GameData.Instance)
		{
			return;
		}
		MakePublicButton.sprite = (AmongUsClient.Instance.IsGamePublic ? PublicGameImage : PrivateGameImage);
		if (GameData.Instance.PlayerCount == LastPlayerCount) 
		{
			return;
		}
		LastPlayerCount = GameData.Instance.PlayerCount;
		Color color = PlayerCountGradient.Evaluate((float)LastPlayerCount / (float)PlayerControl.GameOptions.MaxPlayers); //despite what VS says those float conversions are necessary do not remove them
		string arg = "[" + ColorUtility.ToHtmlStringRGBA(color) + "]";
		PlayerCounter.Text = $"{arg}{LastPlayerCount}/{PlayerControl.GameOptions.MaxPlayers}";
		StartButton.color = ((LastPlayerCount >= MinPlayers) ? Palette.EnabledColor : Palette.DisabledColor);
		if (DestroyableSingleton<DiscordManager>.InstanceExists)
		{
			if (AmongUsClient.Instance.AmHost && AmongUsClient.Instance.GameMode == GameModes.OnlineGame)
			{
				DestroyableSingleton<DiscordManager>.Instance.SetInLobbyHost(LastPlayerCount, AmongUsClient.Instance.GameId);
			}
			else
			{
				DestroyableSingleton<DiscordManager>.Instance.SetInLobbyClient();
			}
		}
	}

	public void BeginGame()
	{
		if (SaveManager.ShowMinPlayerWarning && GameData.Instance.PlayerCount == MinPlayers || GameData.Instance.PlayerCount == 3 || (GameData.Instance.PlayerCount == 4))
		{
			GameSizePopup.SetActive(value: true);
			if (GameData.Instance.PlayerCount == 2 || (GameData.Instance.PlayerCount == 3))
			{
				GameSizePopup.transform.Find("Text").GetComponent<TextRenderer>().Text = "Among Us: CE allows you to play with just\n" + (GameData.Instance.PlayerCount) + " players, however the majority of \ngamemodes require 4-5 players.\nWith 5 players being recommended.\nAre you sure you want to start?";
			}
			else if (GameData.Instance.PlayerCount == 4)
			{
				GameSizePopup.transform.Find("Text").GetComponent<TextRenderer>().Text = "Among Us: CE allows you to play with just\n4 players.\n\n5+ players however is recommended.\nAre you sure you want to start?";
			}
		}
		else if (GameData.Instance.PlayerCount < MinPlayers)
		{
			StartCoroutine(Effects.Shake(PlayerCounter.transform));
		}
		else
		{
			ReallyBegin(neverShow: false);
		}
	}

	public void ReallyBegin(bool neverShow)
	{
		if (!starting)
		{
			starting = true;
			if (neverShow)
			{
				SaveManager.ShowMinPlayerWarning = false;
			}
			AmongUsClient.Instance.StartGame();
			AmongUsClient.Instance.DisconnectHandlers.Remove(this);
			Object.Destroy(base.gameObject);
		}
	}

	public void HandleDisconnect(PlayerControl pc, DisconnectReasons reason)
	{
		if (AmongUsClient.Instance.AmHost)
		{
			LastPlayerCount = -1;
			if ((bool)StartButton)
			{
				StartButton.gameObject.SetActive(value: true);
			}
		}
	}

	public void HandleDisconnect()
	{
		HandleDisconnect(null, DisconnectReasons.ExitGame);
	}
}
