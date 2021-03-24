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
	public class InnerNetServer : DestroyableSingleton<InnerNetServer>
	{
		protected class Player
		{
			private static int IdCount;

			public int Id;

			public Connection Connection;

			public LimboStates LimboState;

			public Player(Connection connection)
			{
				Id = Interlocked.Increment(ref IdCount);
				Connection = connection;
			}

			static Player()
			{
				IdCount = 1;
			}
		}

		public const int MaxPlayers = 20;

		public bool Running;

		public const int LocalGameId = 32;

		private const int InvalidHost = -1;

		private int HostId = -1;

		public HashSet<string> ipBans = new HashSet<string>();

		public int Port = 22023;

		[SerializeField]
		private GameStates GameState;

		private NetworkConnectionListener listener;

		private List<Player> Clients = new List<Player>();

		public GameStates CurrentGState()
        {
			return GameState;
        }

		public override void OnDestroy()
		{
			StopServer();
			base.OnDestroy();
		}

		public void StartAsServer()
		{
			if (listener != null)
			{
				StopServer();
			}
			GameState = GameStates.NotStarted;
			listener = new UdpConnectionListener(new IPEndPoint(IPAddress.Any, Port));
			listener.NewConnection += OnServerConnect;
			listener.Start();
			Running = true;
		}

		public void StopServer()
		{
			HostId = -1;
			Running = false;
			GameState = GameStates.Destroyed;
			if (listener != null)
			{
				listener.Close();
				listener.Dispose();
				listener = null;
			}
			lock (Clients)
			{
				Clients.Clear();
			}
		}

		public static bool IsCompatibleVersion(int version)
		{
			return Constants.CompatVersions.Contains(version);
		}

		private void OnServerConnect(NewConnectionEventArgs evt)
		{
			MessageReader handshakeData = evt.HandshakeData;
			try
			{
				if (evt.HandshakeData.Length < 5)
				{
					SendIncorrectVersion(evt.Connection);
					return;
				}
				if (!IsCompatibleVersion(handshakeData.ReadInt32()))
				{
					SendIncorrectVersion(evt.Connection);
					return;
				}
			}
			finally
			{
				handshakeData.Recycle();
			}
			Player client = new Player(evt.Connection);
			Debug.Log($"Client {client.Id} added: {evt.Connection.EndPoint}");
			UdpConnection obj = (UdpConnection)evt.Connection;
			obj.KeepAliveInterval = 1500;
			obj.DisconnectTimeout = 6000;
			obj.ResendPingMultiplier = 1.5f;
			obj.DataReceived += delegate(DataReceivedEventArgs e)
			{
				OnDataReceived(client, e);
			};
			obj.Disconnected += delegate
			{
				ClientDisconnect(client);
			};
		}

		private static void SendIncorrectVersion(Connection connection)
		{
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(1);
			messageWriter.Write(5);
			messageWriter.EndMessage();
			connection.Send(messageWriter);
			messageWriter.Recycle();
		}

		private void Connection_DataSentRaw(byte[] data, int length)
		{
			Debug.Log("Server Sent: " + string.Join(" ", data.Select(delegate(byte b)
			{
				byte b2 = b;
				return b2.ToString();
			}).ToArray(), 0, length));
		}

		private void OnDataReceived(Player client, DataReceivedEventArgs evt)
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
					HandleMessage(client, message.ReadMessage(), evt.SendOption);
				}
			}
			catch (Exception arg)
			{
				Debug.Log(string.Format("{0}\r\n{1}", string.Join(" ", message.Buffer), arg));
			}
			finally
			{
				message.Recycle();
			}
		}

		private void HandleMessage(Player client, MessageReader reader, SendOption sendOption)
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
				break;
			}
			case 1:
			{
				Debug.Log("Server got join game");
				if (reader.ReadInt32() == 32)
				{
					if ((reader.ReadInt32() == CE_LuaLoader.TheOmegaHash && reader.ReadInt32() == CE_WardrobeManager.HatHash) && CE_ModLoader.ColorHash == reader.ReadInt32())
					{
						if (reader.ReadInt32() == VersionShower.buildhash)
						{
							JoinGame(client);
						}
						else
                        {
								MessageWriter messageWriter5 = MessageWriter.Get(SendOption.Reliable);
								messageWriter5.StartMessage(1);
								messageWriter5.Write(8);
								messageWriter5.Write("Your version doesn't match with the hosts! Please update your game(or downgrade it) to join!");
								messageWriter5.EndMessage();
								client.Connection.Send(messageWriter5);
								messageWriter5.Recycle();
						}
						break;
					}
					else
					{
						MessageWriter messageWriter5 = MessageWriter.Get(SendOption.Reliable);
						messageWriter5.StartMessage(1);
                        messageWriter5.Write(8);
						messageWriter5.Write("Hash didn't match with Host's!\nThis could be because your Cosmetics and Lua are out of date or you have extra Cosmetics and Gamemodes installed.");
						messageWriter5.EndMessage();
						client.Connection.Send(messageWriter5);
						messageWriter5.Recycle();
						break;
					}
				}
				MessageWriter messageWriter4 = MessageWriter.Get(SendOption.Reliable);
				messageWriter4.StartMessage(1);
				messageWriter4.Write(3);
				messageWriter4.EndMessage();
				client.Connection.Send(messageWriter4);
				messageWriter4.Recycle();
				break;
			}
			case 2:
				if (reader.ReadInt32() == 32)
				{
					StartGame(reader, client);
				}
				break;
			case 3:
				if (reader.ReadInt32() == 32)
				{
					ClientDisconnect(client);
				}
				break;
			case 5:
				if (Clients.Contains(client))
				{
					if (reader.ReadInt32() == 32)
					{
						MessageWriter messageWriter2 = MessageWriter.Get(sendOption);
						messageWriter2.CopyFrom(reader);
						Broadcast(messageWriter2, client);
						messageWriter2.Recycle();
					}
				}
				else if (GameState == GameStates.Started)
				{
					client.Connection.Dispose();
				}
				break;
			case 6:
				if (Clients.Contains(client))
				{
					if (reader.ReadInt32() == 32)
					{
						int targetId = reader.ReadPackedInt32();
						MessageWriter messageWriter3 = MessageWriter.Get(sendOption);
						messageWriter3.CopyFrom(reader);
						SendTo(messageWriter3, targetId);
						messageWriter3.Recycle();
					}
				}
				else if (GameState == GameStates.Started)
				{
					Debug.Log("GameDataTo: Server didn't have client");
					client.Connection.Dispose();
				}
				break;
			case 8:
				if (reader.ReadInt32() == 32)
				{
					EndGame(reader, client);
				}
                    break;
			case 14: //duplicate of 8, no need for change as the server just relays the message.
				if (reader.ReadInt32() == 32)
				{
					EndGame(reader, client);
				}
				break;
				case 11:
				if (reader.ReadInt32() == 32)
				{
					KickPlayer(reader.ReadPackedInt32(), reader.ReadBoolean());
				}
				break;
			case 4:
			case 7:
			case 9:
			case 10:
				break;
			}
		}

		private void KickPlayer(int targetId, bool ban)
		{
			lock (Clients)
			{
				Player player = null;
				for (int i = 0; i < Clients.Count; i++)
				{
					if (Clients[i].Id == targetId)
					{
						player = Clients[i];
						break;
					}
				}
				if (player == null)
				{
					return;
				}
				if (ban)
				{
					lock (ipBans)
					{
						IPEndPoint endPoint = player.Connection.EndPoint;
						ipBans.Add(endPoint.Address.ToString());
					}
				}
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.StartMessage(11);
				messageWriter.Write(32);
				messageWriter.WritePacked(targetId);
				messageWriter.Write(ban);
				messageWriter.EndMessage();
				Broadcast(messageWriter, null);
				messageWriter.Recycle();
			}
		}

		protected void JoinGame(Player client)
		{
			lock (ipBans)
			{
				IPEndPoint endPoint = client.Connection.EndPoint;
				if (ipBans.Contains(endPoint.Address.ToString()))
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
			lock (Clients)
			{
				switch (GameState)
				{
				case GameStates.NotStarted:
					HandleNewGameJoin(client);
					return;
				case GameStates.Ended:
					HandleRejoin(client);
					return;
				}
				MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
				messageWriter2.StartMessage(1);
				messageWriter2.Write(2);
				messageWriter2.EndMessage();
				client.Connection.Send(messageWriter2);
				messageWriter2.Recycle();
			}
		}

		private void HandleRejoin(Player client)
		{
			if (client.Id == HostId)
			{
				GameState = GameStates.NotStarted;
				HandleNewGameJoin(client);
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				for (int i = 0; i < Clients.Count; i++)
				{
					Player player = Clients[i];
					if (player != client)
					{
						try
						{
							WriteJoinedMessage(player, messageWriter, clear: true);
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
			if (Clients.Count >= 9)
			{
				MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
				messageWriter2.StartMessage(1);
				messageWriter2.Write(1);
				messageWriter2.EndMessage();
				client.Connection.Send(messageWriter2);
				messageWriter2.Recycle();
				return;
			}
			Clients.Add(client);
			client.LimboState = LimboStates.WaitingForHost;
			MessageWriter messageWriter3 = MessageWriter.Get(SendOption.Reliable);
			try
			{
				messageWriter3.StartMessage(12);
				messageWriter3.Write(32);
				messageWriter3.Write(client.Id);
				messageWriter3.EndMessage();
				client.Connection.Send(messageWriter3);
				BroadcastJoinMessage(client, messageWriter3);
			}
			catch
			{
			}
			finally
			{
				messageWriter3.Recycle();
			}
		}

		private void HandleNewGameJoin(Player client)
		{
			if (Clients.Count >= 20)
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
			Clients.Add(client);
			client.LimboState = LimboStates.PreSpawn;
			if (HostId == -1)
			{
				HostId = Clients[0].Id;
			}
			if (HostId == client.Id)
			{
				client.LimboState = LimboStates.NotLimbo;
			}
			MessageWriter messageWriter2 = MessageWriter.Get(SendOption.Reliable);
			try
			{
				WriteJoinedMessage(client, messageWriter2, clear: true);
				client.Connection.Send(messageWriter2);
				BroadcastJoinMessage(client, messageWriter2);
			}
			catch
			{
			}
			finally
			{
				messageWriter2.Recycle();
			}
		}

		private void EndGame(MessageReader message, Player source)
		{
			if (source.Id == HostId)
			{
				GameState = GameStates.Ended;
				MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
				messageWriter.CopyFrom(message);
				Broadcast(messageWriter, null);
				messageWriter.Recycle();
				lock (Clients)
				{
					Clients.Clear();
				}
			}
			else
			{
				Debug.LogWarning("Reset request rejected from: " + source.Id);
			}
		}

		private void StartGame(MessageReader message, Player source)
		{
			GameState = GameStates.Started;
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.CopyFrom(message);
			Broadcast(messageWriter, null);
			messageWriter.Recycle();
		}

		private void ClientDisconnect(Player client)
		{
			Debug.Log("Server DC client " + client.Id);
			lock (Clients)
			{
				Clients.Remove(client);
				client.Connection.Dispose();
				if (Clients.Count > 0)
				{
					HostId = Clients[0].Id;
				}
			}
			MessageWriter messageWriter = MessageWriter.Get(SendOption.Reliable);
			messageWriter.StartMessage(4);
			messageWriter.Write(32);
			messageWriter.Write(client.Id);
			messageWriter.Write(HostId);
			messageWriter.Write(0);
			messageWriter.EndMessage();
			Broadcast(messageWriter, null);
			messageWriter.Recycle();
		}

		protected void SendTo(MessageWriter msg, int targetId)
		{
			lock (Clients)
			{
				for (int i = 0; i < Clients.Count; i++)
				{
					Player player = Clients[i];
					if (player.Id == targetId)
					{
						try
						{
							player.Connection.Send(msg);
						}
						catch (Exception exception)
						{
							Debug.LogException(exception);
						}
						break;
					}
				}
			}
		}

		protected void Broadcast(MessageWriter msg, Player source)
		{
			lock (Clients)
			{
				for (int i = 0; i < Clients.Count; i++)
				{
					Player player = Clients[i];
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

		private void BroadcastJoinMessage(Player client, MessageWriter msg)
		{
			msg.Clear(SendOption.Reliable);
			msg.StartMessage(1);
			msg.Write(32);
			msg.Write(client.Id);
			msg.Write(HostId);
			msg.EndMessage();
			Broadcast(msg, client);
		}

		private void WriteJoinedMessage(Player client, MessageWriter msg, bool clear)
		{
			if (clear)
			{
				msg.Clear(SendOption.Reliable);
			}
			msg.StartMessage(7);
			msg.Write(32);
			msg.Write(client.Id);
            msg.Write(HostId);
			msg.WritePacked(Clients.Count - 1);
			for (int i = 0; i < Clients.Count; i++)
			{
				Player player = Clients[i];
				if (player != client)
				{
					msg.WritePacked(player.Id);
				}
			}
			msg.EndMessage();
		}
	}
}
