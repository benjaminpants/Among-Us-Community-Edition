using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.CoreScripts;
using InnerNet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AmongUsClient : InnerNetClient
{
	public static AmongUsClient Instance;

	public GameModes GameMode;

	public string OnlineScene;

	public string MainMenuScene;

	public GameData GameDataPrefab;

	public PlayerControl PlayerPrefab;

	public List<ShipStatus> ShipPrefabs;

	public float SpawnRadius = 1.75f;

	public DiscoveryState discoverState;

	public List<IDisconnectHandler> DisconnectHandlers = new List<IDisconnectHandler>();

	public List<IGameListHandler> GameListHandlers = new List<IGameListHandler>();

	public void Awake()
	{
		if ((bool)Instance)
		{
			if (Instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
		else
		{
			Instance = this;
			CE_UIHelpers.ForceLoadDebugUIs();
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 30;
		}
	}

	protected override byte[] GetConnectionData()
	{
		using MemoryStream memoryStream = new MemoryStream();
		using BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(Constants.GetBroadcastVersion());
		binaryWriter.Write(SaveManager.PlayerName);
		binaryWriter.Flush();
		return memoryStream.ToArray();
	}

	public void StartGame()
	{
		SendStartGame();
		discoverState = DiscoveryState.Off;
	}

	public void ExitGame(DisconnectReasons reason = DisconnectReasons.ExitGame)
	{
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Hide();
		}
		SoundManager.Instance.StopAllSound();
		discoverState = DiscoveryState.Off;
		DisconnectHandlers.Clear();
		DisconnectInternal(reason);
		SceneManager.LoadScene(MainMenuScene);
	}

	protected override void OnGetGameList(int totalGames, List<GameListing> availableGames)
	{
		for (int i = 0; i < GameListHandlers.Count; i++)
		{
			try
			{
				GameListHandlers[i].HandleList(totalGames, availableGames);
			}
			catch
			{
			}
		}
	}

	protected override void OnGameCreated(string gameIdString)
	{
	}

	protected override void OnWaitForHost(string gameIdString)
	{
		Debug.Log("Waiting for host: " + gameIdString);
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Show();
		}
	}

	protected override void OnStartGame()
	{
		Debug.Log("Received game start");
		StartCoroutine(CoStartGame());
	}

	private IEnumerator CoStartGame()
	{
		PlayerControl.LocalPlayer.moveable = false;
		yield return null;
		CE_UIHelpers.CollapseAll();
		CustomPlayerMenu customPlayerMenu = UnityEngine.Object.FindObjectOfType<CustomPlayerMenu>();
		if ((bool)customPlayerMenu)
		{
			customPlayerMenu.Close(canMove: false);
		}
		if (DestroyableSingleton<GameStartManager>.InstanceExists)
		{
			DisconnectHandlers.Remove(DestroyableSingleton<GameStartManager>.Instance);
			UnityEngine.Object.Destroy(DestroyableSingleton<GameStartManager>.Instance.gameObject);
		}
		if (DestroyableSingleton<DiscordManager>.InstanceExists)
		{
			DestroyableSingleton<DiscordManager>.Instance.SetPlayingGame();
		}
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black);
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
			if (base.AmHost)
			{
				continue;
			}
			SendClientReady();
			while (!ShipStatus.Instance && !base.AmHost)
			{
				yield return null;
			}
			if (base.AmHost)
			{
				continue;
			}
			for (int i = 0; i < GameData.Instance.PlayerCount; i++)
			{
				PlayerControl @object = GameData.Instance.AllPlayers[i].Object;
				if ((bool)@object)
				{
					@object.moveable = true;
					@object.NetTransform.enabled = true;
					@object.MyPhysics.enabled = true;
					@object.MyPhysics.Awake();
					@object.MyPhysics.ResetAnim();
					@object.Collider.enabled = true;
					Vector2 spawnLocation = ShipStatus.Instance.GetSpawnLocation(i, GameData.Instance.PlayerCount);
					@object.NetTransform.SnapTo(spawnLocation);
				}
			}
			SaveManager.LastGameStart = DateTime.UtcNow;
			yield break;
		}
		SendClientReady();
		float timer = 0f;
		while (true)
		{
			bool flag = false;
			lock (allClients)
			{
				for (int j = 0; j < allClients.Count; j++)
				{
					ClientData clientData = allClients[j];
					if (!clientData.IsReady)
					{
						if (timer < 5f)
						{
							flag = true;
							continue;
						}
						SendLateRejection(clientData.Id, DisconnectReasons.Error);
						clientData.IsReady = true;
						OnPlayerLeft(clientData, DisconnectReasons.ExitGame);
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
		if ((bool)LobbyBehaviour.Instance)
		{
			LobbyBehaviour.Instance.Despawn();
		}
		if (!ShipStatus.Instance)
		{
			ShipStatus.Instance = UnityEngine.Object.Instantiate(ShipPrefabs[gameOptions.MapId]);
			ShipStatus.Instance.transform.localScale = new Vector3(ShipStatus.Instance.transform.localScale.x * PlayerControl.GameOptions.MapScaleX, ShipStatus.Instance.transform.localScale.y * PlayerControl.GameOptions.MapScaleY, 1f);

			ShipStatus.Instance.transform.eulerAngles = new Vector3(0f, 0f, PlayerControl.GameOptions.MapRot);
		}
		Spawn(ShipStatus.Instance);
		ShipStatus.Instance.SelectInfected();
		ShipStatus.Instance.Begin();
		for (int k = 0; k < GameData.Instance.PlayerCount; k++)
		{
			PlayerControl object2 = GameData.Instance.AllPlayers[k].Object;
			if ((bool)object2)
			{
				object2.moveable = true;
				object2.NetTransform.enabled = true;
				object2.MyPhysics.enabled = true;
				object2.MyPhysics.Awake();
				object2.MyPhysics.ResetAnim();
				object2.Collider.enabled = true;
				Vector2 spawnLocation2 = ShipStatus.Instance.GetSpawnLocation(k, GameData.Instance.PlayerCount);
				object2.NetTransform.SnapTo(spawnLocation2);
			}
		}
		SaveManager.LastGameStart = DateTime.UtcNow;
	}

	protected override void OnBecomeHost()
	{
		ClientData clientData = FindClientById(ClientId);
		if (!clientData.Character)
		{
			OnGameJoined(null, clientData);
		}
		Debug.Log("Became Host");
		RemoveUnownedObjects();
	}

    protected override void OnGameEnd(GameOverReason gameOverReason, bool showAd)
    {
        DisconnectHandlers.Clear();
		if (CE_LuaLoader.CurrentGMLua)
		{
			CE_LuaLoader.GetGamemodeResult("OnGameEnd");
		}
		if ((bool)Minigame.Instance)
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
        for (int j = 0; j < GameData.Instance.PlayerCount; j++) //removed joker specific code
        {
			GameData.PlayerInfo playerInfo2 = GameData.Instance.AllPlayers[j];
			CE_WinWith win = CE_RoleManager.GetRoleFromID(playerInfo2.role).RoleWinWith;
			if (flag != ((playerInfo2.IsImpostor && win != CE_WinWith.Crewmates) || (win == CE_WinWith.Impostors)))
			{
				if (win != CE_WinWith.Neither)
				{
					TempData.winners.Add(new WinningPlayerData(playerInfo2));
				}
			}
        }
        StartCoroutine(CoEndGame());
    }

	protected override void OnGameEndCustom(GameData.PlayerInfo[] plyrs, string victorysong)
	{
        DisconnectHandlers.Clear();
		if (CE_LuaLoader.CurrentGMLua)
		{
			CE_LuaLoader.GetGamemodeResult("OnGameEnd");
		}
		if ((bool)Minigame.Instance)
		{
			Minigame.Instance.Close();
			Minigame.Instance.Close();
		}
        TempData.EndReason = GameOverReason.Custom;
		TempData.CustomStinger = victorysong;
		TempData.showAd = false;
		TempData.winners = new List<WinningPlayerData>();
        foreach (GameData.PlayerInfo plf in plyrs)
        {
            TempData.winners.Add(new WinningPlayerData(plf));
        }
		StartCoroutine(CoEndGame());
	}

	public IEnumerator CoEndGame()
	{
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 0.5f);
		SceneManager.LoadScene("EndGame");
	}

	protected override void OnPlayerJoined(ClientData data)
	{
		if (base.AmHost && data.InScene && !data.Character)
		{
			CreatePlayer(data);
		}
	}

	protected override void OnGameJoined(string gameIdString, ClientData data)
	{
		if (DestroyableSingleton<WaitForHostPopup>.InstanceExists)
		{
			DestroyableSingleton<WaitForHostPopup>.Instance.Hide();
		}
		if (!string.IsNullOrWhiteSpace(OnlineScene))
		{
			SceneManager.LoadScene(OnlineScene);
		}
	}

	protected override void OnPlayerLeft(ClientData data, DisconnectReasons reason)
	{
		if ((bool)data.Character)
		{
			PlayerControl character = data.Character;
			Debug.Log($"Player {data.Id}({character.name}) left due to {reason}: {data.IsReady}");
			for (int num = DisconnectHandlers.Count - 1; num > -1; num--)
			{
				try
				{
					DisconnectHandlers[num].HandleDisconnect(character, reason);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
					DisconnectHandlers.RemoveAt(num);
				}
			}
			UnityEngine.Object.Destroy(data.Character.gameObject);
		}
		else
		{
			Debug.LogWarning("A player without a character disconnected");
			for (int num2 = DisconnectHandlers.Count - 1; num2 > -1; num2--)
			{
				try
				{
					DisconnectHandlers[num2].HandleDisconnect();
				}
				catch (Exception exception2)
				{
					Debug.LogException(exception2);
					DisconnectHandlers.RemoveAt(num2);
				}
			}
		}
		if (!base.AmHost)
		{
			return;
		}
		GameOptionsData gameOptions = PlayerControl.GameOptions;
		if (gameOptions != null && gameOptions.isDefaults)
		{
			PlayerControl.GameOptions.SetRecommendations(GameData.Instance.PlayerCount, Instance.GameMode);
			PlayerControl localPlayer = PlayerControl.LocalPlayer;
			if (!(localPlayer == null))
			{
				localPlayer.RpcSyncSettings(PlayerControl.GameOptions);
			}
		}
	}

	protected override void OnDisconnected()
	{
		SceneManager.LoadScene(MainMenuScene);
	}

	protected override void OnPlayerChangedScene(ClientData client, string currentScene)
	{
		client.InScene = true;
		if (!base.AmHost)
		{
			return;
		}
		if (currentScene.Equals("Tutorial"))
		{
			GameData.Instance = UnityEngine.Object.Instantiate(GameDataPrefab);
			Spawn(GameData.Instance);
			ShipStatus netObjParent = ((!TempData.IsDo2Enabled) ? UnityEngine.Object.Instantiate(ShipPrefabs[0]) : UnityEngine.Object.Instantiate(ShipPrefabs[1]));
			Spawn(netObjParent);
			CreatePlayer(client);
		}
		else
		{
			if (!currentScene.Equals("OnlineGame"))
			{
				return;
			}
			if (client.Id != ClientId)
			{
				SendInitialData(client.Id);
			}
			else
			{
				if (GameMode == GameModes.LocalGame)
				{
					StartCoroutine(CoBroadcastManager());
				}
				if (!GameData.Instance)
				{
					GameData.Instance = UnityEngine.Object.Instantiate(GameDataPrefab);
					Spawn(GameData.Instance);
				}
			}
			if (!client.Character)
			{
				CreatePlayer(client);
			}
		}
	}

	[ContextMenu("Spawn Tester")]
	private void SpawnTester()
	{
		sbyte availableId = GameData.Instance.GetAvailableId();
		Vector2 v = Vector2.up.Rotate((float)availableId * (360f / (float)Palette.PlayerColors.Length)) * SpawnRadius;
		PlayerControl playerControl = UnityEngine.Object.Instantiate(PlayerPrefab, v, Quaternion.identity);
		playerControl.PlayerId = (byte)availableId;
		GameData.Instance.AddPlayer(playerControl);
		Spawn(playerControl);
		playerControl.CmdCheckName("Test");
		playerControl.CmdCheckColor(0);
		if (DestroyableSingleton<HatManager>.InstanceExists)
		{
			playerControl.RpcSetHat((uint)(availableId % DestroyableSingleton<HatManager>.Instance.AllHats.Count));
			playerControl.RpcSetSkin((uint)(availableId % DestroyableSingleton<HatManager>.Instance.AllSkins.Count));
		}
	}

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
			SendLateRejection(clientData.Id, DisconnectReasons.GameNotFound);
			Debug.Log("Overfilled room.");
			return;
		}
		Vector2 v = Vector2.zero;
		if ((bool)ShipStatus.Instance)
		{
			v = ShipStatus.Instance.GetSpawnLocation(availableId, Palette.PlayerColors.Length);
		}
		else if (DestroyableSingleton<TutorialManager>.InstanceExists)
		{
			v = new Vector2(-1.9f, 3.25f);
		}
		Debug.Log($"Spawned player {availableId} for client {clientData.Id}");
		PlayerControl playerControl = UnityEngine.Object.Instantiate(PlayerPrefab, v, Quaternion.identity);
		playerControl.PlayerId = (byte)availableId;
		clientData.Character = playerControl;
		Spawn(playerControl, clientData.Id, SpawnFlags.IsClientCharacter);
		GameData.Instance.AddPlayer(playerControl);
		if (PlayerControl.GameOptions.isDefaults)
		{
			PlayerControl.GameOptions.SetRecommendations(GameData.Instance.PlayerCount, Instance.GameMode);
		}
		playerControl.RpcSyncSettings(PlayerControl.GameOptions);
	}

	private IEnumerator CoBroadcastManager()
	{
		while (!GameData.Instance)
		{
			yield return null;
		}
		int lastPlayerCount = 0;
		discoverState = DiscoveryState.Broadcast;
		while (discoverState == DiscoveryState.Broadcast)
		{
			if (lastPlayerCount != GameData.Instance.PlayerCount)
			{
				lastPlayerCount = GameData.Instance.PlayerCount;
				string data = $"{SaveManager.PlayerName}~Open~{GameData.Instance.PlayerCount}~";
				DestroyableSingleton<InnerDiscover>.Instance.Interval = 1f;
				DestroyableSingleton<InnerDiscover>.Instance.StartAsServer(data);
			}
			yield return null;
		}
		DestroyableSingleton<InnerDiscover>.Instance.StopServer();
	}
}
