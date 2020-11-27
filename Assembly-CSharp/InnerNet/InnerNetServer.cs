using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Hazel;
using Hazel.Udp;
using UnityEngine;

namespace InnerNet
{
	// Token: 0x0200025E RID: 606
	public class InnerNetServer : DestroyableSingleton<InnerNetServer>
	{
		// Token: 0x06000D12 RID: 3346 RVA: 0x00009DB6 File Offset: 0x00007FB6
		public override void OnDestroy()
		{
			this.StopServer();
			base.OnDestroy();
		}

		// Token: 0x06000D13 RID: 3347 RVA: 0x0003E65C File Offset: 0x0003C85C
		public void StartAsServer()
		{
			if (this.listener != null)
			{
				this.StopServer();
			}
			this.GameState = GameStates.NotStarted;
			this.listener = new UdpConnectionListener(new IPEndPoint(IPAddress.Any, this.Port), IPMode.IPv4, null);
			this.listener.NewConnection += this.OnServerConnect;
			this.listener.Start();
			this.Running = true;
		}

		// Token: 0x06000D14 RID: 3348 RVA: 0x0003E6C4 File Offset: 0x0003C8C4
		public void StopServer()
		{
			this.HostId = -1;
			this.Running = false;
			this.GameState = GameStates.Destroyed;
			if (this.listener != null)
			{
				this.listener.Close();
				this.listener.Dispose();
				this.listener = null;
			}
			List<InnerNetServer.Player> clients = this.Clients;
			lock (clients)
			{
				this.Clients.Clear();
			}
		}

		// Token: 0x06000D15 RID: 3349 RVA: 0x00009DC4 File Offset: 0x00007FC4
		public static bool IsCompatibleVersion(int version)
		{
			return Constants.CompatVersions.Contains(version);
		}

		// Token: 0x06000D16 RID: 3350 RVA: 0x0003E744 File Offset: 0x0003C944
		private void OnServerConnect(NewConnectionEventArgs evt)
		{
			MessageReader handshakeData = evt.HandshakeData;
			try
			{
				if (evt.HandshakeData.Length < 5)
				{
					InnerNetServer.SendIncorrectVersion(evt.Connection);
					return;
				}
				if (!InnerNetServer.IsCompatibleVersion(handshakeData.ReadInt32()))
				{
					InnerNetServer.SendIncorrectVersion(evt.Connection);
					return;
				}
			}
			finally
			{
				handshakeData.Recycle();
			}
			InnerNetServer.Player client = new InnerNetServer.Player(evt.Connection);
			Debug.Log(string.Format("Client {0} added: {1}", client.Id, evt.Connection.EndPoint));
			UdpConnection udpConnection = (UdpConnection)evt.Connection;
			udpConnection.KeepAliveInterval = 1500;
			udpConnection.DisconnectTimeout = 6000;
			udpConnection.ResendPingMultiplier = 1.5f;
			udpConnection.DataReceived += delegate(DataReceivedEventArgs e)
			{
				this.OnDataReceived(client, e);
			};
			udpConnection.Disconnected += delegate(object o, DisconnectedEventArgs e)
			{
				this.ClientDisconnect(client);
			};
		}

