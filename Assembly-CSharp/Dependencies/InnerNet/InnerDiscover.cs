using System;
using System.Collections;
using System.Net.Sockets;
using Hazel.Udp;
using UnityEngine;

namespace InnerNet
{
	public class InnerDiscover : DestroyableSingleton<InnerDiscover>
	{
		private UdpBroadcastListener listener;

		private UdpBroadcaster sender;

		public int Port = 47777;

		public float Interval = 1f;

		public event Action<BroadcastPacket> OnPacketGet;

		public void StartAsServer(string data)
		{
			bool num = sender == null;
			if (num)
			{
				sender = new UdpBroadcaster(Port);
			}
			sender.SetData(data);
			if (num)
			{
				StartCoroutine(RunServer());
			}
		}

		private IEnumerator RunServer()
		{
			while (sender != null)
			{
				sender.Broadcast();
				for (float timer = 0f; timer < Interval; timer += Time.deltaTime)
				{
					yield return null;
				}
			}
		}

		public void StopServer()
		{
			if (sender != null)
			{
				sender.Dispose();
				sender = null;
			}
		}

		public void StartAsClient()
		{
			if (listener == null)
			{
				try
				{
					listener = new UdpBroadcastListener(Port);
					listener.StartListen();
					StartCoroutine(RunClient());
				}
				catch (SocketException)
				{
					AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.Custom;
					AmongUsClient.Instance.LastCustomDisconnect = "Couldn't start local network listener. You may need to restart Among Us.";
					DestroyableSingleton<DisconnectPopup>.Instance.Show();
				}
			}
		}

		private IEnumerator RunClient()
		{
			while (listener != null)
			{
				if (!listener.Running)
				{
					listener.StartListen();
				}
				BroadcastPacket[] packets = listener.GetPackets();
				for (int i = 0; i < packets.Length; i++)
				{
					this.OnPacketGet?.Invoke(packets[i]);
				}
				yield return null;
			}
		}

		public void StopClient()
		{
			if (listener != null)
			{
				listener.Dispose();
				listener = null;
			}
		}

		public override void OnDestroy()
		{
			StopServer();
			StopClient();
			base.OnDestroy();
		}
	}
}
