using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Assets.CoreScripts;
using FaDe.Unity.Core;
using Hazel;
using Hazel.Udp;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace InnerNet
{
	// Token: 0x02000144 RID: 324
	public abstract class InnerNetClient : MonoBehaviour
	{
		// Token: 0x060007AE RID: 1966 RVA: 0x0002E381 File Offset: 0x0002C581
		public void SetEndpoint(string addr, ushort port)
		{
			this.networkAddress = addr;
			this.networkPort = (int)port;
		}

		// Token: 0x170000BE RID: 190
		// (get) Token: 0x060007AF RID: 1967 RVA: 0x0002E391 File Offset: 0x0002C591
		private bool AmConnected
		{
			get
			{
				return this.connection != null;
			}
		}

		// Token: 0x170000BF RID: 191
		// (get) Token: 0x060007B0 RID: 1968 RVA: 0x0002E39C File Offset: 0x0002C59C
		public int Ping
		{
			get
			{
				if (this.connection == null)
				{
					return 0;
				}
				return (int)this.connection.AveragePingMs;
			}
		}

		// Token: 0x170000C0 RID: 192
		// (get) Token: 0x060007B1 RID: 1969 RVA: 0x0002E3B4 File Offset: 0x0002C5B4
		public bool AmHost
		{
			get
			{
				return this.HostId == this.ClientId;
			}
		}

		// Token: 0x170000C1 RID: 193
		// (get) Token: 0x060007B2 RID: 1970 RVA: 0x0002E3C4 File Offset: 0x0002C5C4
		public bool AmClient
		{
			get
			{
				return this.ClientId > 0;
			}
		}

		// Token: 0x170000C2 RID: 194
		// (get) Token: 0x060007B3 RID: 1971 RVA: 0x0002E3CF File Offset: 0x0002C5CF
		// (set) Token: 0x060007B4 RID: 1972 RVA: 0x0002E3D7 File Offset: 0x0002C5D7
		public bool IsGamePublic { get; private set; }

		// Token: 0x170000C3 RID: 195
		// (get) Token: 0x060007B5 RID: 1973 RVA: 0x0002E3E0 File Offset: 0x0002C5E0
		public bool IsGameStarted
		{
			get
			{
				return this.GameState == InnerNetClient.GameStates.Started;
			}
		}

		// Token: 0x170000C4 RID: 196
		// (get) Token: 0x060007B6 RID: 1974 RVA: 0x0002E3EB File Offset: 0x0002C5EB
		public bool IsGameOver
		{
			get
			{
				return this.GameState == InnerNetClient.GameStates.Ended;
			}
		}

		// Token: 0x060007B7 RID: 1975 RVA: 0x0002E3F6 File Offset: 0x0002C5F6
		public virtual void Start()
		{
			SceneManager.activeSceneChanged += delegate (Scene oldScene, Scene scene)
			{
				this.SendSceneChange(scene.name);
			};
			this.ClientId = -1;
			UnityEngine.Object.Instantiate<GameObject>(new GameObject()).AddComponent<ProblemTraceConsole>();
			this.GameId = 32;
		}

		// Token: 0x060007B8 RID: 1976 RVA: 0x0002E428 File Offset: 0x0002C628
		private void SendOrDisconnect(MessageWriter msg)
		{
			try
			{
				this.connection.Send(msg);
			}
			catch
			{
				this.EnqueueDisconnect(DisconnectReasons.Error, "Failed to send message");
			}
		}

		// Token: 0x060007B9 RID: 1977 RVA: 0x0002E464 File Offset: 0x0002C664
		public ClientData GetHost()
		{
			for (int i = 0; i < this.allClients.Count; i++)
			{
				if (this.allClients[i].Id == this.HostId)
				{
					return this.allClients[i];
				}
			}
			return null;
		}

		// Token: 0x060007BA RID: 1978 RVA: 0x0002E4B0 File Offset: 0x0002C6B0
		public int GetClientIdFromCharacter(InnerNetObject character)
		{
			for (int i = 0; i < this.allClients.Count; i++)
			{
				if (this.allClients[i].Character == character)
				{
					return this.allClients[i].Id;
				}
			}
			return -1;
		}

		// Token: 0x060007BB RID: 1979 RVA: 0x0002E4FF File Offset: 0x0002C6FF
		public virtual void OnDestroy()
		{
			if (this.AmConnected)
			{
				this.DisconnectInternal(DisconnectReasons.Destroy, null);
			}
		}

		// Token: 0x060007BC RID: 1980 RVA: 0x0002E512 File Offset: 0x0002C712
		public IEnumerator CoConnect()
		{
			if (this.AmConnected)
			{
				yield break;
			}
			this.LastDisconnectReason = DisconnectReasons.ExitGame;
			if (this.Streams == null)
			{
				this.Streams = new MessageWriter[2];
				for (int i = 0; i < this.Streams.Length; i++)
				{
					this.Streams[i] = MessageWriter.Get((SendOption)i);
				}
			}
			for (int j = 0; j < this.Streams.Length; j++)
			{
				MessageWriter messageWriter = this.Streams[j];
				messageWriter.Clear((SendOption)j);
				messageWriter.StartMessage(5);
				messageWriter.Write(this.GameId);
			}
			IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse(this.networkAddress), this.networkPort);
			this.connection = new UdpClientConnection(remoteEndPoint, IPMode.IPv4);
			this.connection.KeepAliveInterval = 1500;
			this.connection.DisconnectTimeout = 7500;
			this.connection.ResendPingMultiplier = 1.3f;
			this.connection.DataReceived += this.OnMessageReceived;
			this.connection.Disconnected += this.OnDisconnect;
			this.connection.ConnectAsync(this.GetConnectionData(), 5000);
			yield return this.WaitWithTimeout(() => this.connection == null || this.connection.State == ConnectionState.Connected);
			yield break;
		}

		// Token: 0x060007BD RID: 1981 RVA: 0x0002E521 File Offset: 0x0002C721
		private void Connection_DataReceivedRaw(byte[] data)
		{
			Debug.Log("Client Got: " + string.Join(" ", data.Select(delegate (byte b)
			{
				byte b2 = b;
				return b2.ToString();
			})));
		}

		// Token: 0x060007BE RID: 1982 RVA: 0x0002E564 File Offset: 0x0002C764
		private void Connection_DataSentRaw(byte[] data, int length)
		{
			Debug.Log("Client Sent: " + string.Join(" ", data.Select(delegate (byte b)
			{
				byte b2 = b;
				return b2.ToString();
			}).ToArray<string>(), 0, length));
		}

		// Token: 0x060007BF RID: 1983 RVA: 0x0002E5B6 File Offset: 0x0002C7B6
		public void Connect(MatchMakerModes mode)
		{
			base.StartCoroutine(this.CoConnect(mode));
		}

		// Token: 0x060007C0 RID: 1984 RVA: 0x0002E5C6 File Offset: 0x0002C7C6
		private IEnumerator CoConnect(MatchMakerModes mode)
		{
			if (this.mode != MatchMakerModes.None)
			{
				this.DisconnectInternal(DisconnectReasons.NewConnection, null);
			}
			this.mode = mode;
			yield return this.CoConnect();
			if (!this.AmConnected)
			{
				yield break;
			}
			MatchMakerModes matchMakerModes = this.mode;
			if (matchMakerModes == MatchMakerModes.Client)
			{
				this.JoinGame();
				yield return this.WaitWithTimeout(() => this.ClientId >= 0);
				bool amConnected = this.AmConnected;
				yield break;
			}
			if (matchMakerModes != MatchMakerModes.HostAndClient)
			{
				yield break;
			}
			this.GameId = 0;
			PlayerControl.GameOptions = SaveManager.GameHostOptions;
			this.HostGame(SaveManager.GameHostOptions);
			yield return this.WaitWithTimeout(() => this.GameId != 0);
			if (!this.AmConnected)
			{
				yield break;
			}
			this.JoinGame();
			yield return this.WaitWithTimeout(() => this.ClientId >= 0);
			bool amConnected2 = this.AmConnected;
			yield break;
		}

		// Token: 0x060007C1 RID: 1985 RVA: 0x0002E5DC File Offset: 0x0002C7DC
		public IEnumerator WaitForConnectionOrFail()
		{
			while (this.AmConnected)
			{
				switch (this.mode)
				{
					case MatchMakerModes.None:
						goto IL_72;
					case MatchMakerModes.Client:
						if (this.ClientId >= 0)
						{
							yield break;
						}
						break;
					case MatchMakerModes.HostAndClient:
						if (this.GameId != 0 && this.ClientId >= 0)
						{
							yield break;
						}
						break;
					default:
						goto IL_72;
				}
				yield return null;
				continue;
				IL_72:
				yield break;
			}
			yield break;
		}

		// Token: 0x060007C2 RID: 1986 RVA: 0x0002E5EB File Offset: 0x0002C7EB
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
				if (!this.AmConnected)
				{
					yield break;
				}
				yield return null;
			}
			if (failed)
			{
				this.DisconnectInternal(DisconnectReasons.Error, null);
			}
			yield break;
		}

		// Token: 0x060007C3 RID: 1987 RVA: 0x0002E604 File Offset: 0x0002C804
		public void Update()
		{
			if (Input.GetKeyDown(KeyCode.Return) && (Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)))
			{
				ResolutionManager.ToggleFullscreen();
			}
			if (this.DispatchQueue.Count > 0)
			{
				List<Action> dispatchQueue = this.DispatchQueue;
				lock (dispatchQueue)
				{
					for (int i = 0; i < this.DispatchQueue.Count; i++)
					{
						try
						{
							this.DispatchQueue[i]();
						}
						catch (Exception exception)
						{
							Debug.LogException(exception);
							this.DispatchQueue.RemoveAt(i);
							i--;
						}
					}
					this.DispatchQueue.Clear();
				}
			}
		}

		// Token: 0x060007C4 RID: 1988 RVA: 0x0002E6CC File Offset: 0x0002C8CC
		private void OnDisconnect(object sender, DisconnectedEventArgs e)
		{
			this.EnqueueDisconnect(DisconnectReasons.Error, e.Reason);
		}

		// Token: 0x060007C5 RID: 1989 RVA: 0x0002E6DC File Offset: 0x0002C8DC
		public void HandleDisconnect(DisconnectReasons reason, string stringReason = null)
		{
			if (reason != DisconnectReasons.ExitGame && reason != DisconnectReasons.GameNotFound && reason != DisconnectReasons.GameFull && reason != DisconnectReasons.GameStarted)
			{
				SaveManager.LastGameStart = DateTime.MinValue;
			}
			base.StopAllCoroutines();
			DestroyableSingleton<Telemetry>.Instance.WriteDisconnect(this.LastDisconnectReason);
			this.DisconnectInternal(reason, stringReason);
			this.OnDisconnected();
		}

		// Token: 0x060007C6 RID: 1990 RVA: 0x0002E71C File Offset: 0x0002C91C
		protected void EnqueueDisconnect(DisconnectReasons reason, string stringReason = null)
		{
			List<Action> dispatchQueue = this.DispatchQueue;
			lock (dispatchQueue)
			{
				UdpClientConnection udpClientConnection = this.connection;
				this.DispatchQueue.Clear();
				this.DispatchQueue.Add(delegate
				{
					this.HandleDisconnect(reason, stringReason);
				});
			}
		}

		// Token: 0x060007C7 RID: 1991 RVA: 0x0002E79C File Offset: 0x0002C99C
		protected void DisconnectInternal(DisconnectReasons reason, string stringReason = null)
		{
			List<Action> dispatchQueue = this.DispatchQueue;
			lock (dispatchQueue)
			{
				this.DispatchQueue.Clear();
			}
			this.NetIdCnt = 0U;
			this.allObjects.Clear();
			this.allClients.Clear();
			this.allObjectsFast.Clear();
			if (reason != DisconnectReasons.NewConnection && reason != DisconnectReasons.FocusLostBackground)
			{
				this.LastDisconnectReason = reason;
				if (reason != DisconnectReasons.ExitGame && DestroyableSingleton<DisconnectPopup>.InstanceExists)
				{
					DestroyableSingleton<DisconnectPopup>.Instance.Show();
				}
			}
			if (this.mode == MatchMakerModes.HostAndClient)
			{
				this.GameId = 0;
			}
			if (this.mode == MatchMakerModes.Client || this.mode == MatchMakerModes.HostAndClient)
			{
				this.ClientId = -1;
			}
			this.mode = MatchMakerModes.None;
			this.GameState = InnerNetClient.GameStates.NotJoined;
			UdpClientConnection udpClientConnection = this.connection;
			this.connection = null;
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

		// Token: 0x060007C8 RID: 1992 RVA: 0x0002E8A8 File Offset: 0x0002CAA8
		public void HostGame(IBytesSerializable settings)
		{
			this.IsGamePublic = false;
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(0);
			messageWriter.WriteBytesAndSize(settings.ToBytes());
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
			Debug.Log("Client requesting new game.");
		}

		// Token: 0x060007C9 RID: 1993 RVA: 0x0002E8F4 File Offset: 0x0002CAF4
		public void JoinGame()
		{
			this.ClientId = -1;
			if (!this.AmConnected)
			{
				this.HandleDisconnect(DisconnectReasons.Error, null);
				return;
			}
			Debug.Log("Client joining game: " + InnerNetClient.IntToGameName(this.GameId));
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(1);
			messageWriter.Write(this.GameId);
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x170000C5 RID: 197
		// (get) Token: 0x060007CA RID: 1994 RVA: 0x0002E961 File Offset: 0x0002CB61
		// (set) Token: 0x060007CB RID: 1995 RVA: 0x0002E969 File Offset: 0x0002CB69
		public int KicksLeft { get; private set; } = 1;

		// Token: 0x060007CC RID: 1996 RVA: 0x0002E972 File Offset: 0x0002CB72
		public bool CanKick()
		{
			return this.AmHost && this.KicksLeft != 0;
		}

		// Token: 0x060007CD RID: 1997 RVA: 0x0002E988 File Offset: 0x0002CB88
		public void KickPlayer(int clientId, bool ban)
		{
			if (!this.CanKick())
			{
				return;
			}
			if (this.KicksLeft > 0)
			{
				int kicksLeft = this.KicksLeft;
				this.KicksLeft = kicksLeft - 1;
			}
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(11);
			messageWriter.Write(this.GameId);
			messageWriter.WritePacked(clientId);
			messageWriter.Write(ban);
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007CE RID: 1998 RVA: 0x0002E9F3 File Offset: 0x0002CBF3
		public MessageWriter StartEndGame()
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(8);
			messageWriter.Write(this.GameId);
			return messageWriter;
		}

		// Token: 0x060007CF RID: 1999 RVA: 0x0002EA0E File Offset: 0x0002CC0E
		public void FinishEndGame(MessageWriter msg)
		{
			msg.EndMessage();
			this.SendOrDisconnect(msg);
			msg.Recycle();
		}

		// Token: 0x060007D0 RID: 2000 RVA: 0x0002EA24 File Offset: 0x0002CC24
		protected void SendLateRejection(int targetId, DisconnectReasons reason)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(4);
			messageWriter.Write(this.GameId);
			messageWriter.WritePacked(targetId);
			messageWriter.Write((byte)reason);
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0002EA70 File Offset: 0x0002CC70
		protected void SendClientReady()
		{
			if (this.AmHost)
			{
				this.FindClientById(this.ClientId).IsReady = true;
				return;
			}
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(5);
			messageWriter.Write(this.GameId);
			messageWriter.StartMessage(7);
			messageWriter.WritePacked(this.ClientId);
			messageWriter.EndMessage();
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0002EAE0 File Offset: 0x0002CCE0
		protected void SendStartGame()
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(2);
			messageWriter.Write(this.GameId);
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007D3 RID: 2003 RVA: 0x0002EB1C File Offset: 0x0002CD1C
		public void RequestGameList(bool includePrivate, IBytesSerializable settings)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(9);
			messageWriter.Write(includePrivate);
			messageWriter.WriteBytesAndSize(settings.ToBytes());
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007D4 RID: 2004 RVA: 0x0002EB60 File Offset: 0x0002CD60
		public void ChangeGamePublic(bool isPublic)
		{
			if (this.AmHost)
			{
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.StartMessage(10);
				messageWriter.Write(this.GameId);
				messageWriter.Write(1);
				messageWriter.Write(isPublic);
				messageWriter.EndMessage();
				this.SendOrDisconnect(messageWriter);
				messageWriter.Recycle();
				this.IsGamePublic = isPublic;
			}
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x0002EBB8 File Offset: 0x0002CDB8
		private void OnMessageReceived(DataReceivedEventArgs e)
		{
			MessageReader message = e.Message;
			try
			{
				while (message.Position < message.Length)
				{
					this.HandleMessage(message.ReadMessage());
				}
			}
			finally
			{
				message.Recycle();
			}
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0002EC04 File Offset: 0x0002CE04
		private void HandleMessage(MessageReader reader)
		{
			List<Action> dispatchQueue;
			switch (reader.Tag)
			{
				case 0:
					this.GameId = reader.ReadInt32();
					Debug.Log("Client hosting game: " + InnerNetClient.IntToGameName(this.GameId));
					dispatchQueue = this.DispatchQueue;
					lock (dispatchQueue)
					{
						this.DispatchQueue.Add(delegate
						{
							this.OnGameCreated(InnerNetClient.IntToGameName(this.GameId));
						});
						return;
					}
					break;
				case 1:
					goto IL_473;
				case 2:
					break;
				case 3:
					goto IL_105;
				case 4:
					goto IL_229;
				case 5:
				case 6:
					{
						int num = reader.ReadInt32();
						if (this.GameId == num)
						{
							if (reader.Tag == 6)
							{
								int num2 = reader.ReadPackedInt32();
								if (this.ClientId != num2)
								{
									Debug.LogWarning(string.Format("Got data meant for {0}", num2));
									return;
								}
							}
							MessageReader subReader = MessageReader.Get(reader);
							dispatchQueue = this.DispatchQueue;
							lock (dispatchQueue)
							{
								this.DispatchQueue.Add(delegate
								{
									this.HandleGameData(subReader);
								});
								return;
							}
						}
						return;
					}
				case 7:
					goto IL_3BF;
				case 8:
					goto IL_2C0;
				case 9:
					{
						int totalGames = reader.ReadPackedInt32();
						List<GameListing> output = new List<GameListing>();
						while (reader.Position < reader.Length)
						{
							output.Add(new GameListing(reader.ReadInt32(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadPackedInt32(), reader.ReadString()));
						}
						dispatchQueue = this.DispatchQueue;
						lock (dispatchQueue)
						{
							this.DispatchQueue.Add(delegate
							{
								this.OnGetGameList(totalGames, output);
							});
							return;
						}
						goto IL_613;
					}
				case 10:
					goto IL_613;
				case 11:
					{
						int num3 = reader.ReadInt32();
						if (this.GameId == num3 && reader.ReadPackedInt32() == this.ClientId)
						{
							this.EnqueueDisconnect(reader.ReadBoolean() ? DisconnectReasons.Banned : DisconnectReasons.Kicked, null);
							return;
						}
						return;
					}
				case 12:
					goto IL_364;
				case 13:
					{
						uint address = reader.ReadUInt32();
						ushort port = reader.ReadUInt16();
						AmongUsClient.Instance.SetEndpoint(InnerNetClient.AddressToString(address), port);
						dispatchQueue = this.DispatchQueue;
						lock (dispatchQueue)
						{
							this.DispatchQueue.Add(delegate
							{
								Debug.Log(string.Format("Redirected to: {0}:{1}", this.networkAddress, this.networkPort));
								this.StopAllCoroutines();
								this.Connect(this.mode);
							});
							return;
						}
						goto IL_229;
					}
				default:
					Debug.Log(string.Format("Bad tag {0} at {1}+{2}={3}:  ", new object[]
					{
					reader.Tag,
					reader.Offset,
					reader.Position,
					reader.Length
					}) + string.Join<byte>(" ", reader.Buffer));
					return;
			}
			this.GameState = InnerNetClient.GameStates.Started;
			this.KicksLeft = 0;
			dispatchQueue = this.DispatchQueue;
			lock (dispatchQueue)
			{
				this.DispatchQueue.Add(delegate
				{
					this.OnStartGame();
				});
				return;
			}
			IL_105:
			DisconnectReasons reason3 = DisconnectReasons.ServerRequest;
			if (reader.Position < reader.Length)
			{
				reason3 = (DisconnectReasons)reader.ReadByte();
			}
			this.EnqueueDisconnect(reason3, null);
			return;
			IL_229:
			int num4 = reader.ReadInt32();
			if (this.GameId != num4)
			{
				return;
			}
			int playerIdThatLeft = reader.ReadInt32();
			bool amHost = this.AmHost;
			this.HostId = reader.ReadInt32();
			DisconnectReasons reason4 = DisconnectReasons.ExitGame;
			if (reader.Position < reader.Length)
			{
				reason4 = (DisconnectReasons)reader.ReadByte();
			}
			this.RemovePlayer(playerIdThatLeft, reason4);
			if (!this.AmHost || amHost)
			{
				return;
			}
			dispatchQueue = this.DispatchQueue;
			lock (dispatchQueue)
			{
				this.DispatchQueue.Add(delegate
				{
					this.OnBecomeHost();
				});
				return;
			}
			IL_2C0:
			int num5 = reader.ReadInt32();
			if (this.GameId != num5 || this.GameState == InnerNetClient.GameStates.Ended)
			{
				return;
			}
			this.GameState = InnerNetClient.GameStates.Ended;
			List<ClientData> obj = this.allClients;
			lock (obj)
			{
				this.allClients.Clear();
			}
			GameOverReason reason = (GameOverReason)reader.ReadByte();
			bool showAd = reader.ReadBoolean();
			dispatchQueue = this.DispatchQueue;
			lock (dispatchQueue)
			{
				this.DispatchQueue.Add(delegate
				{
					this.OnGameEnd(reason, showAd);
				});
				return;
			}
			IL_364:
			int num6 = reader.ReadInt32();
			if (this.GameId != num6)
			{
				return;
			}
			this.ClientId = reader.ReadInt32();
			dispatchQueue = this.DispatchQueue;
			lock (dispatchQueue)
			{
				this.DispatchQueue.Add(delegate
				{
					this.OnWaitForHost(InnerNetClient.IntToGameName(this.GameId));
				});
				return;
			}
			IL_3BF:
			int num7 = reader.ReadInt32();
			if (this.GameId != num7)
			{
				return;
			}
			this.ClientId = reader.ReadInt32();
			this.GameState = InnerNetClient.GameStates.Joined;
			this.KicksLeft = -1;
			ClientData myClient = this.GetOrCreateClient(this.ClientId);
			bool amHost3 = this.AmHost;
			this.HostId = reader.ReadInt32();
			int num8 = reader.ReadPackedInt32();
			for (int i = 0; i < num8; i++)
			{
				this.GetOrCreateClient(reader.ReadPackedInt32());
			}
			dispatchQueue = this.DispatchQueue;
			lock (dispatchQueue)
			{
				this.DispatchQueue.Add(delegate
				{
					this.OnGameJoined(InnerNetClient.IntToGameName(this.GameId), myClient);
				});
				return;
			}
			IL_473:
			int num9 = reader.ReadInt32();
			DisconnectReasons disconnectReasons = (DisconnectReasons)num9;
			if (InnerNetClient.disconnectReasons.Contains(disconnectReasons))
			{
				if (disconnectReasons == DisconnectReasons.Custom)
				{
					this.LastCustomDisconnect = reader.ReadString();
				}
				this.GameId = -1;
				this.EnqueueDisconnect(disconnectReasons, null);
				return;
			}
			if (this.GameId == num9)
			{
				int num10 = reader.ReadInt32();
				bool amHost2 = this.AmHost;
				this.HostId = reader.ReadInt32();
				ClientData client = this.GetOrCreateClient(num10);
				Debug.Log(string.Format("Player {0} joined", num10));
				dispatchQueue = this.DispatchQueue;
				lock (dispatchQueue)
				{
					this.DispatchQueue.Add(delegate
					{
						this.OnPlayerJoined(client);
					});
					if (this.AmHost && !amHost2)
					{
						this.DispatchQueue.Add(delegate
						{
							this.OnBecomeHost();
						});
					}
					return;
				}
			}
			this.EnqueueDisconnect(DisconnectReasons.IncorrectGame, null);
			return;
			IL_613:
			int num11 = reader.ReadInt32();
			if (this.GameId != num11)
			{
				return;
			}
			if (reader.ReadByte() == 1)
			{
				this.IsGamePublic = reader.ReadBoolean();
				Debug.Log("Alter Public = " + this.IsGamePublic.ToString());
				return;
			}
			Debug.Log("Alter unknown");
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0002F398 File Offset: 0x0002D598
		private static string AddressToString(uint address)
		{
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				(byte)address,
				(byte)(address >> 8),
				(byte)(address >> 16),
				(byte)(address >> 24)
			});
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0002F3E8 File Offset: 0x0002D5E8
		private ClientData GetOrCreateClient(int clientId)
		{
			List<ClientData> obj = this.allClients;
			ClientData clientData;
			lock (obj)
			{
				clientData = this.allClients.FirstOrDefault((ClientData c) => c.Id == clientId);
				if (clientData == null)
				{
					clientData = new ClientData(clientId);
					this.allClients.Add(clientData);
				}
			}
			return clientData;
		}

		// Token: 0x060007D9 RID: 2009 RVA: 0x0002F464 File Offset: 0x0002D664
		private void RemovePlayer(int playerIdThatLeft, DisconnectReasons reason)
		{
			ClientData client = null;
			List<ClientData> obj = this.allClients;
			lock (obj)
			{
				int num = this.allClients.FindIndex((ClientData c) => c.Id == playerIdThatLeft);
				if (num != -1)
				{
					client = this.allClients[num];
					this.allClients.RemoveAt(num);
				}
			}
			if (client != null)
			{
				List<Action> dispatchQueue = this.DispatchQueue;
				lock (dispatchQueue)
				{
					this.DispatchQueue.Add(delegate
					{
						this.OnPlayerLeft(client, reason);
					});
				}
			}
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0002F548 File Offset: 0x0002D748
		protected virtual void OnApplicationPause(bool pause)
		{
			this.appPaused = pause;
			if (!pause)
			{
				Debug.Log("Resumed Game");
				if (this.AmHost)
				{
					this.RemoveUnownedObjects();
					return;
				}
			}
			else if (this.GameState != InnerNetClient.GameStates.Ended && this.AmConnected)
			{
				Debug.Log("Lost focus during game");
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.WaitToDisconnect));
			}
		}

		// Token: 0x060007DB RID: 2011 RVA: 0x0002F5A8 File Offset: 0x0002D7A8
		private void WaitToDisconnect(object state)
		{
			int num = 0;
			while (num < 10 && this.appPaused)
			{
				Thread.Sleep(1000);
				num++;
			}
			if (this.appPaused && this.GameState != InnerNetClient.GameStates.Ended && this.AmConnected)
			{
				this.DisconnectInternal(DisconnectReasons.FocusLostBackground, null);
				this.EnqueueDisconnect(DisconnectReasons.FocusLost, null);
			}
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x0002F60C File Offset: 0x0002D80C
		protected void SendInitialData(int clientId)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(6);
			messageWriter.Write(this.GameId);
			messageWriter.WritePacked(clientId);
			List<InnerNetObject> obj = this.allObjects;
			lock (obj)
			{
				HashSet<GameObject> hashSet = new HashSet<GameObject>();
				for (int i = 0; i < this.allObjects.Count; i++)
				{
					InnerNetObject innerNetObject = this.allObjects[i];
					if (innerNetObject && hashSet.Add(innerNetObject.gameObject))
					{
						this.WriteSpawnMessage(innerNetObject, innerNetObject.OwnerId, innerNetObject.SpawnFlags, messageWriter);
					}
				}
			}
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007DD RID: 2013
		protected abstract void OnGameCreated(string gameIdString);

		// Token: 0x060007DE RID: 2014
		protected abstract void OnGameJoined(string gameIdString, ClientData client);

		// Token: 0x060007DF RID: 2015
		protected abstract void OnWaitForHost(string gameIdString);

		// Token: 0x060007E0 RID: 2016
		protected abstract void OnStartGame();

		// Token: 0x060007E1 RID: 2017
		protected abstract void OnGameEnd(GameOverReason reason, bool showAd);

		// Token: 0x060007E2 RID: 2018
		protected abstract void OnBecomeHost();

		// Token: 0x060007E3 RID: 2019
		protected abstract void OnPlayerJoined(ClientData client);

		// Token: 0x060007E4 RID: 2020
		protected abstract void OnPlayerChangedScene(ClientData client, string targetScene);

		// Token: 0x060007E5 RID: 2021
		protected abstract void OnPlayerLeft(ClientData client, DisconnectReasons reason);

		// Token: 0x060007E6 RID: 2022
		protected abstract void OnDisconnected();

		// Token: 0x060007E7 RID: 2023
		protected abstract void OnGetGameList(int totalGames, List<GameListing> availableGames);

		// Token: 0x060007E8 RID: 2024
		protected abstract byte[] GetConnectionData();

		// Token: 0x060007E9 RID: 2025 RVA: 0x0002F6D8 File Offset: 0x0002D8D8
		protected ClientData FindClientById(int id)
		{
			List<ClientData> obj = this.allClients;
			ClientData result;
			lock (obj)
			{
				for (int i = 0; i < this.allClients.Count; i++)
				{
					if (this.allClients[i].Id == id)
					{
						return this.allClients[i];
					}
				}
				result = null;
			}
			return result;
		}

		// Token: 0x060007EA RID: 2026 RVA: 0x0002F754 File Offset: 0x0002D954
		public static string IntToGameName(int gameId)
		{
			char[] array = new char[]
			{
				(char)(gameId & 255),
				(char)(gameId >> 8 & 255),
				(char)(gameId >> 16 & 255),
				(char)(gameId >> 24 & 255)
			};
			if (array.Any((char c) => c < 'A' || c > 'z'))
			{
				return null;
			}
			return new string(array);
		}

		// Token: 0x060007EB RID: 2027 RVA: 0x0002F7CB File Offset: 0x0002D9CB
		public static int GameNameToInt(string gameId)
		{
			if (gameId.Length != 4)
			{
				return -1;
			}
			gameId = gameId.ToUpperInvariant();
			return (int)(gameId[0] | (int)gameId[1] << 8 | (int)gameId[2] << 16 | (int)gameId[3] << 24);
		}

		// Token: 0x060007EC RID: 2028 RVA: 0x0002F808 File Offset: 0x0002DA08
		private void FixedUpdate()
		{
			if (this.mode == MatchMakerModes.None || this.Streams == null)
			{
				this.timer = 0f;
				return;
			}
			this.timer += Time.fixedDeltaTime;
			if (this.timer < this.MinSendInterval)
			{
				return;
			}
			this.timer = 0f;
			List<InnerNetObject> obj = this.allObjects;
			lock (obj)
			{
				for (int i = 0; i < this.allObjects.Count; i++)
				{
					InnerNetObject innerNetObject = this.allObjects[i];
					if (innerNetObject && innerNetObject.DirtyBits != 0U && (innerNetObject.AmOwner || (innerNetObject.OwnerId == -2 && this.AmHost)))
					{
						MessageWriter messageWriter = this.Streams[(int)innerNetObject.sendMode];
						messageWriter.StartMessage(1);
						messageWriter.WritePacked(innerNetObject.NetId);
						if (innerNetObject.Serialize(messageWriter, false))
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
			for (int j = 0; j < this.Streams.Length; j++)
			{
				MessageWriter messageWriter2 = this.Streams[j];
				if (messageWriter2.HasBytes(7))
				{
					messageWriter2.EndMessage();
					this.SendOrDisconnect(messageWriter2);
					messageWriter2.Clear((SendOption)j);
					messageWriter2.StartMessage(5);
					messageWriter2.Write(this.GameId);
				}
			}
		}

		// Token: 0x060007ED RID: 2029 RVA: 0x0002F974 File Offset: 0x0002DB74
		public T FindObjectByNetId<T>(uint netId) where T : InnerNetObject
		{
			InnerNetObject innerNetObject;
			if (this.allObjectsFast.TryGetValue(netId, out innerNetObject))
			{
				return (T)((object)innerNetObject);
			}
			Debug.LogWarning("Couldn't find target object: " + netId.ToString());
			return default(T);
		}

		// Token: 0x060007EE RID: 2030 RVA: 0x0002F9B8 File Offset: 0x0002DBB8
		public void SendRpcImmediately(uint targetNetId, byte callId, SendOption option)
		{
			MessageWriter messageWriter = MessageWriter.Get(option);
			messageWriter.StartMessage(5);
			messageWriter.Write(this.GameId);
			messageWriter.StartMessage(2);
			messageWriter.WritePacked(targetNetId);
			messageWriter.Write(callId);
			messageWriter.EndMessage();
			messageWriter.EndMessage();
			this.connection.Send(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007EF RID: 2031 RVA: 0x0002FA14 File Offset: 0x0002DC14
		public MessageWriter StartRpcImmediately(uint targetNetId, byte callId, SendOption option, int targetClientId = -1)
		{
			MessageWriter messageWriter = MessageWriter.Get(option);
			if (targetClientId < 0)
			{
				messageWriter.StartMessage(5);
				messageWriter.Write(this.GameId);
			}
			else
			{
				messageWriter.StartMessage(6);
				messageWriter.Write(this.GameId);
				messageWriter.WritePacked(targetClientId);
			}
			messageWriter.StartMessage(2);
			messageWriter.WritePacked(targetNetId);
			messageWriter.Write(callId);
			return messageWriter;
		}

		// Token: 0x060007F0 RID: 2032 RVA: 0x0002FA73 File Offset: 0x0002DC73
		public void FinishRpcImmediately(MessageWriter msg)
		{
			msg.EndMessage();
			msg.EndMessage();
			this.SendOrDisconnect(msg);
			msg.Recycle();
		}

		// Token: 0x060007F1 RID: 2033 RVA: 0x0002FA8E File Offset: 0x0002DC8E
		public void SendRpc(uint targetNetId, byte callId, SendOption option = SendOption.Reliable)
		{
			this.StartRpc(targetNetId, callId, option).EndMessage();
		}

		// Token: 0x060007F2 RID: 2034 RVA: 0x0002FA9E File Offset: 0x0002DC9E
		public MessageWriter StartRpc(uint targetNetId, byte callId, SendOption option = SendOption.Reliable)
		{
			MessageWriter messageWriter = this.Streams[(int)option];
			messageWriter.StartMessage(2);
			messageWriter.WritePacked(targetNetId);
			messageWriter.Write(callId);
			return messageWriter;
		}

		// Token: 0x060007F3 RID: 2035 RVA: 0x0002FABD File Offset: 0x0002DCBD
		private void SendSceneChange(string sceneName)
		{
			if (!this.AmConnected)
			{
				return;
			}
			UnityEngine.Object.Instantiate<GameObject>(new GameObject()).AddComponent<ProblemTraceConsole>();
			base.StartCoroutine(this.CoSendSceneChange(sceneName));
		}

		// Token: 0x060007F4 RID: 2036 RVA: 0x0002FAE6 File Offset: 0x0002DCE6
		private IEnumerator CoSendSceneChange(string sceneName)
		{
			List<InnerNetObject> obj = this.allObjects;
			lock (obj)
			{
				for (int i = this.allObjects.Count - 1; i > -1; i--)
				{
					if (!this.allObjects[i])
					{
						this.allObjects.RemoveAt(i);
					}
				}
				goto IL_C1;
			}
			IL_AA:
			yield return null;
			IL_C1:
			if (this.AmConnected && this.ClientId < 0)
			{
				goto IL_AA;
			}
			if (!this.AmConnected)
			{
				yield break;
			}
			if (!this.AmHost && this.connection.State == ConnectionState.Connected)
			{
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.StartMessage(5);
				messageWriter.Write(this.GameId);
				messageWriter.StartMessage(6);
				messageWriter.WritePacked(this.ClientId);
				messageWriter.Write(sceneName);
				messageWriter.EndMessage();
				messageWriter.EndMessage();
				this.SendOrDisconnect(messageWriter);
				messageWriter.Recycle();
			}
			ClientData client = this.FindClientById(this.ClientId);
			if (client != null)
			{
				Debug.Log(string.Format("Changed scene: {0} {1}", this.ClientId, sceneName));
				List<Action> dispatchQueue = this.DispatchQueue;
				lock (dispatchQueue)
				{
					this.DispatchQueue.Add(delegate
					{
						this.OnPlayerChangedScene(client, sceneName);
					});
					yield break;
				}
			}
			Debug.Log(string.Format("Couldn't find self in clients: {0}.", this.ClientId));
			yield break;
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0002FAFC File Offset: 0x0002DCFC
		public void Spawn(InnerNetObject netObjParent, int ownerId = -2, SpawnFlags flags = SpawnFlags.None)
		{
			if (this.AmHost)
			{
				ownerId = ((ownerId == -3) ? this.ClientId : ownerId);
				MessageWriter msg = this.Streams[1];
				this.WriteSpawnMessage(netObjParent, ownerId, flags, msg);
				return;
			}
			if (!this.AmClient)
			{
				return;
			}
			Debug.LogError("Tried to spawn while not host:" + ((netObjParent != null) ? netObjParent.ToString() : null));
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0002FB5C File Offset: 0x0002DD5C
		private void WriteSpawnMessage(InnerNetObject netObjParent, int ownerId, SpawnFlags flags, MessageWriter msg)
		{
			msg.StartMessage(4);
			msg.WritePacked(netObjParent.SpawnId);
			msg.WritePacked(ownerId);
			msg.Write((byte)flags);
			InnerNetObject[] componentsInChildren = netObjParent.GetComponentsInChildren<InnerNetObject>();
			msg.WritePacked(componentsInChildren.Length);
			foreach (InnerNetObject innerNetObject in componentsInChildren)
			{
				innerNetObject.OwnerId = ownerId;
				innerNetObject.SpawnFlags = flags;
				if (innerNetObject.NetId == 0U)
				{
					this.RegisterNetObject(innerNetObject);
				}
				msg.WritePacked(innerNetObject.NetId);
				msg.StartMessage(1);
				innerNetObject.Serialize(msg, true);
				msg.EndMessage();
			}
			msg.EndMessage();
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x0002FC00 File Offset: 0x0002DE00
		public void Despawn(InnerNetObject objToDespawn)
		{
			if (objToDespawn.NetId < 1U)
			{
				Debug.Log("Tried to net destroy: " + ((objToDespawn != null) ? objToDespawn.ToString() : null));
				return;
			}
			MessageWriter messageWriter = this.Streams[1];
			messageWriter.StartMessage(5);
			messageWriter.WritePacked(objToDespawn.NetId);
			messageWriter.EndMessage();
			this.RemoveNetObject(objToDespawn);
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0002FC5C File Offset: 0x0002DE5C
		private void RegisterNetObject(InnerNetObject obj)
		{
			if (obj.NetId == 0U)
			{
				uint netIdCnt = this.NetIdCnt;
				this.NetIdCnt = netIdCnt + 1U;
				obj.NetId = netIdCnt;
				this.allObjects.Add(obj);
				this.allObjectsFast.Add(obj.NetId, obj);
				return;
			}
			Debug.LogError("Attempted to double register: " + obj.name);
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0002FCBC File Offset: 0x0002DEBC
		private bool AddNetObject(InnerNetObject obj)
		{
			uint num = obj.NetId + 1U;
			this.NetIdCnt = ((this.NetIdCnt > num) ? this.NetIdCnt : num);
			if (!this.allObjectsFast.ContainsKey(obj.NetId))
			{
				this.allObjects.Add(obj);
				this.allObjectsFast.Add(obj.NetId, obj);
				return true;
			}
			return false;
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0002FD20 File Offset: 0x0002DF20
		public void RemoveNetObject(InnerNetObject obj)
		{
			int num = this.allObjects.IndexOf(obj);
			if (num > -1)
			{
				this.allObjects.RemoveAt(num);
			}
			this.allObjectsFast.Remove(obj.NetId);
			obj.NetId = uint.MaxValue;
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0002FD64 File Offset: 0x0002DF64
		public void RemoveUnownedObjects()
		{
			HashSet<int> hashSet = new HashSet<int>();
			hashSet.Add(-2);
			List<ClientData> obj = this.allClients;
			lock (obj)
			{
				for (int i = this.allClients.Count - 1; i >= 0; i--)
				{
					ClientData clientData = this.allClients[i];
					if (clientData.Character)
					{
						hashSet.Add(clientData.Id);
					}
				}
			}
			List<InnerNetObject> obj2 = this.allObjects;
			lock (obj2)
			{
				for (int j = this.allObjects.Count - 1; j > -1; j--)
				{
					InnerNetObject innerNetObject = this.allObjects[j];
					if (!innerNetObject)
					{
						this.allObjects.RemoveAt(j);
					}
					else if (!hashSet.Contains(innerNetObject.OwnerId))
					{
						innerNetObject.OwnerId = this.ClientId;
						UnityEngine.Object.Destroy(innerNetObject.gameObject);
					}
				}
			}
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0002FE88 File Offset: 0x0002E088
		private void HandleGameData(MessageReader parentReader)
		{
			try
			{
				while (parentReader.Position < parentReader.Length)
				{
					MessageReader messageReader = parentReader.ReadMessage();
					switch (messageReader.Tag)
					{
						case 1:
							{
								uint key = messageReader.ReadPackedUInt32();
								InnerNetObject innerNetObject;
								if (this.allObjectsFast.TryGetValue(key, out innerNetObject))
								{
									innerNetObject.Deserialize(messageReader, false);
									continue;
								}
								continue;
							}
						case 2:
							{
								uint key2 = messageReader.ReadPackedUInt32();
								InnerNetObject innerNetObject2;
								if (this.allObjectsFast.TryGetValue(key2, out innerNetObject2))
								{
									innerNetObject2.HandleRpc(messageReader.ReadByte(), messageReader);
									continue;
								}
								continue;
							}
						case 3:
							goto IL_336;
						case 4:
							{
								uint num = messageReader.ReadPackedUInt32();
								if ((ulong)num >= (ulong)((long)this.SpawnableObjects.Length))
								{
									Debug.LogError("Couldn't find spawnable prefab: " + num.ToString());
									continue;
								}
								InnerNetObject innerNetObject3 = UnityEngine.Object.Instantiate<InnerNetObject>(this.SpawnableObjects[(int)num]);
								int num2 = messageReader.ReadPackedInt32();
								innerNetObject3.SpawnFlags = (SpawnFlags)messageReader.ReadByte();
								int num3 = messageReader.ReadPackedInt32();
								InnerNetObject[] componentsInChildren = innerNetObject3.GetComponentsInChildren<InnerNetObject>();
								if (num3 != componentsInChildren.Length)
								{
									Debug.LogError("Children didn't match for spawnable " + num.ToString());
									UnityEngine.Object.Destroy(innerNetObject3.gameObject);
									continue;
								}
								for (int i = 0; i < num3; i++)
								{
									InnerNetObject innerNetObject4 = componentsInChildren[i];
									innerNetObject4.NetId = messageReader.ReadPackedUInt32();
									innerNetObject4.OwnerId = num2;
									if (!this.AddNetObject(innerNetObject4))
									{
										innerNetObject3.NetId = uint.MaxValue;
										UnityEngine.Object.Destroy(innerNetObject3.gameObject);
										break;
									}
									MessageReader messageReader2 = messageReader.ReadMessage();
									if (messageReader2.Length > 0)
									{
										innerNetObject4.Deserialize(messageReader2, true);
									}
								}
								if ((innerNetObject3.SpawnFlags & SpawnFlags.IsClientCharacter) == SpawnFlags.None)
								{
									continue;
								}
								ClientData clientData = this.FindClientById(num2);
								if (clientData == null)
								{
									Debug.LogWarning("Spawn unowned character");
									UnityEngine.Object.Destroy(innerNetObject3.gameObject);
									continue;
								}
								if (!clientData.Character)
								{
									clientData.InScene = true;
									clientData.Character = (innerNetObject3 as PlayerControl);
									continue;
								}
								Debug.LogWarning("Double spawn character");
								UnityEngine.Object.Destroy(innerNetObject3.gameObject);
								continue;
							}
						case 5:
							{
								uint netId = messageReader.ReadPackedUInt32();
								InnerNetObject innerNetObject5 = this.FindObjectByNetId<InnerNetObject>(netId);
								if (innerNetObject5)
								{
									this.RemoveNetObject(innerNetObject5);
									UnityEngine.Object.Destroy(innerNetObject5.gameObject);
									continue;
								}
								continue;
							}
						case 6:
							{
								ClientData client = this.FindClientById(messageReader.ReadPackedInt32());
								string targetScene = messageReader.ReadString();
								string format = "Client {0} changed scene to {1}";
								ClientData client2 = client;
								Debug.Log(string.Format(format, (client2 != null) ? client2.Id : -1, targetScene));
								if (client == null || string.IsNullOrWhiteSpace(targetScene))
								{
									continue;
								}
								List<Action> dispatchQueue = this.DispatchQueue;
								lock (dispatchQueue)
								{
									this.DispatchQueue.Add(delegate
									{
										this.OnPlayerChangedScene(client, targetScene);
									});
									continue;
								}
								break;
							}
						case 7:
							break;
						default:
							goto IL_336;
					}
					ClientData clientData2 = this.FindClientById(messageReader.ReadPackedInt32());
					if (clientData2 != null)
					{
						Debug.Log(string.Format("Client {0} ready", clientData2.Id));
						clientData2.IsReady = true;
						continue;
					}
					continue;
					IL_336:
					Debug.Log(string.Format("Bad tag {0} at {1}+{2}={3}:  ", new object[]
					{
						messageReader.Tag,
						messageReader.Offset,
						messageReader.Position,
						messageReader.Length
					}) + string.Join<byte>(" ", messageReader.Buffer));
				}
			}
			finally
			{
				parentReader.Recycle();
			}
		}

		// Token: 0x040007E2 RID: 2018
		private static readonly DisconnectReasons[] disconnectReasons = new DisconnectReasons[]
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

		// Token: 0x040007E3 RID: 2019
		public const int NoClientId = -1;

		// Token: 0x040007E4 RID: 2020
		private string networkAddress = "127.0.0.1";

		// Token: 0x040007E5 RID: 2021
		private int networkPort;

		// Token: 0x040007E6 RID: 2022
		private UdpClientConnection connection;

		// Token: 0x040007E7 RID: 2023
		public MatchMakerModes mode;

		// Token: 0x040007E8 RID: 2024
		public int GameId = 32;

		// Token: 0x040007E9 RID: 2025
		public int HostId;

		// Token: 0x040007EA RID: 2026
		public int ClientId = -1;

		// Token: 0x040007EB RID: 2027
		public List<ClientData> allClients = new List<ClientData>();

		// Token: 0x040007EC RID: 2028
		public DisconnectReasons LastDisconnectReason;

		// Token: 0x040007ED RID: 2029
		public string LastCustomDisconnect;

		// Token: 0x040007EE RID: 2030
		private readonly List<Action> DispatchQueue = new List<Action>();

		// Token: 0x040007EF RID: 2031
		public InnerNetClient.GameStates GameState;

		// Token: 0x040007F0 RID: 2032
		private const int KicksPerGame = 0;

		// Token: 0x040007F1 RID: 2033
		private volatile bool appPaused;

		// Token: 0x040007F2 RID: 2034
		public const int CurrentClient = -3;

		// Token: 0x040007F3 RID: 2035
		public const int InvalidClient = -2;

		// Token: 0x040007F4 RID: 2036
		internal const byte DataFlag = 1;

		// Token: 0x040007F5 RID: 2037
		internal const byte RpcFlag = 2;

		// Token: 0x040007F6 RID: 2038
		internal const byte SpawnFlag = 4;

		// Token: 0x040007F7 RID: 2039
		internal const byte DespawnFlag = 5;

		// Token: 0x040007F8 RID: 2040
		internal const byte SceneChangeFlag = 6;

		// Token: 0x040007F9 RID: 2041
		internal const byte ReadyFlag = 7;

		// Token: 0x040007FA RID: 2042
		internal const byte ChangeSettingsFlag = 8;

		// Token: 0x040007FB RID: 2043
		public float MinSendInterval = 0.1f;

		// Token: 0x040007FC RID: 2044
		private uint NetIdCnt = 1U;

		// Token: 0x040007FD RID: 2045
		private float timer;

		// Token: 0x040007FE RID: 2046
		public InnerNetObject[] SpawnableObjects;

		// Token: 0x040007FF RID: 2047
		public List<InnerNetObject> allObjects = new List<InnerNetObject>();

		// Token: 0x04000800 RID: 2048
		private Dictionary<uint, InnerNetObject> allObjectsFast = new Dictionary<uint, InnerNetObject>();

		// Token: 0x04000801 RID: 2049
		private MessageWriter[] Streams;

		// Token: 0x02000258 RID: 600
		public enum GameStates
		{
			// Token: 0x04000C67 RID: 3175
			NotJoined,
			// Token: 0x04000C68 RID: 3176
			Joined,
			// Token: 0x04000C69 RID: 3177
			Started,
			// Token: 0x04000C6A RID: 3178
			Ended
		}
	}
}