		// Token: 0x06000D17 RID: 3351 RVA: 0x0003E848 File Offset: 0x0003CA48
		private static void SendIncorrectVersion(Connection connection)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(1);
			messageWriter.Write(5);
			messageWriter.EndMessage();
			connection.Send(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x06000D18 RID: 3352 RVA: 0x0003E880 File Offset: 0x0003CA80
		private void Connection_DataSentRaw(byte[] data, int length)
		{
			Debug.Log("Server Sent: " + string.Join(" ", data.Select(delegate(byte b)
			{
				byte b2 = b;
				return b2.ToString();
			}).ToArray<string>(), 0, length));
		}

		// Token: 0x06000D19 RID: 3353 RVA: 0x0003E8D4 File Offset: 0x0003CAD4
		private void OnDataReceived(InnerNetServer.Player client, DataReceivedEventArgs evt)
		{
			MessageReader message = evt.Message;
			if (message.Length <= 0)
			{
				Debug.Log("Server got 0 bytes");
				message.Recycle();
				return;
			}
			try
			{
				while (message.Position < message.Length)
				{
					this.HandleMessage(client, message.ReadMessage(), evt.SendOption);
				}
			}
			catch (Exception arg)
			{
				Debug.Log(string.Format("{0}\r\n{1}", string.Join<byte>(" ", message.Buffer), arg));
			}
			finally
			{
				message.Recycle();
			}
		}

		// Token: 0x06000D1A RID: 3354 RVA: 0x0003E970 File Offset: 0x0003CB70
		private void HandleMessage(InnerNetServer.Player client, MessageReader reader, SendOption sendOption)
		{
			switch (reader.Tag)
			{
			case 0:
			{
				Debug.Log("Server got host game");
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.StartMessage(0);
				messageWriter.Write(32);
				messageWriter.EndMessage();
				client.Connection.Send(messageWriter);
				messageWriter.Recycle();
				return;
			}
			case 1:
			{
				Debug.Log("Server got join game");
				if (reader.ReadInt32() == 32)
				{
					this.JoinGame(client);
					return;
				}
				MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
				messageWriter2.StartMessage(1);
				messageWriter2.Write(3);
				messageWriter2.EndMessage();
				client.Connection.Send(messageWriter2);
				messageWriter2.Recycle();
				return;
			}
			case 2:
				if (reader.ReadInt32() == 32)
				{
					this.StartGame(reader, client);
					return;
				}
				break;
			case 3:
				if (reader.ReadInt32() == 32)
				{
					this.ClientDisconnect(client);
					return;
				}
				break;
			case 4:
			case 7:
			case 9:
			case 10:
				break;
			case 5:
				if (this.Clients.Contains(client))
				{
					if (reader.ReadInt32() == 32)
					{
						MessageWriter messageWriter3 = MessageWriter.Get(sendOption);
						messageWriter3.CopyFrom(reader);
						this.Broadcast(messageWriter3, client);
						messageWriter3.Recycle();
						return;
					}
				}
				else if (this.GameState == GameStates.Started)
				{
					client.Connection.Dispose();
					return;
				}
				break;
			case 6:
				if (this.Clients.Contains(client))
				{
					if (reader.ReadInt32() == 32)
					{
						int targetId = reader.ReadPackedInt32();
						MessageWriter messageWriter4 = MessageWriter.Get(sendOption);
						messageWriter4.CopyFrom(reader);
						this.SendTo(messageWriter4, targetId);
						messageWriter4.Recycle();
						return;
					}
				}
				else if (this.GameState == GameStates.Started)
				{
					Debug.Log("GameDataTo: Server didn't have client");
					client.Connection.Dispose();
					return;
				}
				break;
			case 8:
				if (reader.ReadInt32() == 32)
				{
					this.EndGame(reader, client);
					return;
				}
				break;
			case 11:
				if (reader.ReadInt32() == 32)
				{
					this.KickPlayer(reader.ReadPackedInt32(), reader.ReadBoolean());
					return;
				}
				break;
			default:
				return;
			}
		}

		// Token: 0x06000D1B RID: 3355 RVA: 0x0003EB4C File Offset: 0x0003CD4C
		private void KickPlayer(int targetId, bool ban)
		{
			List<InnerNetServer.Player> clients = this.Clients;
			lock (clients)
			{
				InnerNetServer.Player player = null;
				for (int i = 0; i < this.Clients.Count; i++)
				{
					if (this.Clients[i].Id == targetId)
					{
						player = this.Clients[i];
						break;
					}
				}
				if (player != null)
				{
					if (ban)
					{
						HashSet<string> obj = this.ipBans;
						lock (obj)
						{
							IPEndPoint endPoint = player.Connection.EndPoint;
							this.ipBans.Add(endPoint.Address.ToString());
						}
					}
					MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
					messageWriter.StartMessage(11);
					messageWriter.Write(32);
					messageWriter.WritePacked(targetId);
					messageWriter.Write(ban);
					messageWriter.EndMessage();
					this.Broadcast(messageWriter, null);
					messageWriter.Recycle();
				}
			}
		}

		// Token: 0x06000D1C RID: 3356 RVA: 0x0003EC60 File Offset: 0x0003CE60
		protected void JoinGame(InnerNetServer.Player client)
		{
			HashSet<string> obj = this.ipBans;
			lock (obj)
			{
				IPEndPoint endPoint = client.Connection.EndPoint;
				if (this.ipBans.Contains(endPoint.Address.ToString()))
				{
					MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
					messageWriter.StartMessage(1);
					messageWriter.Write(6);
					messageWriter.EndMessage();
					client.Connection.Send(messageWriter);
					messageWriter.Recycle();
					return;
				}
			}
			List<InnerNetServer.Player> clients = this.Clients;
			lock (clients)
			{
				GameStates gameState = this.GameState;
				if (gameState != GameStates.NotStarted)
				{
					if (gameState != GameStates.Ended)
					{
						MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
						messageWriter2.StartMessage(1);
						messageWriter2.Write(2);
						messageWriter2.EndMessage();
						client.Connection.Send(messageWriter2);
						messageWriter2.Recycle();
					}
					else
					{
						this.HandleRejoin(client);
					}
				}
				else
				{
					this.HandleNewGameJoin(client);
				}
			}
		}

		// Token: 0x06000D1D RID: 3357 RVA: 0x0003ED74 File Offset: 0x0003CF74
		private void HandleRejoin(InnerNetServer.Player client)
		{
			if (client.Id == this.HostId)
			{
				this.GameState = GameStates.NotStarted;
				this.HandleNewGameJoin(client);
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				for (int i = 0; i < this.Clients.Count; i++)
				{
					InnerNetServer.Player player = this.Clients[i];
					if (player != client)
					{
						try
						{
							this.WriteJoinedMessage(player, messageWriter, true);
							player.Connection.Send(messageWriter);
						}
						catch
						{
						}
					}
				}
				messageWriter.Recycle();
				return;
			}
			if (this.Clients.Count >= 9)
			{
				MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
				messageWriter2.StartMessage(1);
				messageWriter2.Write(1);
				messageWriter2.EndMessage();
				client.Connection.Send(messageWriter2);
				messageWriter2.Recycle();
				return;
			}
			this.Clients.Add(client);
			client.LimboState = LimboStates.WaitingForHost;
			MessageWriter messageWriter3 = MessageWriter.Get(SendOption.Reliable);
			try
			{
				messageWriter3.StartMessage(12);
				messageWriter3.Write(32);
				messageWriter3.Write(client.Id);
				messageWriter3.EndMessage();
				client.Connection.Send(messageWriter3);
				this.BroadcastJoinMessage(client, messageWriter3);
			}
			catch
			{
			}
			finally
			{
				messageWriter3.Recycle();
			}
		}

		// Token: 0x06000D1E RID: 3358 RVA: 0x0003EEB4 File Offset: 0x0003D0B4
		private void HandleNewGameJoin(InnerNetServer.Player client)
		{
			if (this.Clients.Count >= 10)
			{
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				try
				{
					messageWriter.StartMessage(1);
					messageWriter.Write(1);
					messageWriter.EndMessage();
					client.Connection.Send(messageWriter);
				}
				catch
				{
				}
				finally
				{
					messageWriter.Recycle();
				}
				return;
			}
			this.Clients.Add(client);
			client.LimboState = LimboStates.PreSpawn;
			if (this.HostId == -1)
			{
				this.HostId = this.Clients[0].Id;
			}
			if (this.HostId == client.Id)
			{
				client.LimboState = LimboStates.NotLimbo;
			}
			MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
			try
			{
				this.WriteJoinedMessage(client, messageWriter2, true);
				client.Connection.Send(messageWriter2);
				this.BroadcastJoinMessage(client, messageWriter2);
			}
			catch
			{
			}
			finally
			{
				messageWriter2.Recycle();
			}
		}

		// Token: 0x06000D1F RID: 3359 RVA: 0x0003EFB0 File Offset: 0x0003D1B0
		private void EndGame(MessageReader message, InnerNetServer.Player source)
		{
			if (source.Id == this.HostId)
			{
				this.GameState = GameStates.Ended;
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.CopyFrom(message);
				this.Broadcast(messageWriter, null);
				messageWriter.Recycle();
				List<InnerNetServer.Player> clients = this.Clients;
				lock (clients)
				{
					this.Clients.Clear();
					return;
				}
			}
			Debug.LogWarning("Reset request rejected from: " + source.Id);
		}

		// Token: 0x06000D20 RID: 3360 RVA: 0x0003F040 File Offset: 0x0003D240
		private void StartGame(MessageReader message, InnerNetServer.Player source)
		{
			this.GameState = GameStates.Started;
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.CopyFrom(message);
			this.Broadcast(messageWriter, null);
			messageWriter.Recycle();
		}

		// Token: 0x06000D21 RID: 3361 RVA: 0x0003F070 File Offset: 0x0003D270
		private void ClientDisconnect(InnerNetServer.Player client)
		{
			Debug.Log("Server DC client " + client.Id);
			List<InnerNetServer.Player> clients = this.Clients;
			lock (clients)
			{
				this.Clients.Remove(client);
				client.Connection.Dispose();
				if (this.Clients.Count > 0)
				{
					this.HostId = this.Clients[0].Id;
				}
			}
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(4);
			messageWriter.Write(32);
			messageWriter.Write(client.Id);
			messageWriter.Write(this.HostId);
			messageWriter.Write(0);
			messageWriter.EndMessage();
			this.Broadcast(messageWriter, null);
			messageWriter.Recycle();
		}

		// Token: 0x06000D22 RID: 3362 RVA: 0x0003F14C File Offset: 0x0003D34C
		protected void SendTo(MessageWriter msg, int targetId)
		{
			List<InnerNetServer.Player> clients = this.Clients;
			lock (clients)
			{
				for (int i = 0; i < this.Clients.Count; i++)
				{
					InnerNetServer.Player player = this.Clients[i];
					if (player.Id == targetId)
					{
						try
						{
							player.Connection.Send(msg);
							break;
						}
						catch (Exception exception)
						{
							Debug.LogException(exception);
							break;
						}
					}
				}
			}
		}

		// Token: 0x06000D23 RID: 3363 RVA: 0x0003F1D4 File Offset: 0x0003D3D4
		protected void Broadcast(MessageWriter msg, InnerNetServer.Player source)
		{
			List<InnerNetServer.Player> clients = this.Clients;
			lock (clients)
			{
				for (int i = 0; i < this.Clients.Count; i++)
				{
					InnerNetServer.Player player = this.Clients[i];
					if (player != source)
					{
						try
						{
							player.Connection.Send(msg);
						}
						catch
						{
						}
					}
				}
			}
		}

		// Token: 0x06000D24 RID: 3364 RVA: 0x00009DD1 File Offset: 0x00007FD1
		private void BroadcastJoinMessage(InnerNetServer.Player client, MessageWriter msg)
		{
			msg.Clear(SendOption.Reliable);
			msg.StartMessage(1);
			msg.Write(32);
			msg.Write(client.Id);
			msg.Write(this.HostId);
			msg.EndMessage();
			this.Broadcast(msg, client);
		}

		// Token: 0x06000D25 RID: 3365 RVA: 0x0003F254 File Offset: 0x0003D454
		private void WriteJoinedMessage(InnerNetServer.Player client, MessageWriter msg, bool clear)
		{
			if (clear)
			{
				msg.Clear(SendOption.Reliable);
			}
			msg.StartMessage(7);
			msg.Write(32);
			msg.Write(client.Id);
			msg.Write(this.HostId);
			msg.WritePacked(this.Clients.Count - 1);
			for (int i = 0; i < this.Clients.Count; i++)
			{
				InnerNetServer.Player player = this.Clients[i];
				if (player != client)
				{
					msg.WritePacked(player.Id);
				}
			}
			msg.EndMessage();
		}

		// Token: 0x04000C90 RID: 3216
		public const int MaxPlayers = 20;

		// Token: 0x04000C91 RID: 3217
		public bool Running;

		// Token: 0x04000C92 RID: 3218
		public const int LocalGameId = 32;

		// Token: 0x04000C93 RID: 3219
		private const int InvalidHost = -1;

		// Token: 0x04000C94 RID: 3220
		private int HostId = -1;

		// Token: 0x04000C95 RID: 3221
		public HashSet<string> ipBans = new HashSet<string>();

		// Token: 0x04000C96 RID: 3222
		public int Port = 22023;

		// Token: 0x04000C97 RID: 3223
		[SerializeField]
		private GameStates GameState;

		// Token: 0x04000C98 RID: 3224
		private NetworkConnectionListener listener;

		// Token: 0x04000C99 RID: 3225
		private List<InnerNetServer.Player> Clients = new List<InnerNetServer.Player>();

		// Token: 0x0200025F RID: 607
		protected class Player
		{
			// Token: 0x06000D27 RID: 3367 RVA: 0x00009E3F File Offset: 0x0000803F
			public Player(Connection connection)
			{
				this.Id = Interlocked.Increment(ref InnerNetServer.Player.IdCount);
				this.Connection = connection;
			}

			// Token: 0x04000C9A RID: 3226
			private static int IdCount = 1;

			// Token: 0x04000C9B RID: 3227
			public int Id;

			// Token: 0x04000C9C RID: 3228
			public Connection Connection;

			// Token: 0x04000C9D RID: 3229
			public LimboStates LimboState;
		}
	}
}
