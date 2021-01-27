using System;
using System.Collections;
using InnerNet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiscordManager : DestroyableSingleton<DiscordManager>
{
	private DiscordRpc.RichPresence presence;

	public DiscordRpc.DiscordUser joinRequest;

	private DateTime? StartTime;

	private static readonly DateTime epoch;

	private readonly string[] Gamemodes;

	private readonly string[] GamemodeIcons;

	public void Start()
	{
		if (DestroyableSingleton<DiscordManager>.Instance == this)
		{
			DiscordRpc.EventHandlers handlers = default(DiscordRpc.EventHandlers);
			handlers.errorCallback = (DiscordRpc.ErrorCallback)Delegate.Combine(handlers.errorCallback, new DiscordRpc.ErrorCallback(HandleError));
			handlers.disconnectedCallback = (DiscordRpc.DisconnectedCallback)Delegate.Combine(handlers.disconnectedCallback, new DiscordRpc.DisconnectedCallback(HandleError));
			handlers.joinCallback = (DiscordRpc.JoinCallback)Delegate.Combine(handlers.joinCallback, new DiscordRpc.JoinCallback(HandleJoinRequest));
			handlers.requestCallback = (DiscordRpc.RequestCallback)Delegate.Combine(handlers.requestCallback, new DiscordRpc.RequestCallback(HandleAutoJoin));
			DiscordRpc.Initialize("780630509704708096", ref handlers, autoRegister: true, null);
			SetInMenus();
			SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode mode)
			{
				OnSceneChange(scene.name);
			};
		}
	}

	private void HandleError(int errorCode, string message)
	{
		Debug.LogError(message ?? $"No message: {errorCode}");
	}

	private void OnSceneChange(string name)
	{
		switch (name)
		{
		case "MatchMaking":
		case "MMOnline":
		case "MainMenu":
			SetInMenus();
			break;
		}
	}

	public void FixedUpdate()
	{
		DiscordRpc.RunCallbacks();
	}

	public void SetInMenus()
	{
		ClearPresence();
		StartTime = null;
		presence.state = "In Menus";
		presence.largeImageKey = "icon_menu";
		DiscordRpc.UpdatePresence(presence);
	}

	public void SetPlayingGame()
	{
		if (!StartTime.HasValue)
		{
			StartTime = DateTime.UtcNow;
		}
		presence.state = "In Game";
		presence.details = "Playing " + GameOptionsData.Gamemodes[PlayerControl.GameOptions.Gamemode];
		presence.largeImageKey = "icon";
		presence.startTimestamp = ToUnixTime(StartTime.Value);
		DiscordRpc.UpdatePresence(presence);
	}

	public void SetHowToPlay()
	{
		ClearPresence();
		presence.state = "In Freeplay";
		presence.largeImageKey = "icon_freeplay";
		DiscordRpc.UpdatePresence(presence);
	}

	public void SetInLobbyClient()
	{
		if (!StartTime.HasValue)
		{
			StartTime = DateTime.UtcNow;
		}
		ClearPresence();
		presence.state = "In Lobby";
		presence.largeImageKey = "icon_lobby";
		presence.startTimestamp = ToUnixTime(StartTime.Value);
		DiscordRpc.UpdatePresence(presence);
	}

	private void ClearPresence()
	{
		presence.startTimestamp = 0L;
		presence.details = null;
		presence.partyId = null;
		presence.matchSecret = null;
		presence.joinSecret = null;
		presence.partySize = 0;
		presence.partyMax = 0;
	}

	public void SetInLobbyHost(int numPlayers, int gameId)
	{
		if (!StartTime.HasValue)
		{
			StartTime = DateTime.UtcNow;
		}
		string text = InnerNetClient.IntToGameName(gameId);
		presence.state = "In Lobby";
		presence.details = "Hosting a game";
		presence.partySize = numPlayers;
		presence.partyMax = 20;
		presence.smallImageKey = "icon_lobby";
		presence.largeImageText = "Ask to play!";
		presence.joinSecret = "join" + text;
		presence.matchSecret = "match" + text;
		presence.partyId = text;
		DiscordRpc.UpdatePresence(presence);
	}

	private void HandleAutoJoin(ref DiscordRpc.DiscordUser requestUser)
	{
		Debug.Log("Discord: request from " + requestUser.username);
		if (AmongUsClient.Instance.IsGameStarted)
		{
			RequestRespondNo();
		}
		else
		{
			RequestRespondYes();
		}
	}

	private void HandleJoinRequest(string joinSecret)
	{
		if (!joinSecret.StartsWith("join"))
		{
			Debug.LogWarning("Invalid join secret: " + joinSecret);
			return;
		}
		if (!AmongUsClient.Instance)
		{
			Debug.LogWarning("Missing AmongUsClient");
			return;
		}
		if (!DestroyableSingleton<DiscordManager>.InstanceExists)
		{
			Debug.LogWarning("Missing DiscordManager");
			return;
		}
		if (AmongUsClient.Instance.mode != 0)
		{
			Debug.LogWarning("Already connected");
			return;
		}
		AmongUsClient.Instance.GameMode = GameModes.OnlineGame;
		AmongUsClient.Instance.GameId = InnerNetClient.GameNameToInt(joinSecret.Substring(4));
		AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, 25565);
		AmongUsClient.Instance.MainMenuScene = "MMOnline";
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		DestroyableSingleton<DiscordManager>.Instance.StopAllCoroutines();
		DestroyableSingleton<DiscordManager>.Instance.StartCoroutine(DestroyableSingleton<DiscordManager>.Instance.CoJoinGame());
	}

	public IEnumerator CoJoinGame()
	{
		while ((bool)DataCollectScreen.Instance && DataCollectScreen.Instance.isActiveAndEnabled)
		{
			yield return null;
		}
		AmongUsClient.Instance.Connect(MatchMakerModes.Client);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		if (AmongUsClient.Instance.ClientId < 0)
		{
			SceneManager.LoadScene("MMOnline");
		}
	}

	public void RequestRespondYes()
	{
		DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.Yes);
	}

	public void RequestRespondNo()
	{
		Debug.Log("Discord: responding no to Ask to Join request");
		DiscordRpc.Respond(joinRequest.userId, DiscordRpc.Reply.No);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		if (DestroyableSingleton<DiscordManager>.Instance == this)
		{
			DiscordRpc.Shutdown();
		}
	}

	private static long ToUnixTime(DateTime time)
	{
		return (long)(time - epoch).TotalSeconds;
	}

	public DiscordManager()
	{
		Gamemodes = new string[5]
		{
			"Classic",
			"Zombies",
			"Murder",
			"Hot Potato",
			"Classic+Joker"
		};
		GamemodeIcons = new string[5]
		{
			"icon",
			"icon_zombie",
			"icon_murder",
			"icon_potato",
			"icon"
		};
		presence = new DiscordRpc.RichPresence();
	}

	static DiscordManager()
	{
		epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
	}
}
