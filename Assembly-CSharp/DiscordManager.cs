using System;
using System.Collections;
using InnerNet;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000077 RID: 119
public class DiscordManager : DestroyableSingleton<DiscordManager>
{
	// Token: 0x06000280 RID: 640 RVA: 0x0001424C File Offset: 0x0001244C
	public void Start()
	{
		if (DestroyableSingleton<DiscordManager>.Instance == this)
		{
			DiscordRpc.EventHandlers eventHandlers = default(DiscordRpc.EventHandlers);
			eventHandlers.errorCallback = (DiscordRpc.ErrorCallback)Delegate.Combine(eventHandlers.errorCallback, new DiscordRpc.ErrorCallback(this.HandleError));
			eventHandlers.disconnectedCallback = (DiscordRpc.DisconnectedCallback)Delegate.Combine(eventHandlers.disconnectedCallback, new DiscordRpc.DisconnectedCallback(this.HandleError));
			eventHandlers.joinCallback = (DiscordRpc.JoinCallback)Delegate.Combine(eventHandlers.joinCallback, new DiscordRpc.JoinCallback(this.HandleJoinRequest));
			eventHandlers.requestCallback = (DiscordRpc.RequestCallback)Delegate.Combine(eventHandlers.requestCallback, new DiscordRpc.RequestCallback(this.HandleAutoJoin));
			DiscordRpc.Initialize("780630509704708096", ref eventHandlers, true, null);
			this.SetInMenus();
			SceneManager.sceneLoaded += delegate(Scene scene, LoadSceneMode mode)
			{
				this.OnSceneChange(scene.name);
			};
		}
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00003935 File Offset: 0x00001B35
	private void HandleError(int errorCode, string message)
	{
		Debug.LogError(message ?? string.Format("No message: {0}", errorCode));
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00003951 File Offset: 0x00001B51
	private void OnSceneChange(string name)
	{
		if (name == "MatchMaking" || name == "MMOnline" || name == "MainMenu")
		{
			this.SetInMenus();
		}
	}

	// Token: 0x06000283 RID: 643 RVA: 0x00003980 File Offset: 0x00001B80
	public void FixedUpdate()
	{
		DiscordRpc.RunCallbacks();
	}

	// Token: 0x06000284 RID: 644 RVA: 0x00003987 File Offset: 0x00001B87
	public void SetInMenus()
	{
		this.ClearPresence();
		this.StartTime = null;
		this.presence.state = "In Menus";
		this.presence.largeImageKey = "icon_menu";
		DiscordRpc.UpdatePresence(this.presence);
	}

	// Token: 0x06000285 RID: 645 RVA: 0x00014324 File Offset: 0x00012524
	public void SetPlayingGame()
	{
		if (this.StartTime == null)
		{
			this.StartTime = new DateTime?(DateTime.UtcNow);
		}
		this.presence.state = "In Game";
		this.presence.details = "Playing " + this.Gamemodes[(int)PlayerControl.GameOptions.Gamemode];
		this.presence.largeImageKey = this.GamemodeIcons[(int)PlayerControl.GameOptions.Gamemode];
		this.presence.startTimestamp = DiscordManager.ToUnixTime(this.StartTime.Value);
		DiscordRpc.UpdatePresence(this.presence);
	}

	// Token: 0x06000286 RID: 646 RVA: 0x000039C6 File Offset: 0x00001BC6
	public void SetHowToPlay()
	{
		this.ClearPresence();
		this.presence.state = "In Freeplay";
		this.presence.largeImageKey = "icon_freeplay";
		DiscordRpc.UpdatePresence(this.presence);
	}

	// Token: 0x06000287 RID: 647 RVA: 0x000143C8 File Offset: 0x000125C8
	public void SetInLobbyClient()
	{
		if (this.StartTime == null)
		{
			this.StartTime = new DateTime?(DateTime.UtcNow);
		}
		this.ClearPresence();
		this.presence.state = "In Lobby";
		this.presence.largeImageKey = "icon_lobby";
		this.presence.startTimestamp = DiscordManager.ToUnixTime(this.StartTime.Value);
		DiscordRpc.UpdatePresence(this.presence);
	}

	// Token: 0x06000288 RID: 648 RVA: 0x00014440 File Offset: 0x00012640
	private void ClearPresence()
	{
		this.presence.startTimestamp = 0L;
		this.presence.details = null;
		this.presence.partyId = null;
		this.presence.matchSecret = null;
		this.presence.joinSecret = null;
		this.presence.partySize = 0;
		this.presence.partyMax = 0;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x000144A4 File Offset: 0x000126A4
	public void SetInLobbyHost(int numPlayers, int gameId)
	{
		if (this.StartTime == null)
		{
			this.StartTime = new DateTime?(DateTime.UtcNow);
		}
		string text = InnerNetClient.IntToGameName(gameId);
		this.presence.state = "In Lobby";
		this.presence.details = "Hosting a game";
		this.presence.partySize = numPlayers;
		this.presence.partyMax = 20;
		this.presence.smallImageKey = "icon_lobby";
		this.presence.largeImageText = "Ask to play!";
		this.presence.joinSecret = "join" + text;
		this.presence.matchSecret = "match" + text;
		this.presence.partyId = text;
		DiscordRpc.UpdatePresence(this.presence);
	}

	// Token: 0x0600028A RID: 650 RVA: 0x000039F9 File Offset: 0x00001BF9
	private void HandleAutoJoin(ref DiscordRpc.DiscordUser requestUser)
	{
		Debug.Log("Discord: request from " + requestUser.username);
		if (AmongUsClient.Instance.IsGameStarted)
		{
			this.RequestRespondNo();
			return;
		}
		this.RequestRespondYes();
	}

	// Token: 0x0600028B RID: 651 RVA: 0x00014574 File Offset: 0x00012774
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
		if (AmongUsClient.Instance.mode != MatchMakerModes.None)
		{
			Debug.LogWarning("Already connected");
			return;
		}
		AmongUsClient.Instance.GameMode = GameModes.OnlineGame;
		AmongUsClient.Instance.GameId = InnerNetClient.GameNameToInt(joinSecret.Substring(4));
		AmongUsClient.Instance.SetEndpoint(DestroyableSingleton<ServerManager>.Instance.OnlineNetAddress, 22023);
		AmongUsClient.Instance.MainMenuScene = "MMOnline";
		AmongUsClient.Instance.OnlineScene = "OnlineGame";
		DestroyableSingleton<DiscordManager>.Instance.StopAllCoroutines();
		DestroyableSingleton<DiscordManager>.Instance.StartCoroutine(DestroyableSingleton<DiscordManager>.Instance.CoJoinGame());
	}

	// Token: 0x0600028C RID: 652 RVA: 0x00003A29 File Offset: 0x00001C29
	public IEnumerator CoJoinGame()
	{
		while (DataCollectScreen.Instance && DataCollectScreen.Instance.isActiveAndEnabled)
		{
			yield return null;
		}
		AmongUsClient.Instance.Connect(MatchMakerModes.Client);
		yield return AmongUsClient.Instance.WaitForConnectionOrFail();
		if (AmongUsClient.Instance.ClientId < 0)
		{
			SceneManager.LoadScene("MMOnline");
		}
		yield break;
	}

	// Token: 0x0600028D RID: 653 RVA: 0x00003A31 File Offset: 0x00001C31
	public void RequestRespondYes()
	{
		DiscordRpc.Respond(this.joinRequest.userId, DiscordRpc.Reply.Yes);
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00003A44 File Offset: 0x00001C44
	public void RequestRespondNo()
	{
		Debug.Log("Discord: responding no to Ask to Join request");
		DiscordRpc.Respond(this.joinRequest.userId, DiscordRpc.Reply.No);
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00003A61 File Offset: 0x00001C61
	public override void OnDestroy()
	{
		base.OnDestroy();
		if (DestroyableSingleton<DiscordManager>.Instance == this)
		{
			DiscordRpc.Shutdown();
		}
	}

	// Token: 0x06000290 RID: 656 RVA: 0x00014658 File Offset: 0x00012858
	private static long ToUnixTime(DateTime time)
	{
		return (long)(time - DiscordManager.epoch).TotalSeconds;
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0001467C File Offset: 0x0001287C
	public DiscordManager()
	{
		this.presence = new DiscordRpc.RichPresence();
	}

	// Token: 0x0400027A RID: 634
	private DiscordRpc.RichPresence presence;

	// Token: 0x0400027B RID: 635
	public DiscordRpc.DiscordUser joinRequest;

	// Token: 0x0400027C RID: 636
	private DateTime? StartTime;

	// Token: 0x0400027D RID: 637
	private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	// Token: 0x0400027E RID: 638
	private readonly string[] Gamemodes = new string[]
	{
		"Classic",
		"Zombies",
		"Murder"
	};

	// Token: 0x0400027F RID: 639
	private readonly string[] GamemodeIcons = new string[]
	{
		"icon",
		"icon_zombie",
		"icon_murder"
	};
}
