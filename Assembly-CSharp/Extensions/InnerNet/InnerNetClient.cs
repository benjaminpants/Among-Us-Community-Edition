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
	// Token: 0x02000246 RID: 582
	public abstract class InnerNetClient : MonoBehaviour
	{
		// Token: 0x06000C73 RID: 3187 RVA: 0x000097BC File Offset: 0x000079BC
		public void SetEndpoint(string addr, ushort port)
		{
			this.networkAddress = addr;
			this.networkPort = (int)port;
		}

		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06000C74 RID: 3188 RVA: 0x000097CC File Offset: 0x000079CC
		private bool AmConnected
		{
			get
			{
				return this.connection != null;
			}
		}

		// Token: 0x170001E3 RID: 483
		// (get) Token: 0x06000C75 RID: 3189 RVA: 0x000097D7 File Offset: 0x000079D7
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

		// Token: 0x170001E4 RID: 484
		// (get) Token: 0x06000C76 RID: 3190 RVA: 0x000097EF File Offset: 0x000079EF
		public bool AmHost
		{
			get
			{
				return this.HostId == this.ClientId;
			}
		}

		// Token: 0x170001E5 RID: 485
		// (get) Token: 0x06000C77 RID: 3191 RVA: 0x000097FF File Offset: 0x000079FF
		public bool AmClient
		{
			get
			{
				return this.ClientId > 0;
			}
		}

		// Token: 0x170001E6 RID: 486
		// (get) Token: 0x06000C78 RID: 3192 RVA: 0x0000980A File Offset: 0x00007A0A
		// (set) Token: 0x06000C79 RID: 3193 RVA: 0x00009812 File Offset: 0x00007A12
		public bool IsGamePublic { get; private set; }

		// Token: 0x170001E7 RID: 487
		// (get) Token: 0x06000C7A RID: 3194 RVA: 0x0000981B File Offset: 0x00007A1B
		public bool IsGameStarted
		{
			get
			{
				return this.GameState == InnerNetClient.GameStates.Started;
			}
		}

		// Token: 0x170001E8 RID: 488
		// (get) Token: 0x06000C7B RID: 3195 RVA: 0x00009826 File Offset: 0x00007A26
		public bool IsGameOver
		{
			get
			{
				return this.GameState == InnerNetClient.GameStates.Ended;
			}
		}

		// Token: 0x06000C7C RID: 3196 RVA: 0x00009831 File Offset: 0x00007A31
		public virtual void Start()
		{
			SceneManager.activeSceneChanged += delegate(Scene oldScene, Scene scene)
			{
				this.SendSceneChange(scene.name);
			};
			this.ClientId = -1;
			UnityEngine.Object.Instantiate<GameObject>(new GameObject()).AddComponent<ProblemTraceConsole>();
			this.GameId = 32;
		}

		// Token: 0x06000C7D RID: 3197 RVA: 0x0003C3A4 File Offset: 0x0003A5A4
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

		// Token: 0x06000C7E RID: 3198 RVA: 0x0003C3E0 File Offset: 0x0003A5E0
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

		// Token: 0x06000C7F RID: 3199 RVA: 0x0003C42C File Offset: 0x0003A62C
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

		// Token: 0x06000C80 RID: 3200 RVA: 0x00009863 File Offset: 0x00007A63
		public virtual void OnDestroy()
		{
			if (this.AmConnected)
			{
				this.DisconnectInternal(DisconnectReasons.Destroy, null);
			}
		}

		// Token: 0x06000C81 RID: 3201 RVA: 0x00009876 File Offset: 0x00007A76
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

		// Token: 0x06000C82 RID: 3202 RVA: 0x00009885 File Offset: 0x00007A85
		private void Connection_DataReceivedRaw(byte[] data)
		{
			Debug.Log("Client Got: " + string.Join(" ", from b in data
			select b.ToString()));
		}

		// Token: 0x06000C83 RID: 3203 RVA: 0x0003C47C File Offset: 0x0003A67C
		private void Connection_DataSentRaw(byte[] data, int length)
		{
			Debug.Log("Client Sent: " + string.Join(" ", (from b in data
			select b.ToString()).ToArray<string>(), 0, length));
		}

		// Token: 0x06000C84 RID: 3204 RVA: 0x000098C5 File Offset: 0x00007AC5
		public void Connect(MatchMakerModes mode)
		{
			base.StartCoroutine(this.CoConnect(mode));
		}

		// Token: 0x06000C85 RID: 3205 RVA: 0x000098D5 File Offset: 0x00007AD5
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

		// Token: 0x06000C86 RID: 3206 RVA: 0x000098EB File Offset: 0x00007AEB
		public IEnumerator WaitForConnectionOrFail()
		{
			while (this.AmConnected)
			{
				switch (this.mode)
				{
				case MatchMakerModes.None:
					goto IL_5F;
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
					goto IL_5F;
				}
				yield return null;
				continue;
				IL_5F:
				yield break;
			}
			yield break;
		}

		// Token: 0x06000C87 RID: 3207 RVA: 0x000098FA File Offset: 0x00007AFA
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

		// Token: 0x06000C88 RID: 3208 RVA: 0x0003C4D0 File Offset: 0x0003A6D0
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

		// Token: 0x06000C89 RID: 3209 RVA: 0x00009910 File Offset: 0x00007B10
		private void OnDisconnect(object sender, DisconnectedEventArgs e)
		{
			this.EnqueueDisconnect(DisconnectReasons.Error, e.Reason);
		}

		// Token: 0x06000C8A RID: 3210 RVA: 0x00009920 File Offset: 0x00007B20
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

		// Token: 0x06000C8B RID: 3211 RVA: 0x0003C598 File Offset: 0x0003A798
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

		// Token: 0x06000C8C RID: 3212 RVA: 0x0003C618 File Offset: 0x0003A818
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

		// Token: 0x06000C8D RID: 3213 RVA: 0x0003C724 File Offset: 0x0003A924
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

		// Token: 0x06000C8E RID: 3214 RVA: 0x0003C770 File Offset: 0x0003A970
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

		// Token: 0x170001E9 RID: 489
		// (get) Token: 0x06000C8F RID: 3215 RVA: 0x0000995F File Offset: 0x00007B5F
		// (set) Token: 0x06000C90 RID: 3216 RVA: 0x00009967 File Offset: 0x00007B67
		public int KicksLeft { get; private set; } = 1;

		// Token: 0x06000C91 RID: 3217 RVA: 0x00009970 File Offset: 0x00007B70
		public bool CanKick()
		{
			return this.AmHost && this.KicksLeft != 0;
		}

		// Token: 0x06000C92 RID: 3218 RVA: 0x0003C7E0 File Offset: 0x0003A9E0
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

		// Token: 0x06000C93 RID: 3219 RVA: 0x00009985 File Offset: 0x00007B85
		public MessageWriter StartEndGame()
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(8);
			messageWriter.Write(this.GameId);
			return messageWriter;
		}

		// Token: 0x06000C94 RID: 3220 RVA: 0x000099A0 File Offset: 0x00007BA0
		public void FinishEndGame(MessageWriter msg)
		{
			msg.EndMessage();
			this.SendOrDisconnect(msg);
			msg.Recycle();
		}

		// Token: 0x06000C95 RID: 3221 RVA: 0x0003C84C File Offset: 0x0003AA4C
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

		// Token: 0x06000C96 RID: 3222 RVA: 0x0003C898 File Offset: 0x0003AA98
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

		// Token: 0x06000C97 RID: 3223 RVA: 0x0003C908 File Offset: 0x0003AB08
		protected void SendStartGame()
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(2);
			messageWriter.Write(this.GameId);
			messageWriter.EndMessage();
			this.SendOrDisconnect(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x06000C98 RID: 3224 RVA: 0x0003C944 File Offset: 0x0003AB44
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

		// Token: 0x06000C99 RID: 3225 RVA: 0x0003C988 File Offset: 0x0003AB88
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

		// Token: 0x06000C9A RID: 3226 RVA: 0x0003C9E0 File Offset: 0x0003ABE0
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

		// Token: 0x06000C9B RID: 3227 RVA: 0x0003CA2C File Offset: 0x0003AC2C
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
				goto IL_310;
			case 2:
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
				goto IL_60E;
			case 3:
			{
				DisconnectReasons reason3 = DisconnectReasons.ServerRequest;
				if (reader.Position < reader.Length)
				{
					reason3 = (DisconnectReasons)reader.ReadByte();
				}
				this.EnqueueDisconnect(reason3, null);
				return;
			}
			case 4:
				break;
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
					goto IL_4A9;
				}
				return;
			}
			case 7:
				goto IL_24E;
			case 8:
				goto IL_142;
			case 9:
				goto IL_4A9;
			case 10:
				goto IL_54E;
			case 11:
				goto IL_60E;
			case 12:
				goto IL_1F6;
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
						base.StopAllCoroutines();
						this.Connect(this.mode);
					});
					return;
				}
				goto IL_6A3;
			}
			default:
				goto IL_6A3;
			}
			int num3 = reader.ReadInt32();
			if (this.GameId != num3)
			{
				return;
			}
			int playerIdThatLeft = reader.ReadInt32();
			bool amHost = this.AmHost;
			this.HostId = reader.ReadInt32();
			DisconnectReasons reason2 = DisconnectReasons.ExitGame;
			if (reader.Position < reader.Length)
			{
				reason2 = (DisconnectReasons)reader.ReadByte();
			}
			this.RemovePlayer(playerIdThatLeft, reason2);
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
			IL_142:
			int num4 = reader.ReadInt32();
			if (this.GameId != num4 || this.GameState == InnerNetClient.GameStates.Ended)
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
			IL_1F6:
			int num5 = reader.ReadInt32();
			if (this.GameId != num5)
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
			IL_24E:
			int num6 = reader.ReadInt32();
			if (this.GameId != num6)
			{
				return;
			}
			this.ClientId = reader.ReadInt32();
			this.GameState = InnerNetClient.GameStates.Joined;
			this.KicksLeft = -1;
			ClientData myClient = this.GetOrCreateClient(this.ClientId);
			bool amHost2 = this.AmHost;
			this.HostId = reader.ReadInt32();
			int num7 = reader.ReadPackedInt32();
			for (int i = 0; i < num7; i++)
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
			IL_310:
			int num8 = reader.ReadInt32();
			DisconnectReasons disconnectReasons = (DisconnectReasons)num8;
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
			if (this.GameId == num8)
			{
				int num9 = reader.ReadInt32();
				bool amHost3 = this.AmHost;
				this.HostId = reader.ReadInt32();
				ClientData client = this.GetOrCreateClient(num9);
				Debug.Log(string.Format("Player {0} joined", num9));
				dispatchQueue = this.DispatchQueue;
				lock (dispatchQueue)
				{
					this.DispatchQueue.Add(delegate
					{
						this.OnPlayerJoined(client);
					});
					if (this.AmHost && !amHost3)
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
			IL_4A9:
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
			IL_54E:
			int num10 = reader.ReadInt32();
			if (this.GameId != num10)
			{
				return;
			}
			byte b = reader.ReadByte();
			if (b == 1)
			{
				this.IsGamePublic = reader.ReadBoolean();
				string str = "Alter Public = ";
				bool flag = this.IsGamePublic;
				Debug.Log(str + flag.ToString());
				return;
			}
			Debug.Log("Alter unknown");
			return;
			IL_60E:
			int num11 = reader.ReadInt32();
			if (this.GameId == num11 && reader.ReadPackedInt32() == this.ClientId)
			{
				this.EnqueueDisconnect(reader.ReadBoolean() ? DisconnectReasons.Banned : DisconnectReasons.Kicked, null);
				return;
			}
			return;
			IL_6A3:
			Debug.Log(string.Format("Bad tag {0} at {1}+{2}={3}:  ", new object[]
			{
				reader.Tag,
				reader.Offset,
				reader.Position,
				reader.Length
			}) + string.Join<byte>(" ", reader.Buffer));
		}

		// Token: 0x06000C9C RID: 3228 RVA: 0x0003D1C8 File Offset: 0x0003B3C8
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

		// Token: 0x06000C9D RID: 3229 RVA: 0x0003D218 File Offset: 0x0003B418
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

		// Token: 0x06000C9E RID: 3230 RVA: 0x0003D294 File Offset: 0x0003B494
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

		// Token: 0x06000C9F RID: 3231 RVA: 0x0003D378 File Offset: 0x0003B578
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

		// Token: 0x06000CA0 RID: 3232 RVA: 0x0003D3D8 File Offset: 0x0003B5D8
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

		// Token: 0x06000CA1 RID: 3233 RVA: 0x0003D43C File Offset: 0x0003B63C
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

		// Token: 0x06000CA2 RID: 3234
		protected abstract void OnGameCreated(string gameIdString);

		// Token: 0x06000CA3 RID: 3235
		protected abstract void OnGameJoined(string gameIdString, ClientData client);

		// Token: 0x06000CA4 RID: 3236
		protected abstract void OnWaitForHost(string gameIdString);

		// Token: 0x06000CA5 RID: 3237
		protected abstract void OnStartGame();

		// Token: 0x06000CA6 RID: 3238
		protected abstract void OnGameEnd(GameOverReason reason, bool showAd);

		// Token: 0x06000CA7 RID: 3239
		protected abstract void OnBecomeHost();

		// Token: 0x06000CA8 RID: 3240
		protected abstract void OnPlayerJoined(ClientData client);

		// Token: 0x06000CA9 RID: 3241
		protected abstract void OnPlayerChangedScene(ClientData client, string targetScene);

		// Token: 0x06000CAA RID: 3242
		protected abstract void OnPlayerLeft(ClientData client, DisconnectReasons reason);

		// Token: 0x06000CAB RID: 3243
		protected abstract void OnDisconnected();

		// Token: 0x06000CAC RID: 3244
		protected abstract void OnGetGameList(int totalGames, List<GameListing> availableGames);

		// Token: 0x06000CAD RID: 3245
		protected abstract byte[] GetConnectionData();

		// Token: 0x06000CAE RID: 3246 RVA: 0x0003D508 File Offset: 0x0003B708
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

		// Token: 0x06000CAF RID: 3247 RVA: 0x0003D580 File Offset: 0x0003B780
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

		// Token: 0x06000CB0 RID: 3248 RVA: 0x000099B5 File Offset: 0x00007BB5
		public static int GameNameToInt(string gameId)
		{
			if (gameId.Length != 4)
			{
				return -1;
			}
			gameId = gameId.ToUpperInvariant();
			return (int)(gameId[0] | (int)gameId[1] << 8 | (int)gameId[2] << 16 | (int)gameId[3] << 24);
		}

		// Token: 0x06000CB1 RID: 3249 RVA: 0x0003D5F8 File Offset: 0x0003B7F8
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

		// Token: 0x06000CB2 RID: 3250 RVA: 0x0003D764 File Offset: 0x0003B964
		public T FindObjectByNetId<T>(uint netId) where T : InnerNetObject
		{
			InnerNetObject innerNetObject;
			if (this.allObjectsFast.TryGetValue(netId, out innerNetObject))
			{
				return (T)((object)innerNetObject);
			}
			Debug.LogWarning("Couldn't find target object: " + netId);
			return default(T);
		}

		// Token: 0x06000CB3 RID: 3251 RVA: 0x0003D7A8 File Offset: 0x0003B9A8
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

		// Token: 0x06000CB4 RID: 3252 RVA: 0x0003D804 File Offset: 0x0003BA04
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

		// Token: 0x06000CB5 RID: 3253 RVA: 0x000099F1 File Offset: 0x00007BF1
		public void FinishRpcImmediately(MessageWriter msg)
		{
			msg.EndMessage();
			msg.EndMessage();
			this.SendOrDisconnect(msg);
			msg.Recycle();
		}

		// Token: 0x06000CB6 RID: 3254 RVA: 0x00009A0C File Offset: 0x00007C0C
		public void SendRpc(uint targetNetId, byte callId, SendOption option = SendOption.Reliable)
		{
			this.StartRpc(targetNetId, callId, option).EndMessage();
		}

		// Token: 0x06000CB7 RID: 3255 RVA: 0x00009A1C File Offset: 0x00007C1C
		public MessageWriter StartRpc(uint targetNetId, byte callId, SendOption option = SendOption.Reliable)
		{
			MessageWriter messageWriter = this.Streams[(int)option];
			messageWriter.StartMessage(2);
			messageWriter.WritePacked(targetNetId);
			messageWriter.Write(callId);
			return messageWriter;
		}

		// Token: 0x06000CB8 RID: 3256 RVA: 0x00009A3B File Offset: 0x00007C3B
		private void SendSceneChange(string sceneName)
		{
			if (!this.AmConnected)
			{
				return;
			}
			UnityEngine.Object.Instantiate<GameObject>(new GameObject()).AddComponent<ProblemTraceConsole>();
			base.StartCoroutine(this.CoSendSceneChange(sceneName));
		}

		// Token: 0x06000CB9 RID: 3257 RVA: 0x00009A64 File Offset: 0x00007C64
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
				goto IL_BF;
			}
			IL_A8:
			yield return null;
			IL_BF:
			if (this.AmConnected && this.ClientId < 0)
			{
				goto IL_A8;
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

		// Token: 0x06000CBA RID: 3258 RVA: 0x0003D864 File Offset: 0x0003BA64
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
			Debug.LogError("Tried to spawn while not host:" + netObjParent);
		}

		// Token: 0x06000CBB RID: 3259 RVA: 0x0003D8B8 File Offset: 0x0003BAB8
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

		// Token: 0x06000CBC RID: 3260 RVA: 0x0003D958 File Offset: 0x0003BB58
		public void Despawn(InnerNetObject objToDespawn)
		{
			if (objToDespawn.NetId < 1U)
			{
				Debug.Log("Tried to net destroy: " + objToDespawn);
				return;
			}
			MessageWriter messageWriter = this.Streams[1];
			messageWriter.StartMessage(5);
			messageWriter.WritePacked(objToDespawn.NetId);
			messageWriter.EndMessage();
			this.RemoveNetObject(objToDespawn);
		}

		// Token: 0x06000CBD RID: 3261 RVA: 0x0003D9A8 File Offset: 0x0003BBA8
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

		// Token: 0x06000CBE RID: 3262 RVA: 0x0003DA08 File Offset: 0x0003BC08
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

		// Token: 0x06000CBF RID: 3263 RVA: 0x0003DA6C File Offset: 0x0003BC6C
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

		// Token: 0x06000CC0 RID: 3264 RVA: 0x0003DAB0 File Offset: 0x0003BCB0
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

		// Token: 0x06000CC1 RID: 3265 RVA: 0x0003DBD4 File Offset: 0x0003BDD4
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
						goto IL_338;
					case 4:
					{
						uint num = messageReader.ReadPackedUInt32();
						if ((ulong)num >= (ulong)((long)this.SpawnableObjects.Length))
						{
							Debug.LogError("Couldn't find spawnable prefab: " + num);
							continue;
						}
						InnerNetObject innerNetObject3 = UnityEngine.Object.Instantiate<InnerNetObject>(this.SpawnableObjects[(int)num]);
						int num2 = messageReader.ReadPackedInt32();
						innerNetObject3.SpawnFlags = (SpawnFlags)messageReader.ReadByte();
						int num3 = messageReader.ReadPackedInt32();
						InnerNetObject[] componentsInChildren = innerNetObject3.GetComponentsInChildren<InnerNetObject>();
						if (num3 != componentsInChildren.Length)
						{
							Debug.LogError("Children didn't match for spawnable " + num);
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
						goto IL_338;
					}
					ClientData clientData2 = this.FindClientById(messageReader.ReadPackedInt32());
					if (clientData2 != null)
					{
						Debug.Log(string.Format("Client {0} ready", clientData2.Id));
						clientData2.IsReady = true;
						continue;
					}
					continue;
					IL_338:
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

		// Token: 0x04000C15 RID: 3093
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

		// Token: 0x04000C16 RID: 3094
		public const int NoClientId = -1;

		// Token: 0x04000C17 RID: 3095
		private string networkAddress = "127.0.0.1";

		// Token: 0x04000C18 RID: 3096
		private int networkPort;

		// Token: 0x04000C19 RID: 3097
		private UdpClientConnection connection;

		// Token: 0x04000C1A RID: 3098
		public MatchMakerModes mode;

		// Token: 0x04000C1B RID: 3099
		public int GameId = 32;

		// Token: 0x04000C1C RID: 3100
		public int HostId;

		// Token: 0x04000C1D RID: 3101
		public int ClientId = -1;

		// Token: 0x04000C1E RID: 3102
		public List<ClientData> allClients = new List<ClientData>();

		// Token: 0x04000C1F RID: 3103
		public DisconnectReasons LastDisconnectReason;

		// Token: 0x04000C20 RID: 3104
		public string LastCustomDisconnect;

		// Token: 0x04000C21 RID: 3105
		private readonly List<Action> DispatchQueue = new List<Action>();

		// Token: 0x04000C23 RID: 3107
		public InnerNetClient.GameStates GameState;

		// Token: 0x04000C24 RID: 3108
		private const int KicksPerGame = 0;

		// Token: 0x04000C26 RID: 3110
		private volatile bool appPaused;

		// Token: 0x04000C27 RID: 3111
		public const int CurrentClient = -3;

		// Token: 0x04000C28 RID: 3112
		public const int InvalidClient = -2;

		// Token: 0x04000C29 RID: 3113
		internal const byte DataFlag = 1;

		// Token: 0x04000C2A RID: 3114
		internal const byte RpcFlag = 2;

		// Token: 0x04000C2B RID: 3115
		internal const byte SpawnFlag = 4;

		// Token: 0x04000C2C RID: 3116
		internal const byte DespawnFlag = 5;

		// Token: 0x04000C2D RID: 3117
		internal const byte SceneChangeFlag = 6;

		// Token: 0x04000C2E RID: 3118
		internal const byte ReadyFlag = 7;

		// Token: 0x04000C2F RID: 3119
		internal const byte ChangeSettingsFlag = 8;

		// Token: 0x04000C30 RID: 3120
		public float MinSendInterval = 0.1f;

		// Token: 0x04000C31 RID: 3121
		private uint NetIdCnt = 1U;

		// Token: 0x04000C32 RID: 3122
		private float timer;

		// Token: 0x04000C33 RID: 3123
		public InnerNetObject[] SpawnableObjects;

		// Token: 0x04000C34 RID: 3124
		public List<InnerNetObject> allObjects = new List<InnerNetObject>();

		// Token: 0x04000C35 RID: 3125
		private Dictionary<uint, InnerNetObject> allObjectsFast = new Dictionary<uint, InnerNetObject>();

		// Token: 0x04000C36 RID: 3126
		private MessageWriter[] Streams;

		// Token: 0x02000247 RID: 583
		public enum GameStates
		{
			// Token: 0x04000C38 RID: 3128
			NotJoined,
			// Token: 0x04000C39 RID: 3129
			Joined,
			// Token: 0x04000C3A RID: 3130
			Started,
			// Token: 0x04000C3B RID: 3131
			Ended
		}
	}
}
