using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Assets.CoreScripts;
using Hazel;
using Hazel.Udp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InnerNet
{
	public abstract class InnerNetClient : MonoBehaviour
	{
		public enum GameStates
		{
			NotJoined,
			Joined,
			Started,
			Ended
		}

		private static readonly DisconnectReasons[] disconnectReasons;

		public const int NoClientId = -1;

		private string networkAddress = "127.0.0.1";

		private int networkPort;

		private UdpClientConnection connection;

		public MatchMakerModes mode;

		public int GameId = 32;

		public int HostId;

		public int ClientId = -1;

		public List<ClientData> allClients = new List<ClientData>();

		public DisconnectReasons LastDisconnectReason;

		public string LastCustomDisconnect;

		private readonly List<Action> DispatchQueue = new List<Action>();

		public GameStates GameState;

		private const int KicksPerGame = 0;

		private volatile bool appPaused;

		public const int CurrentClient = -3;

		public const int InvalidClient = -2;

		internal const byte DataFlag = 1;

		internal const byte RpcFlag = 2;

		internal const byte SpawnFlag = 4;

		internal const byte DespawnFlag = 5;

		internal const byte SceneChangeFlag = 6;

		internal const byte ReadyFlag = 7;

		internal const byte ChangeSettingsFlag = 8;

		public float MinSendInterval = 0.1f;

		private uint NetIdCnt = 1u;

		private float timer;

		public InnerNetObject[] SpawnableObjects;

		public List<InnerNetObject> allObjects = new List<InnerNetObject>();

		private Dictionary<uint, InnerNetObject> allObjectsFast = new Dictionary<uint, InnerNetObject>();

		private MessageWriter[] Streams;

		private bool AmConnected => connection != null;

		public int Ping
		{
			get
			{
				if (connection == null)
				{
					return 0;
				}
				return (int)connection.AveragePingMs;
			}
		}

		public bool AmHost => HostId == ClientId;

		public bool AmClient => ClientId > 0;

		public bool IsGamePublic
		{
			get;
			private set;
		}

		public bool IsGameStarted => GameState == GameStates.Started;

		public bool IsGameOver => GameState == GameStates.Ended;

		public int KicksLeft
		{
			get;
			private set;
		} = 1;


		public void SetEndpoint(string addr, ushort port)
		{
			networkAddress = addr;
			networkPort = port;
		}

		public virtual void Start()
		{
			SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene scene)
			{
				SendSceneChange(scene.name);
			};
			ClientId = -1;
			GameId = 32;
		}

		private void SendOrDisconnect(MessageWriter msg)
		{
			try
			{
				connection.Send(msg);
			}
			catch
			{
				EnqueueDisconnect(DisconnectReasons.Error, "Failed to send message");
			}
		}

		public ClientData GetHost()
		{
			for (int i = 0; i < allClients.Count; i++)
			{
				if (allClients[i].Id == HostId)
				{
					return allClients[i];
				}
			}
			return null;
		}

		public int GetClientIdFromCharacter(InnerNetObject character)
		{
			for (int i = 0; i < allClients.Count; i++)
			{
				if (allClients[i].Character == character)
				{
					return allClients[i].Id;
				}
			}
			return -1;
		}

		public virtual void OnDestroy()
		{
			if (AmConnected)
			{
				DisconnectInternal(DisconnectReasons.Destroy);
			}
		}

		public IEnumerator CoConnect()
		{
			if (AmConnected)
			{
				yield break;
			}
			LastDisconnectReason = DisconnectReasons.ExitGame;
			if (Streams == null)
			{
				Streams = new MessageWriter[2];
				for (int i = 0; i < Streams.Length; i++)
				{
					Streams[i] = MessageWriter.Get((SendOption)i);
				}
			}
			for (int j = 0; j < Streams.Length; j++)
			{
				MessageWriter obj = Streams[j];
				obj.Clear((SendOption)j);
				obj.StartMessage(5);
				obj.Write(GameId);
			}
			IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(networkAddress), networkPort);
			connection = new UdpClientConnection(remoteEndPoint);
			connection.KeepAliveInterval = 1500;
			connection.DisconnectTimeout = 7500;
			connection.ResendPingMultiplier = 1.3f;
			connection.DataReceived += OnMessageReceived;
			connection.Disconnected += OnDisconnect;
			connection.ConnectAsync(GetConnectionData());
			yield return WaitWithTimeout(() => connection == null || connection.State == ConnectionState.Connected);
		}

		private void Connection_DataReceivedRaw(byte[] data)
		{
			Debug.Log("Client Got: " + string.Join(" ", data.Select(delegate(byte b)
			{
				byte b2 = b;
				return b2.ToString();
			})));
		}

		private void Connection_DataSentRaw(byte[] data, int length)
		{
			Debug.Log("Client Sent: " + string.Join(" ", data.Select(delegate(byte b)
			{
				byte b2 = b;
				return b2.ToString();
			}).ToArray(), 0, length));
		}

		public void Connect(MatchMakerModes mode)
		{
			StartCoroutine(CoConnect(mode));
		}

		private IEnumerator CoConnect(MatchMakerModes mode)
		{
			if (this.mode != 0)
			{
				DisconnectInternal(DisconnectReasons.NewConnection);
			}
			this.mode = mode;
			yield return CoConnect();
			if (!AmConnected)
			{
				yield break;
			}
			switch (this.mode)
			{
			case MatchMakerModes.Client:
				JoinGame();
				yield return WaitWithTimeout(() => ClientId >= 0);
				_ = AmConnected;
				break;
			case MatchMakerModes.HostAndClient:
				GameId = 0;
				PlayerControl.GameOptions = SaveManager.GameHostOptions;
				HostGame(SaveManager.GameHostOptions);
				yield return WaitWithTimeout(() => GameId != 0);
				if (AmConnected)
				{
					JoinGame();
					yield return WaitWithTimeout(() => ClientId >= 0);
					_ = AmConnected;
				}
				break;
			}
		}

		public IEnumerator WaitForConnectionOrFail()
		{
			while (AmConnected)
			{
				switch (mode)
				{
				default:
					yield break;
				case MatchMakerModes.Client:
					if (ClientId >= 0)
					{
						yield break;
					}
					break;
				case MatchMakerModes.HostAndClient:
					if (GameId != 0 && ClientId >= 0)
					{
						yield break;
					}
					break;
				}
				yield return null;
			}
		}

		private IEnumerator WaitWithTimeout(Func<bool> success)
		{
			bool failed = true;
			for (float timer = 0f; timer < 5f; timer += Time.deltaTime)
			{
				if (success())
				{
					failed = false;
					break;
				}
				if (!AmConnected)
				{
					yield break;
				}
				yield return null;
			}
			if (failed)
			{
				DisconnectInternal(DisconnectReasons.Error);
			}
		}

		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
			{
				ResolutionManager.ToggleFullscreen();
			}
			if (DispatchQueue.Count <= 0)
			{
				return;
			}
			lock (DispatchQueue)
			{
				for (int i = 0; i < DispatchQueue.Count; i++)
				{
					try
					{
						DispatchQueue[i]();
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
						DispatchQueue.RemoveAt(i);
						i--;
					}
				}
				DispatchQueue.Clear();
			}
		}

		private void OnDisconnect(object sender, DisconnectedEventArgs e)
		{
			EnqueueDisconnect(DisconnectReasons.Error, e.Reason);
		}

		public void HandleDisconnect(DisconnectReasons reason, string stringReason = null)
		{
			if (reason != 0 && reason != DisconnectReasons.GameNotFound && reason != DisconnectReasons.GameFull && reason != DisconnectReasons.GameStarted)
			{
				SaveManager.LastGameStart = DateTime.MinValue;
			}
			StopAllCoroutines();
			DestroyableSingleton<Telemetry>.Instance.WriteDisconnect(LastDisconnectReason);
			DisconnectInternal(reason, stringReason);
			OnDisconnected();
		}

		protected void EnqueueDisconnect(DisconnectReasons reason, string stringReason = null)
		{
			lock (DispatchQueue)
			{
				_ = connection;
				DispatchQueue.Clear();
				DispatchQueue.Add(delegate
				{
					HandleDisconnect(reason, stringReason);
				});
			}
		}

		protected void DisconnectInternal(DisconnectReasons reason, string stringReason = null)
		{
			lock (DispatchQueue)
			{
				DispatchQueue.Clear();
			}
			NetIdCnt = 0u;
			allObjects.Clear();
			allClients.Clear();
			allObjectsFast.Clear();
			if (reason != DisconnectReasons.NewConnection && reason != DisconnectReasons.FocusLostBackground)
			{
				LastDisconnectReason = reason;
				if (reason != 0 && DestroyableSingleton<DisconnectPopup>.InstanceExists)
				{
					DestroyableSingleton<DisconnectPopup>.Instance.Show();
				}
			}
			if (mode == MatchMakerModes.HostAndClient)
			{
				GameId = 0;
			}
			if (mode == MatchMakerModes.Client || mode == MatchMakerModes.HostAndClient)
			{
				ClientId = -1;
			}
			mode = MatchMakerModes.None;
			GameState = GameStates.NotJoined;
			UdpClientConnection udpClientConnection = connection;
			connection = null;
			if (udpClientConnection != null)
			{
				try
				{
					udpClientConnection.Dispose();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			if (DestroyableSingleton<InnerNetServer>.InstanceExists)
			{
				DestroyableSingleton<InnerNetServer>.Instance.StopServer();
			}
		}

		public void HostGame(IBytesSerializable settings)
		{
			IsGamePublic = false;
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(0);
			messageWriter.WriteBytesAndSize(settings.ToBytes());
			messageWriter.EndMessage();
			SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
			Debug.Log("Client requesting new game.");
		}

		public void JoinGame()
		{
			ClientId = -1;
			if (!AmConnected)
			{
				HandleDisconnect(DisconnectReasons.Error);
				return;
			}
			Debug.Log("Client joining game: " + IntToGameName(GameId));
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(1);
			messageWriter.Write(GameId);
			messageWriter.EndMessage();
			SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		public bool CanKick()
		{
			if (AmHost)
			{
				return KicksLeft != 0;
			}
			return false;
		}

		public void KickPlayer(int clientId, bool ban)
		{
			if (CanKick())
			{
				if (KicksLeft > 0)
				{
					KicksLeft--;
				}
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.StartMessage(11);
				messageWriter.Write(GameId);
				messageWriter.WritePacked(clientId);
				messageWriter.Write(ban);
				messageWriter.EndMessage();
				SendOrDisconnect(messageWriter);
				messageWriter.Recycle();
			}
		}

        public MessageWriter StartEndGame()
        {
            MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
            messageWriter.StartMessage(8);
            messageWriter.Write(GameId);
            return messageWriter;
        }

		public MessageWriter StartCustomEndGame()
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(14);
			messageWriter.Write(GameId);
			return messageWriter;
		}

		public void FinishEndGame(MessageWriter msg)
		{
			msg.EndMessage();
			SendOrDisconnect(msg);
			msg.Recycle();
		}

		protected void SendLateRejection(int targetId, DisconnectReasons reason)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(4);
			messageWriter.Write(GameId);
			messageWriter.WritePacked(targetId);
			messageWriter.Write((byte)reason);
			messageWriter.EndMessage();
			SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		protected void SendClientReady()
		{
			if (AmHost)
			{
				FindClientById(ClientId).IsReady = true;
				return;
			}
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(5);
			messageWriter.Write(GameId);
			messageWriter.StartMessage(7);
			messageWriter.WritePacked(ClientId);
			messageWriter.EndMessage();
			messageWriter.EndMessage();
			SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		protected void SendStartGame()
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(2);
			messageWriter.Write(GameId);
			messageWriter.EndMessage();
			SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		public void RequestGameList(bool includePrivate, IBytesSerializable settings)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(9);
			messageWriter.Write(includePrivate);
			messageWriter.WriteBytesAndSize(settings.ToBytes());
			messageWriter.EndMessage();
			SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		public void ChangeGamePublic(bool isPublic)
		{
			if (AmHost)
			{
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.StartMessage(10);
				messageWriter.Write(GameId);
				messageWriter.Write(1);
				messageWriter.Write(isPublic);
				messageWriter.EndMessage();
				SendOrDisconnect(messageWriter);
				messageWriter.Recycle();
				IsGamePublic = isPublic;
			}
		}

		private void OnMessageReceived(DataReceivedEventArgs e)
		{
			MessageReader message = e.Message;
			try
			{
				while (message.Position < message.Length)
				{
					HandleMessage(message.ReadMessage());
				}
			}
			finally
			{
				message.Recycle();
			}
		}

		private void HandleMessage(MessageReader reader)
		{
			switch (reader.Tag)
			{
			case 0:
				GameId = reader.ReadInt32();
				Debug.Log("Client hosting game: " + IntToGameName(GameId));
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnGameCreated(IntToGameName(GameId));
					});
				}
				break;
			case 2:
				GameState = GameStates.Started;
				KicksLeft = 0;
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnStartGame();
					});
				}
				break;
			case 4:
			{
				int num9 = reader.ReadInt32();
				if (GameId != num9)
				{
					break;
				}
				int playerIdThatLeft = reader.ReadInt32();
				bool amHost2 = AmHost;
				HostId = reader.ReadInt32();
				DisconnectReasons reason3 = DisconnectReasons.ExitGame;
				if (reader.Position < reader.Length)
				{
					reason3 = (DisconnectReasons)reader.ReadByte();
				}
				RemovePlayer(playerIdThatLeft, reason3);
				if (!AmHost || amHost2)
				{
					break;
				}
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnBecomeHost();
					});
				}
				break;
			}
			case 9:
			{
				int totalGames = reader.ReadPackedInt32();
				List<GameListing> output = new List<GameListing>();
				while (reader.Position < reader.Length)
				{
					output.Add(new GameListing(reader.ReadInt32(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadPackedInt32(), reader.ReadString()));
				}
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnGetGameList(totalGames, output);
					});
				}
				break;
			}
			case 10:
			{
				int num3 = reader.ReadInt32();
				if (GameId == num3)
				{
					if (reader.ReadByte() == 1)
					{
						IsGamePublic = reader.ReadBoolean();
						Debug.Log("Alter Public = " + IsGamePublic);
					}
					else
					{
						Debug.Log("Alter unknown");
					}
				}
				break;
			}
			case 13:
			{
				uint address = reader.ReadUInt32();
				ushort port = reader.ReadUInt16();
				AmongUsClient.Instance.SetEndpoint(AddressToString(address), port);
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						Debug.Log($"Redirected to: {networkAddress}:{networkPort}");
						StopAllCoroutines();
						Connect(mode);
					});
				}
				break;
			}
			case 5:
			case 6:
			{
				int num4 = reader.ReadInt32();
				if (GameId != num4)
				{
					break;
				}
				if (reader.Tag == 6)
				{
					int num5 = reader.ReadPackedInt32();
					if (ClientId != num5)
					{
						Debug.LogWarning($"Got data meant for {num5}");
						break;
					}
				}
				MessageReader subReader = MessageReader.Get(reader);
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						HandleGameData(subReader);
					});
				}
				break;
			}
			case 11:
			{
				int num8 = reader.ReadInt32();
				if (GameId == num8 && reader.ReadPackedInt32() == ClientId)
				{
					EnqueueDisconnect(reader.ReadBoolean() ? DisconnectReasons.Banned : DisconnectReasons.Kicked);
				}
				break;
			}
			default:
				Debug.Log($"Bad tag {reader.Tag} at {reader.Offset}+{reader.Position}={reader.Length}:  " + string.Join(" ", reader.Buffer));
				break;
			case 3:
			{
				DisconnectReasons reason2 = DisconnectReasons.ServerRequest;
				if (reader.Position < reader.Length)
				{
					reason2 = (DisconnectReasons)reader.ReadByte();
				}
				EnqueueDisconnect(reason2);
				break;
			}
			case 8:
			{
				int num11 = reader.ReadInt32();
				if (GameId != num11 || GameState == GameStates.Ended)
				{
					break;
				}
				GameState = GameStates.Ended;
				lock (allClients)
				{
					allClients.Clear();
				}
				GameOverReason reason = (GameOverReason)reader.ReadByte();
				bool showAd = reader.ReadBoolean();
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnGameEnd(reason, showAd);
					});
				}
				break;
			}
			case 14:
				{
					int num11 = reader.ReadInt32();
					if (GameId != num11 || GameState == GameStates.Ended)
					{
						break;
					}
					GameState = GameStates.Ended;
					lock (allClients)
					{
						allClients.Clear();
					}
					int length = reader.ReadPackedInt32(); //Get length of winner array
					GameData.PlayerInfo[] plrs = new GameData.PlayerInfo[length]; //Create new blank array using length
					for (int i = 0; i < length; i++)
					{
						plrs[i] = GameData.Instance.GetPlayerById(reader.ReadByte()); //get all the player infos
					}
					string song = reader.ReadString();


					lock (DispatchQueue)
					{
						DispatchQueue.Add(delegate
						{
							OnGameEndCustom(plrs, song);
						});
					}
					break;
					}
				case 12:
			{
				int num10 = reader.ReadInt32();
				if (GameId != num10)
				{
					break;
				}
				ClientId = reader.ReadInt32();
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnWaitForHost(IntToGameName(GameId));
					});
				}
				break;
			}
			case 7:
			{
				int num6 = reader.ReadInt32();
				if (GameId != num6)
				{
					break;
				}
				ClientId = reader.ReadInt32();
				GameState = GameStates.Joined;
				KicksLeft = -1;
				ClientData myClient = GetOrCreateClient(ClientId);
				_ = AmHost;
				HostId = reader.ReadInt32();
				int num7 = reader.ReadPackedInt32();
				for (int i = 0; i < num7; i++)
				{
					GetOrCreateClient(reader.ReadPackedInt32());
				}
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnGameJoined(IntToGameName(GameId), myClient);
					});
				}
				break;
			}
			case 1:
			{
				int num = reader.ReadInt32();
				DisconnectReasons disconnectReasons = (DisconnectReasons)num;
				if (InnerNetClient.disconnectReasons.Contains(disconnectReasons))
				{
					if (disconnectReasons == DisconnectReasons.Custom)
					{
						LastCustomDisconnect = reader.ReadString();
					}
					GameId = -1;
					EnqueueDisconnect(disconnectReasons);
				}
				else if (GameId == num)
				{
					int num2 = reader.ReadInt32();
					bool amHost = AmHost;
					HostId = reader.ReadInt32();
					ClientData client = GetOrCreateClient(num2);
					Debug.Log($"Player {num2} joined");
					lock (DispatchQueue)
					{
						DispatchQueue.Add(delegate
						{
							OnPlayerJoined(client);
						});
						if (AmHost && !amHost)
						{
							DispatchQueue.Add(delegate
							{
								OnBecomeHost();
							});
						}
					}
				}
				else
				{
					EnqueueDisconnect(DisconnectReasons.IncorrectGame);
				}
				break;
			}
			}
		}

		private static string AddressToString(uint address)
		{
			return $"{(byte)address}.{(byte)(address >> 8)}.{(byte)(address >> 16)}.{(byte)(address >> 24)}";
		}

		private ClientData GetOrCreateClient(int clientId)
		{
			lock (allClients)
			{
				ClientData clientData = allClients.FirstOrDefault((ClientData c) => c.Id == clientId);
				if (clientData == null)
				{
					clientData = new ClientData(clientId);
					allClients.Add(clientData);
					return clientData;
				}
				return clientData;
			}
		}

		private void RemovePlayer(int playerIdThatLeft, DisconnectReasons reason)
		{
			ClientData client = null;
			lock (allClients)
			{
				int num = allClients.FindIndex((ClientData c) => c.Id == playerIdThatLeft);
				if (num != -1)
				{
					client = allClients[num];
					allClients.RemoveAt(num);
				}
			}
			if (client == null)
			{
				return;
			}
			lock (DispatchQueue)
			{
				DispatchQueue.Add(delegate
				{
					OnPlayerLeft(client, reason);
				});
			}
		}

		protected virtual void OnApplicationPause(bool pause)
		{
			appPaused = pause;
			if (!pause)
			{
				Debug.Log("Resumed Game");
				if (AmHost)
				{
					RemoveUnownedObjects();
				}
			}
			else if (GameState != GameStates.Ended && AmConnected)
			{
				Debug.Log("Lost focus during game");
				ThreadPool.QueueUserWorkItem(WaitToDisconnect);
			}
		}

		private void WaitToDisconnect(object state)
		{
			for (int i = 0; i < 10; i++)
			{
				if (!appPaused)
				{
					break;
				}
				Thread.Sleep(1000);
			}
			if (appPaused && GameState != GameStates.Ended && AmConnected)
			{
				DisconnectInternal(DisconnectReasons.FocusLostBackground);
				EnqueueDisconnect(DisconnectReasons.FocusLost);
			}
		}

		protected void SendInitialData(int clientId)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(6);
			messageWriter.Write(GameId);
			messageWriter.WritePacked(clientId);
			lock (allObjects)
			{
				HashSet<GameObject> hashSet = new HashSet<GameObject>();
				for (int i = 0; i < allObjects.Count; i++)
				{
					InnerNetObject innerNetObject = allObjects[i];
					if ((bool)innerNetObject && hashSet.Add(innerNetObject.gameObject))
					{
						WriteSpawnMessage(innerNetObject, innerNetObject.OwnerId, innerNetObject.SpawnFlags, messageWriter);
					}
				}
			}
			messageWriter.EndMessage();
			SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		protected abstract void OnGameCreated(string gameIdString);

		protected abstract void OnGameJoined(string gameIdString, ClientData client);

		protected abstract void OnWaitForHost(string gameIdString);

		protected abstract void OnStartGame();

        protected abstract void OnGameEnd(GameOverReason reason, bool showAd);

		protected abstract void OnGameEndCustom(GameData.PlayerInfo[] plyrs, string victorysong);

		protected abstract void OnBecomeHost();

		protected abstract void OnPlayerJoined(ClientData client);

		protected abstract void OnPlayerChangedScene(ClientData client, string targetScene);

		protected abstract void OnPlayerLeft(ClientData client, DisconnectReasons reason);

		protected abstract void OnDisconnected();

		protected abstract void OnGetGameList(int totalGames, List<GameListing> availableGames);

		protected abstract byte[] GetConnectionData();

		protected ClientData FindClientById(int id)
		{
			lock (allClients)
			{
				for (int i = 0; i < allClients.Count; i++)
				{
					if (allClients[i].Id == id)
					{
						return allClients[i];
					}
				}
				return null;
			}
		}

		public static string IntToGameName(int gameId)
		{
			char[] array = new char[4]
			{
				(char)((uint)gameId & 0xFFu),
				(char)((uint)(gameId >> 8) & 0xFFu),
				(char)((uint)(gameId >> 16) & 0xFFu),
				(char)((uint)(gameId >> 24) & 0xFFu)
			};
			if (array.Any((char c) => c < 'A' || c > 'z'))
			{
				return null;
			}
			return new string(array);
		}

		public static int GameNameToInt(string gameId)
		{
			if (gameId.Length != 4)
			{
				return -1;
			}
			gameId = gameId.ToUpperInvariant();
			return (int)(gameId[0] | ((uint)gameId[1] << 8) | ((uint)gameId[2] << 16) | ((uint)gameId[3] << 24));
		}

		private void FixedUpdate()
		{
			if (mode == MatchMakerModes.None || Streams == null)
			{
				timer = 0f;
				return;
			}
			timer += Time.fixedDeltaTime;
			if (timer < MinSendInterval)
			{
				return;
			}
			timer = 0f;
			lock (allObjects)
			{
				for (int i = 0; i < allObjects.Count; i++)
				{
					InnerNetObject innerNetObject = allObjects[i];
					if ((bool)innerNetObject && innerNetObject.DirtyBits != 0 && (innerNetObject.AmOwner || (innerNetObject.OwnerId == -2 && AmHost)))
					{
						MessageWriter messageWriter = Streams[(uint)innerNetObject.sendMode];
						messageWriter.StartMessage(1);
						messageWriter.WritePacked(innerNetObject.NetId);
						if (innerNetObject.Serialize(messageWriter, initialState: false))
						{
							messageWriter.EndMessage();
						}
						else
						{
							messageWriter.CancelMessage();
						}
					}
				}
			}
			for (int j = 0; j < Streams.Length; j++)
			{
				MessageWriter messageWriter2 = Streams[j];
				if (messageWriter2.HasBytes(7))
				{
					messageWriter2.EndMessage();
					SendOrDisconnect(messageWriter2);
					messageWriter2.Clear((SendOption)j);
					messageWriter2.StartMessage(5);
					messageWriter2.Write(GameId);
				}
			}
		}

		public T FindObjectByNetId<T>(uint netId) where T : InnerNetObject
		{
			if (allObjectsFast.TryGetValue(netId, out var value))
			{
				return (T)value;
			}
			Debug.LogWarning("Couldn't find target object: " + netId);
			return null;
		}

		public void SendRpcImmediately(uint targetNetId, byte callId, SendOption option)
		{
			MessageWriter messageWriter = MessageWriter.Get(option);
			messageWriter.StartMessage(5);
			messageWriter.Write(GameId);
			messageWriter.StartMessage(2);
			messageWriter.WritePacked(targetNetId);
			messageWriter.Write(callId);
			messageWriter.EndMessage();
			messageWriter.EndMessage();
			connection.Send(messageWriter);
			messageWriter.Recycle();
		}

		public MessageWriter StartRpcImmediately(uint targetNetId, byte callId, SendOption option, int targetClientId = -1)
		{
			MessageWriter messageWriter = MessageWriter.Get(option);
			if (targetClientId < 0)
			{
				messageWriter.StartMessage(5);
				messageWriter.Write(GameId);
			}
			else
			{
				messageWriter.StartMessage(6);
				messageWriter.Write(GameId);
				messageWriter.WritePacked(targetClientId);
			}
			messageWriter.StartMessage(2);
			messageWriter.WritePacked(targetNetId);
			messageWriter.Write(callId);
			return messageWriter;
		}

		public void FinishRpcImmediately(MessageWriter msg)
		{
			msg.EndMessage();
			msg.EndMessage();
			SendOrDisconnect(msg);
			msg.Recycle();
		}

		public void SendRpc(uint targetNetId, byte callId, SendOption option = SendOption.Reliable)
		{
			StartRpc(targetNetId, callId, option).EndMessage();
		}

		public MessageWriter StartRpc(uint targetNetId, byte callId, SendOption option = SendOption.Reliable)
		{
			MessageWriter obj = Streams[(uint)option];
			obj.StartMessage(2);
			obj.WritePacked(targetNetId);
			obj.Write(callId);
			return obj;
		}

		private void SendSceneChange(string sceneName)
		{
			if (AmConnected)
			{
				CE_UIHelpers.ForceLoadDebugUIs();
				StartCoroutine(CoSendSceneChange(sceneName));
			}
		}

		private IEnumerator CoSendSceneChange(string sceneName)
		{
			lock (allObjects)
			{
				for (int num = allObjects.Count - 1; num > -1; num--)
				{
					if (!allObjects[num])
					{
						allObjects.RemoveAt(num);
					}
				}
			}
			while (AmConnected && ClientId < 0)
			{
				yield return null;
			}
			if (!AmConnected)
			{
				yield break;
			}
			if (!AmHost && connection.State == ConnectionState.Connected)
			{
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.StartMessage(5);
				messageWriter.Write(GameId);
				messageWriter.StartMessage(6);
				messageWriter.WritePacked(ClientId);
				messageWriter.Write(sceneName);
				messageWriter.EndMessage();
				messageWriter.EndMessage();
				SendOrDisconnect(messageWriter);
				messageWriter.Recycle();
			}
			ClientData client = FindClientById(ClientId);
			if (client != null)
			{
				Debug.Log($"Changed scene: {ClientId} {sceneName}");
				lock (DispatchQueue)
				{
					DispatchQueue.Add(delegate
					{
						OnPlayerChangedScene(client, sceneName);
					});
				}
			}
			else
			{
				Debug.Log($"Couldn't find self in clients: {ClientId}.");
			}
		}

		public void Spawn(InnerNetObject netObjParent, int ownerId = -2, SpawnFlags flags = SpawnFlags.None)
		{
			if (AmHost)
			{
				ownerId = ((ownerId == -3) ? ClientId : ownerId);
				MessageWriter msg = Streams[1];
				WriteSpawnMessage(netObjParent, ownerId, flags, msg);
			}
			else if (AmClient)
			{
				Debug.LogError("Tried to spawn while not host:" + netObjParent);
			}
		}

		private void WriteSpawnMessage(InnerNetObject netObjParent, int ownerId, SpawnFlags flags, MessageWriter msg)
		{
			msg.StartMessage(4);
			msg.WritePacked(netObjParent.SpawnId);
			msg.WritePacked(ownerId);
			msg.Write((byte)flags);
			InnerNetObject[] componentsInChildren = netObjParent.GetComponentsInChildren<InnerNetObject>();
			msg.WritePacked(componentsInChildren.Length);
			InnerNetObject[] array = componentsInChildren;
			foreach (InnerNetObject innerNetObject in array)
			{
				innerNetObject.OwnerId = ownerId;
				innerNetObject.SpawnFlags = flags;
				if (innerNetObject.NetId == 0)
				{
					RegisterNetObject(innerNetObject);
				}
				msg.WritePacked(innerNetObject.NetId);
				msg.StartMessage(1);
				innerNetObject.Serialize(msg, initialState: true);
				msg.EndMessage();
			}
			msg.EndMessage();
		}

		public void Despawn(InnerNetObject objToDespawn)
		{
			if (objToDespawn.NetId < 1)
			{
				Debug.Log("Tried to net destroy: " + objToDespawn);
				return;
			}
			MessageWriter obj = Streams[1];
			obj.StartMessage(5);
			obj.WritePacked(objToDespawn.NetId);
			obj.EndMessage();
			RemoveNetObject(objToDespawn);
		}

		private void RegisterNetObject(InnerNetObject obj)
		{
			if (obj.NetId == 0)
			{
				obj.NetId = NetIdCnt++;
				allObjects.Add(obj);
				allObjectsFast.Add(obj.NetId, obj);
			}
			else
			{
				Debug.LogError("Attempted to double register: " + obj.name);
			}
		}

		private bool AddNetObject(InnerNetObject obj)
		{
			uint num = obj.NetId + 1;
			NetIdCnt = ((NetIdCnt > num) ? NetIdCnt : num);
			if (!allObjectsFast.ContainsKey(obj.NetId))
			{
				allObjects.Add(obj);
				allObjectsFast.Add(obj.NetId, obj);
				return true;
			}
			return false;
		}

		public void RemoveNetObject(InnerNetObject obj)
		{
			int num = allObjects.IndexOf(obj);
			if (num > -1)
			{
				allObjects.RemoveAt(num);
			}
			allObjectsFast.Remove(obj.NetId);
			obj.NetId = uint.MaxValue;
		}

		public void RemoveUnownedObjects()
		{
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(-2);
			lock (allClients)
			{
				for (int num = allClients.Count - 1; num >= 0; num--)
				{
					ClientData clientData = allClients[num];
					if ((bool)clientData.Character)
					{
						hashSet.Add(clientData.Id);
					}
				}
			}
			lock (allObjects)
			{
				for (int num2 = allObjects.Count - 1; num2 > -1; num2--)
				{
					InnerNetObject innerNetObject = allObjects[num2];
					if (!innerNetObject)
					{
						allObjects.RemoveAt(num2);
					}
					else if (!hashSet.Contains(innerNetObject.OwnerId))
					{
						innerNetObject.OwnerId = ClientId;
						UnityEngine.Object.Destroy(innerNetObject.gameObject);
					}
				}
			}
		}

		private void HandleGameData(MessageReader parentReader)
		{
			try
			{
				ClientData client;
				string targetScene;
				while (parentReader.Position < parentReader.Length)
				{
					MessageReader messageReader = parentReader.ReadMessage();
					switch (messageReader.Tag)
					{
					case 1:
					{
						uint key2 = messageReader.ReadPackedUInt32();
						if (allObjectsFast.TryGetValue(key2, out var value2))
						{
							value2.Deserialize(messageReader, initialState: false);
						}
						break;
					}
					case 2:
					{
						uint key = messageReader.ReadPackedUInt32();
						if (allObjectsFast.TryGetValue(key, out var value))
						{
							value.HandleRpc(messageReader.ReadByte(), messageReader);
						}
						break;
					}
					case 4:
					{
						uint num = messageReader.ReadPackedUInt32();
						if ((ulong)num >= (ulong)SpawnableObjects.Length)
						{
							Debug.LogError("Couldn't find spawnable prefab: " + num);
							break;
						}
						InnerNetObject innerNetObject = UnityEngine.Object.Instantiate(SpawnableObjects[num]);
						int num2 = messageReader.ReadPackedInt32();
						innerNetObject.SpawnFlags = (SpawnFlags)messageReader.ReadByte();
						int num3 = messageReader.ReadPackedInt32();
						InnerNetObject[] componentsInChildren = innerNetObject.GetComponentsInChildren<InnerNetObject>();
						if (num3 != componentsInChildren.Length)
						{
							Debug.LogError("Children didn't match for spawnable " + num);
							UnityEngine.Object.Destroy(innerNetObject.gameObject);
							break;
						}
						for (int i = 0; i < num3; i++)
						{
							InnerNetObject innerNetObject2 = componentsInChildren[i];
							innerNetObject2.NetId = messageReader.ReadPackedUInt32();
							innerNetObject2.OwnerId = num2;
							if (!AddNetObject(innerNetObject2))
							{
								innerNetObject.NetId = uint.MaxValue;
								UnityEngine.Object.Destroy(innerNetObject.gameObject);
								break;
							}
							MessageReader messageReader2 = messageReader.ReadMessage();
							if (messageReader2.Length > 0)
							{
								innerNetObject2.Deserialize(messageReader2, initialState: true);
							}
						}
						if ((innerNetObject.SpawnFlags & SpawnFlags.IsClientCharacter) != 0)
						{
							ClientData clientData2 = FindClientById(num2);
							if (clientData2 == null)
							{
								Debug.LogWarning("Spawn unowned character");
								UnityEngine.Object.Destroy(innerNetObject.gameObject);
							}
							else if (!clientData2.Character)
							{
								clientData2.InScene = true;
								clientData2.Character = innerNetObject as PlayerControl;
							}
							else
							{
								Debug.LogWarning("Double spawn character");
								UnityEngine.Object.Destroy(innerNetObject.gameObject);
							}
						}
						break;
					}
					case 5:
					{
						uint netId = messageReader.ReadPackedUInt32();
						InnerNetObject innerNetObject3 = FindObjectByNetId<InnerNetObject>(netId);
						if ((bool)innerNetObject3)
						{
							RemoveNetObject(innerNetObject3);
							UnityEngine.Object.Destroy(innerNetObject3.gameObject);
						}
						break;
					}
					case 6:
						client = FindClientById(messageReader.ReadPackedInt32());
						targetScene = messageReader.ReadString();
						Debug.Log($"Client {client?.Id ?? (-1)} changed scene to {targetScene}");
						if (client == null || string.IsNullOrWhiteSpace(targetScene))
						{
							break;
						}
						lock (DispatchQueue)
						{
							DispatchQueue.Add(delegate
							{
								OnPlayerChangedScene(client, targetScene);
							});
						}
						break;
					case 7:
					{
						ClientData clientData = FindClientById(messageReader.ReadPackedInt32());
						if (clientData != null)
						{
							Debug.Log($"Client {clientData.Id} ready");
							clientData.IsReady = true;
						}
						break;
					}
					default:
						Debug.Log($"Bad tag {messageReader.Tag} at {messageReader.Offset}+{messageReader.Position}={messageReader.Length}:  " + string.Join(" ", messageReader.Buffer));
						break;
					}
				}
			}
			finally
			{
				parentReader.Recycle();
			}
		}

		static InnerNetClient()
		{
			disconnectReasons = new DisconnectReasons[9]
			{
				DisconnectReasons.Error,
				DisconnectReasons.GameFull,
				DisconnectReasons.GameStarted,
				DisconnectReasons.GameNotFound,
				DisconnectReasons.IncorrectVersion,
				DisconnectReasons.Banned,
				DisconnectReasons.Kicked,
				DisconnectReasons.ServerFull,
				DisconnectReasons.Custom
			};
		}
	}
}
