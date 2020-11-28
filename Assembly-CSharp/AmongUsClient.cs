using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.CoreScripts;
using InnerNet;
using UnityEngine;
using UnityEngine.SceneManagement;

// Token: 0x02000131 RID: 305
public class AmongUsClient : InnerNetClient
{
	// Token: 0x06000661 RID: 1633 RVA: 0x00026490 File Offset: 0x00024690
	public void Awake()
	{
		if (AmongUsClient.Instance)
		{
			if (AmongUsClient.Instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject); //Test
			}
			return;
		}
		AmongUsClient.Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		QualitySettings.vSyncCount = 0;
		Application.targetFrameRate = 30;
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x000264E0 File Offset: 0x000246E0
	protected override byte[] GetConnectionData()
	{
		byte[] result;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				binaryWriter.Write(Constants.GetBroadcastVersion());
				binaryWriter.Write(SaveManager.PlayerName);
				binaryWriter.Flush();
				result = memoryStream.ToArray();
			}
		}
		return result;
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x00005FDD File Offset: 0x000041DD
	public void StartGame()
	{
		base.SendStartGame();
		this.discoverState = DiscoveryState.Off;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x00026550 File Offset: 0x00024750
	public void ExitGame(DisconnectReasons reason = DisconnectReasons.ExitGame)
	{
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Hide();
		}
		SoundManager.Instance.StopAllSound();
		this.discoverState = DiscoveryState.Off;
		this.DisconnectHandlers.Clear();
		base.DisconnectInternal(reason, null);
		SceneManager.LoadScene(this.MainMenuScene);
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x000265A0 File Offset: 0x000247A0
	protected override void OnGetGameList(int totalGames, List<GameListing> availableGames)
	{
		for (int i = 0; i < this.GameListHandlers.Count; i++)
		{
			try
			{
				this.GameListHandlers[i].HandleList(totalGames, availableGames);
			}
			catch
			{
			}
		}
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x00002265 File Offset: 0x00000465
	protected override void OnGameCreated(string gameIdString)
	{
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x00005FEC File Offset: 0x000041EC
	protected override void OnWaitForHost(string gameIdString)
	{
		Debug.Log("Waiting for host: " + gameIdString);
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Show();
		}
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0000600F File Offset: 0x0000420F
	protected override void OnStartGame()
	{
		Debug.Log("Received game start");
		base.StartCoroutine(this.CoStartGame());
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x00006028 File Offset: 0x00004228
	private IEnumerator CoStartGame()
	{
		PlayerControl.LocalPlayer.moveable = false;
		yield return null;
		CustomPlayerMenu customPlayerMenu = UnityEngine.Object.FindObjectOfType<CustomPlayerMenu>();
		if (customPlayerMenu)
		{
			customPlayerMenu.Close(false);
		}
		if (DestroyableSingleton<GameStartManager>.InstanceExists)
		{
			this.DisconnectHandlers.Remove(DestroyableSingleton<GameStartManager>.Instance);
			UnityEngine.Object.Destroy(DestroyableSingleton<GameStartManager>.Instance.gameObject);
		}
		if (DestroyableSingleton<DiscordManager>.InstanceExists)
		{
			DestroyableSingleton<DiscordManager>.Instance.SetPlayingGame();
		}
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 0.2f);
		while (!GameData.Instance)
		{
			yield return null;
		}
		while (!base.AmHost)
		{
			while (PlayerControl.LocalPlayer.Data == null && !base.AmHost)
			{
				yield return null;
			}
			if (!base.AmHost)
			{
				base.SendClientReady();
				while (!ShipStatus.Instance && !base.AmHost)
				{
					yield return null;
				}
				if (!base.AmHost)
				{
					IL_2D5();
					yield break;
				}
			}
		}
		base.SendClientReady();
		float timer = 0f;
		for (;;)
		{
			bool flag = false;
			List<ClientData> allClients = this.allClients;
			lock (allClients)
			{
				for (int j = 0; j < this.allClients.Count; j++)
				{
					ClientData clientData = this.allClients[j];
					if (!clientData.IsReady)
					{
						if (timer < 5f)
						{
							flag = true;
						}
						else
						{
							base.SendLateRejection(clientData.Id, DisconnectReasons.Error);
							clientData.IsReady = true;
							this.OnPlayerLeft(clientData, DisconnectReasons.ExitGame);
						}
					}
				}
			}
			if (!flag)
			{
				break;
			}
			yield return null;
			timer += Time.deltaTime;
		}
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		if (gameOptions.Validate(GameData.Instance.PlayerCount))
		{
			PlayerControl localPlayer = PlayerControl.LocalPlayer;
			if (localPlayer != null)
			{
				localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
			}
		}
		if (LobbyBehaviour.Instance)
		{
			LobbyBehaviour.Instance.Despawn();
		}
		if (!ShipStatus.Instance)
		{
			ShipStatus.Instance = UnityEngine.Object.Instantiate<ShipStatus>(this.ShipPrefabs[(int)gameOptions.MapId]);
		}
		base.Spawn(ShipStatus.Instance, -2, SpawnFlags.None);
		ShipStatus.Instance.SelectInfected();
		ShipStatus.Instance.Begin();
		IL_2D5();
		yield break;

		void IL_2D5()
        {
			for (int i = 0; i < GameData.Instance.PlayerCount; i++)
			{
				PlayerControl @object = GameData.Instance.AllPlayers[i].Object;
				if (@object)
				{
					@object.moveable = true;
					@object.NetTransform.enabled = true;
					@object.MyPhysics.enabled = true;
					@object.MyPhysics.Awake();
					@object.MyPhysics.ResetAnim(true);
					@object.Collider.enabled = true;
					Vector2 spawnLocation = ShipStatus.Instance.GetSpawnLocation(i, GameData.Instance.PlayerCount);
					@object.NetTransform.SnapTo(spawnLocation);
				}
			}
			SaveManager.LastGameStart = DateTime.UtcNow;
		}
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x000265EC File Offset: 0x000247EC
	protected override void OnBecomeHost()
	{
		ClientData clientData = base.FindClientById(this.ClientId);
		if (!clientData.Character)
		{
			this.OnGameJoined(null, clientData);
		}
		Debug.Log("Became Host");
		base.RemoveUnownedObjects();
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0002662C File Offset: 0x0002482C
	protected override void OnGameEnd(GameOverReason gameOverReason, bool showAd)
	{
		this.DisconnectHandlers.Clear();
		if (Minigame.Instance)
		{
			Minigame.Instance.Close();
			Minigame.Instance.Close();
		}
		try
		{
			if (SaveManager.SendTelemetry)
			{
				DestroyableSingleton<Telemetry>.Instance.EndGame(gameOverReason);
			}
		}
		catch
		{
		}
		TempData.EndReason = gameOverReason;
		TempData.showAd = showAd;
		bool flag = TempData.DidHumansWin(gameOverReason);
		TempData.winners = new List<WinningPlayerData>();
		for (int i = 0; i < GameData.Instance.PlayerCount; i++)
		{
			GameData.PlayerInfo playerInfo = GameData.Instance.AllPlayers[i];
			if (flag != playerInfo.IsImpostor)
			{
				TempData.winners.Add(new WinningPlayerData(playerInfo));
			}
		}
		base.StartCoroutine(this.CoEndGame());
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x00006037 File Offset: 0x00004237
	public IEnumerator CoEndGame()
	{
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 0.5f);
		SceneManager.LoadScene("EndGame");
		yield break;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x0000603F File Offset: 0x0000423F
	protected override void OnPlayerJoined(ClientData data)
	{
		if (base.AmHost && data.InScene && !data.Character)
		{
			this.CreatePlayer(data);
		}
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x00006065 File Offset: 0x00004265
	protected override void OnGameJoined(string gameIdString, ClientData data)
	{
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Hide();
		}
		if (!string.IsNullOrWhiteSpace(this.OnlineScene))
		{
			SceneManager.LoadScene(this.OnlineScene);
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x000266F8 File Offset: 0x000248F8
	protected override void OnPlayerLeft(ClientData data, DisconnectReasons reason)
	{
		if (data.Character)
		{
			PlayerControl character = data.Character;
			Debug.Log(string.Format("Player {0}({1}) left due to {2}: {3}", new object[]
			{
				data.Id,
				character.name,
				reason,
				data.IsReady
			}));
			for (int i = this.DisconnectHandlers.Count - 1; i > -1; i--)
			{
				try
				{
					this.DisconnectHandlers[i].HandleDisconnect(character, reason);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					this.DisconnectHandlers.RemoveAt(i);
				}
			}
			UnityEngine.Object.Destroy(data.Character.gameObject);
		}
		else
		{
			Debug.LogWarning("A player without a character disconnected");
			for (int j = this.DisconnectHandlers.Count - 1; j > -1; j--)
			{
				try
				{
					this.DisconnectHandlers[j].HandleDisconnect();
				}
				catch (Exception exception2)
				{
					Debug.LogException(exception2);
					this.DisconnectHandlers.RemoveAt(j);
				}
			}
		}
		if (base.AmHost)
		{
			GameOptionsData gameOptions = PlayerControl.GameOptions;
			if (gameOptions != null && gameOptions.isDefaults)
			{
				PlayerControl.GameOptions.SetRecommendations(GameData.Instance.PlayerCount, AmongUsClient.Instance.GameMode);
				PlayerControl localPlayer = PlayerControl.LocalPlayer;
				if (localPlayer == null)
				{
					return;
				}
				localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
			}
		}
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x00006090 File Offset: 0x00004290
	protected override void OnDisconnected()
	{
		SceneManager.LoadScene(this.MainMenuScene);
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00026864 File Offset: 0x00024A64
	protected override void OnPlayerChangedScene(ClientData client, string currentScene)
	{
		client.InScene = true;
		if (!base.AmHost)
		{
			return;
		}
		if (currentScene.Equals("Tutorial"))
		{
			GameData.Instance = UnityEngine.Object.Instantiate<GameData>(this.GameDataPrefab);
			base.Spawn(GameData.Instance, -2, SpawnFlags.None);
			ShipStatus netObjParent;
			if (TempData.IsDo2Enabled)
			{
				netObjParent = UnityEngine.Object.Instantiate<ShipStatus>(this.ShipPrefabs[1]);
			}
			else
			{
				netObjParent = UnityEngine.Object.Instantiate<ShipStatus>(this.ShipPrefabs[0]);
			}
			base.Spawn(netObjParent, -2, SpawnFlags.None);
			this.CreatePlayer(client);
			return;
		}
		if (currentScene.Equals("OnlineGame"))
		{
			if (client.Id != this.ClientId)
			{
				base.SendInitialData(client.Id);
			}
			else
			{
				if (this.GameMode == GameModes.LocalGame)
				{
					base.StartCoroutine(this.CoBroadcastManager());
				}
				if (!GameData.Instance)
				{
					GameData.Instance = UnityEngine.Object.Instantiate<GameData>(this.GameDataPrefab);
					base.Spawn(GameData.Instance, -2, SpawnFlags.None);
				}
			}
			if (!client.Character)
			{
				this.CreatePlayer(client);
			}
		}
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x00026968 File Offset: 0x00024B68
	[ContextMenu("Spawn Tester")]
	private void SpawnTester()
	{
		sbyte availableId = GameData.Instance.GetAvailableId();
		Vector2 v = Vector2.up.Rotate((float)availableId * (360f / (float)Palette.PlayerColors.Length)) * this.SpawnRadius;
		PlayerControl playerControl = UnityEngine.Object.Instantiate<PlayerControl>(this.PlayerPrefab, v, Quaternion.identity);
		playerControl.PlayerId = (byte)availableId;
		GameData.Instance.AddPlayer(playerControl);
		base.Spawn(playerControl, -2, SpawnFlags.None);
		playerControl.CmdCheckName("Test");
		playerControl.CmdCheckColor(0);
		if (DestroyableSingleton<HatManager>.InstanceExists)
		{
			playerControl.RpcSetHat((uint)((int)availableId % DestroyableSingleton<HatManager>.Instance.AllHats.Count));
			playerControl.RpcSetSkin((uint)((int)availableId % DestroyableSingleton<HatManager>.Instance.AllSkins.Count));
		}
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x00026A24 File Offset: 0x00024C24
	private void CreatePlayer(ClientData clientData)
	{
		if (!base.AmHost)
		{
			Debug.Log("Waiting for host to make my player");
			return;
		}
		sbyte availableId = GameData.Instance.GetAvailableId();
		if (availableId == -1)
		{
			base.SendLateRejection(clientData.Id, DisconnectReasons.GameNotFound);
			Debug.Log("Overfilled room.");
			return;
		}
		Vector2 v = Vector2.zero;
		if (ShipStatus.Instance)
		{
			v = ShipStatus.Instance.GetSpawnLocation((int)availableId, Palette.PlayerColors.Length);
		}
		else if (DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			v = new Vector2(-1.9f, 3.25f);
		}
		Debug.Log(string.Format("Spawned player {0} for client {1}", availableId, clientData.Id));
		PlayerControl playerControl = UnityEngine.Object.Instantiate<PlayerControl>(this.PlayerPrefab, v, Quaternion.identity);
		playerControl.PlayerId = (byte)availableId;
		clientData.Character = playerControl;
		base.Spawn(playerControl, clientData.Id, SpawnFlags.IsClientCharacter);
		GameData.Instance.AddPlayer(playerControl);
		if (PlayerControl.GameOptions.isDefaults)
		{
			PlayerControl.GameOptions.SetRecommendations(GameData.Instance.PlayerCount, AmongUsClient.Instance.GameMode);
		}
		playerControl.RpcSyncSettings(PlayerControl.GameOptions);
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0000609D File Offset: 0x0000429D
	private IEnumerator CoBroadcastManager()
	{
		while (!GameData.Instance)
		{
			yield return null;
		}
		int lastPlayerCount = 0;
		this.discoverState = DiscoveryState.Broadcast;
		while (this.discoverState == DiscoveryState.Broadcast)
		{
			if (lastPlayerCount != GameData.Instance.PlayerCount)
			{
				lastPlayerCount = GameData.Instance.PlayerCount;
				string data = string.Format("{0}~Open~{1}~", SaveManager.PlayerName, GameData.Instance.PlayerCount);
				DestroyableSingleton<InnerDiscover>.Instance.Interval = 1f;
				DestroyableSingleton<InnerDiscover>.Instance.StartAsServer(data);
			}
			yield return null;
		}
		DestroyableSingleton<InnerDiscover>.Instance.StopServer();
		yield break;
	}

	// Token: 0x04000641 RID: 1601
	public static AmongUsClient Instance;

	// Token: 0x04000642 RID: 1602
	public GameModes GameMode;

	// Token: 0x04000643 RID: 1603
	public string OnlineScene;

	// Token: 0x04000644 RID: 1604
	public string MainMenuScene;

	// Token: 0x04000645 RID: 1605
	public GameData GameDataPrefab;

	// Token: 0x04000646 RID: 1606
	public PlayerControl PlayerPrefab;

	// Token: 0x04000647 RID: 1607
	public List<ShipStatus> ShipPrefabs;

	// Token: 0x04000648 RID: 1608
	public float SpawnRadius = 1.75f;

	// Token: 0x04000649 RID: 1609
	public DiscoveryState discoverState;

	// Token: 0x0400064A RID: 1610
	public List<IDisconnectHandler> DisconnectHandlers = new List<IDisconnectHandler>();

	// Token: 0x0400064B RID: 1611
	public List<IGameListHandler> GameListHandlers = new List<IGameListHandler>();
}
