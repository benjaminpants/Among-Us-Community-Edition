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
			DiscordRpc.Initialize("908701325599060028", ref handlers, autoRegister: true, null);
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
		presence.largeImageKey = "icon";
		DiscordRpc.UpdatePresence(presence);
	}

	public void SetPlayingGame()
	{
		try
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
		catch(Exception E)
        {
			Debug.LogError(E.Message + E.StackTrace);
        }
	}

	public void SetHowToPlay()
	{
		try
		{
			ClearPresence();
			presence.state = "In Freeplay";
			presence.largeImageKey = "icon";
			DiscordRpc.UpdatePresence(presence);
		}
		catch (Exception E)
		{
			Debug.LogError(E.Message + E.StackTrace);
		}
	}

	public void SetInLobbyClient()
	{
		try
		{
			if (!StartTime.HasValue)
			{
				StartTime = DateTime.UtcNow;
			}
			ClearPresence();
			presence.state = "In Lobby";
			presence.largeImageKey = "icon";
			presence.startTimestamp = ToUnixTime(StartTime.Value);
			DiscordRpc.UpdatePresence(presence);
		}
		catch (Exception E)
		{
			Debug.LogError(E.Message + E.StackTrace);
		}

	}

	private void ClearPresence()
	{
		try
		{
			presence.startTimestamp = 0L;
			presence.details = null;
			presence.partyId = null;
			presence.matchSecret = null;
			presence.joinSecret = null;
			presence.partySize = 0;
			presence.partyMax = 0;
		}
		catch (Exception E)
		{
			Debug.LogError(E.Message + E.StackTrace);
		}
	}

	public void SetInLobbyHost(int numPlayers, int gameId)
	{
		try
		{
			if (!StartTime.HasValue)
			{
				StartTime = DateTime.UtcNow;
			}
			string text = InnerNetClient.IntToGameName(gameId);
			presence.state = "In Lobby";
			presence.details = "Hosting a game";
			presence.smallImageKey = "icon";
			DiscordRpc.UpdatePresence(presence);
		}
		catch (Exception E)
		{
			Debug.LogError(E.Message + E.StackTrace);
		}
	}

	private void HandleAutoJoin(ref DiscordRpc.DiscordUser requestUser)
	{
	}

	private void HandleJoinRequest(string joinSecret)
	{
		//no.
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
