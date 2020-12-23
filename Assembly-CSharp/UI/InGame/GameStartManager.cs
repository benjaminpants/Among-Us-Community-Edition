using InnerNet;
using UnityEngine;

public class GameStartManager : DestroyableSingleton<GameStartManager>, IDisconnectHandler
{
	public int MinPlayers = 2;

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

	public void Start()
	{
		if (DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		string text = InnerNetClient.IntToGameName(AmongUsClient.Instance.GameId);
		if (text != null)
		{
			GameRoomName.Text = "Room\r\n" + text;
		}
		else
		{
			StartButton.transform.localPosition = new Vector3(0f, -0.2f, 0f);
			PlayerCounter.transform.localPosition = new Vector3(0f, -0.8f, 0f);
		}
		AmongUsClient.Instance.DisconnectHandlers.AddUnique(this);
		if (!AmongUsClient.Instance.AmHost)
		{
			StartButton.gameObject.SetActive(value: false);
		}
		else
		{
			LobbyBehaviour.Instance = Object.Instantiate(LobbyPrefab);
			AmongUsClient.Instance.Spawn(LobbyBehaviour.Instance);
		}
		MakePublicButton.gameObject.SetActive(AmongUsClient.Instance.GameMode == GameModes.OnlineGame);
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
		string arg = "[FF0000FF]";
		if (LastPlayerCount > MinPlayers)
		{
			arg = "[00FF00FF]";
		}
		if (LastPlayerCount == MinPlayers)
		{
			arg = "[FFFF00FF]";
		}
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
		if (SaveManager.ShowMinPlayerWarning && GameData.Instance.PlayerCount == MinPlayers)
		{
			GameSizePopup.SetActive(value: true);
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
