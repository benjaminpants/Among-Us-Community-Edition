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
	// Token: 0x02000141 RID: 321
	public class InnerNetServer : DestroyableSingleton<InnerNetServer>
	{
		// Token: 0x060007D1 RID: 2001 RVA: 0x0002E1F9 File Offset: 0x0002C3F9
		public override void OnDestroy()
		{
			this.StopServer();
			base.OnDestroy();
		}

		// Token: 0x060007D2 RID: 2002 RVA: 0x0002E208 File Offset: 0x0002C408
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

		// Token: 0x060007D3 RID: 2003 RVA: 0x0002E270 File Offset: 0x0002C470
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

		// Token: 0x060007D4 RID: 2004 RVA: 0x0002E2F0 File Offset: 0x0002C4F0
		public static bool IsCompatibleVersion(int version)
		{
			return Constants.CompatVersions.Contains(version);
		}

		// Token: 0x060007D5 RID: 2005 RVA: 0x0002E300 File Offset: 0x0002C500
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
			udpConnection.DataReceived += delegate (DataReceivedEventArgs e)
			{
				this.OnDataReceived(client, e);
			};
			udpConnection.Disconnected += delegate (object o, DisconnectedEventArgs e)
			{
				this.ClientDisconnect(client);
			};
		}

		// Token: 0x060007D6 RID: 2006 RVA: 0x0002E404 File Offset: 0x0002C604
		private static void SendIncorrectVersion(Connection connection)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(1);
			messageWriter.Write(5);
			messageWriter.EndMessage();
			connection.Send(messageWriter);
			messageWriter.Recycle();
		}

		// Token: 0x060007D7 RID: 2007 RVA: 0x0002E43C File Offset: 0x0002C63C
		private void Connection_DataSentRaw(byte[] data, int length)
		{
			Debug.Log("Server Sent: " + string.Join(" ", (from b in data
														  select b.ToString()).ToArray<string>(), 0, length));
		}

		// Token: 0x060007D8 RID: 2008 RVA: 0x0002E490 File Offset: 0x0002C690
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

		// Token: 0x060007D9 RID: 2009 RVA: 0x0002E52C File Offset: 0x0002C72C
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
					}
					break;
				default:
					return;
			}
		}

		// Token: 0x060007DA RID: 2010 RVA: 0x0002E70C File Offset: 0x0002C90C
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

		// Token: 0x060007DB RID: 2011 RVA: 0x0002E820 File Offset: 0x0002CA20
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
				switch (this.GameState)
				{
					case GameStates.NotStarted:
						this.HandleNewGameJoin(client);
						break;
					default:
						{
							MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
							messageWriter2.StartMessage(1);
							messageWriter2.Write(2);
							messageWriter2.EndMessage();
							client.Connection.Send(messageWriter2);
							messageWriter2.Recycle();
							break;
						}
					case GameStates.Ended:
						this.HandleRejoin(client);
						break;
				}
			}
		}

		// Token: 0x060007DC RID: 2012 RVA: 0x0002E93C File Offset: 0x0002CB3C
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

		// Token: 0x060007DD RID: 2013 RVA: 0x0002EA7C File Offset: 0x0002CC7C
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

		// Token: 0x060007DE RID: 2014 RVA: 0x0002EB78 File Offset: 0x0002CD78
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

		// Token: 0x060007DF RID: 2015 RVA: 0x0002EC08 File Offset: 0x0002CE08
		private void StartGame(MessageReader message, InnerNetServer.Player source)
		{
			this.GameState = GameStates.Started;
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.CopyFrom(message);
			this.Broadcast(messageWriter, null);
			messageWriter.Recycle();
		}

		// Token: 0x060007E0 RID: 2016 RVA: 0x0002EC38 File Offset: 0x0002CE38
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

		// Token: 0x060007E1 RID: 2017 RVA: 0x0002ED14 File Offset: 0x0002CF14
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

		// Token: 0x060007E2 RID: 2018 RVA: 0x0002ED9C File Offset: 0x0002CF9C
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

		// Token: 0x060007E3 RID: 2019 RVA: 0x0002EE1C File Offset: 0x0002D01C
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

		// Token: 0x060007E4 RID: 2020 RVA: 0x0002EE5C File Offset: 0x0002D05C
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

		// Token: 0x040007DC RID: 2012
		public const int MaxPlayers = 10;

		// Token: 0x040007DD RID: 2013
		public bool Running;

		// Token: 0x040007DE RID: 2014
		public const int LocalGameId = 32;

		// Token: 0x040007DF RID: 2015
		private const int InvalidHost = -1;

		// Token: 0x040007E0 RID: 2016
		private int HostId = -1;

		// Token: 0x040007E1 RID: 2017
		public HashSet<string> ipBans = new HashSet<string>();

		// Token: 0x040007E2 RID: 2018
		public int Port = 22023;

		// Token: 0x040007E3 RID: 2019
		[SerializeField]
		private GameStates GameState;

		// Token: 0x040007E4 RID: 2020
		private NetworkConnectionListener listener;

		// Token: 0x040007E5 RID: 2021
		private List<InnerNetServer.Player> Clients = new List<InnerNetServer.Player>();

		// Token: 0x0200025A RID: 602
		protected class Player
		{
			// Token: 0x06000DAC RID: 3500 RVA: 0x0003D545 File Offset: 0x0003B745
			public Player(Connection connection)
			{
				this.Id = Interlocked.Increment(ref InnerNetServer.Player.IdCount);
				this.Connection = connection;
			}

			// Token: 0x04000C30 RID: 3120
			private static int IdCount = 1;

			// Token: 0x04000C31 RID: 3121
			public int Id;

			// Token: 0x04000C32 RID: 3122
			public Connection Connection;

			// Token: 0x04000C33 RID: 3123
			public LimboStates LimboState;
		}
	}
}
