using System;
using System.Collections;
using System.Net.Sockets;
using Hazel.Udp;
using UnityEngine;

namespace InnerNet
{
	// Token: 0x02000241 RID: 577
	public class InnerDiscover : DestroyableSingleton<InnerDiscover>
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000C5D RID: 3165 RVA: 0x0003C1A0 File Offset: 0x0003A3A0
		// (remove) Token: 0x06000C5E RID: 3166 RVA: 0x0003C1D8 File Offset: 0x0003A3D8
		public event Action<BroadcastPacket> OnPacketGet;

		// Token: 0x06000C5F RID: 3167 RVA: 0x000096CC File Offset: 0x000078CC
		public void StartAsServer(string data)
		{
			bool flag = this.sender == null;
			if (flag)
			{
				this.sender = new UdpBroadcaster(this.Port);
			}
			this.sender.SetData(data);
			if (flag)
			{
				base.StartCoroutine(this.RunServer());
			}
		}

		// Token: 0x06000C60 RID: 3168 RVA: 0x00009706 File Offset: 0x00007906
		private IEnumerator RunServer()
		{
			while (this.sender != null)
			{
				this.sender.Broadcast();
				for (float timer = 0f; timer < this.Interval; timer += Time.deltaTime)
				{
					yield return null;
				}
			}
			yield break;
		}

		// Token: 0x06000C61 RID: 3169 RVA: 0x00009715 File Offset: 0x00007915
		public void StopServer()
		{
			if (this.sender != null)
			{
				this.sender.Dispose();
				this.sender = null;
			}
		}

		// Token: 0x06000C62 RID: 3170 RVA: 0x0003C210 File Offset: 0x0003A410
		public void StartAsClient()
		{
			if (this.listener == null)
			{
				try
				{
					this.listener = new UdpBroadcastListener(this.Port);
					this.listener.StartListen();
					base.StartCoroutine(this.RunClient());
				}
				catch (SocketException)
				{
					AmongUsClient.Instance.LastDisconnectReason = DisconnectReasons.Custom;
					AmongUsClient.Instance.LastCustomDisconnect = "Couldn't start local network listener. You may need to restart Among Us.";
					DestroyableSingleton<DisconnectPopup>.Instance.Show();
				}
			}
		}

		// Token: 0x06000C63 RID: 3171 RVA: 0x00009731 File Offset: 0x00007931
		private IEnumerator RunClient()
		{
			while (this.listener != null)
			{
				if (!this.listener.Running)
				{
					this.listener.StartListen();
				}
				BroadcastPacket[] packets = this.listener.GetPackets();
				for (int i = 0; i < packets.Length; i++)
				{
					Action<BroadcastPacket> onPacketGet = this.OnPacketGet;
					if (onPacketGet != null)
					{
						onPacketGet(packets[i]);
					}
				}
				yield return null;
			}
			yield break;
		}

		// Token: 0x06000C64 RID: 3172 RVA: 0x00009740 File Offset: 0x00007940
		public void StopClient()
		{
			if (this.listener != null)
			{
				this.listener.Dispose();
				this.listener = null;
			}
		}

		// Token: 0x06000C65 RID: 3173 RVA: 0x0000975C File Offset: 0x0000795C
		public override void OnDestroy()
		{
			this.StopServer();
			this.StopClient();
			base.OnDestroy();
		}

		// Token: 0x04000BF3 RID: 3059
		private UdpBroadcastListener listener;

		// Token: 0x04000BF4 RID: 3060
		private UdpBroadcaster sender;

		// Token: 0x04000BF5 RID: 3061
		public int Port = 47777;

		// Token: 0x04000BF6 RID: 3062
		public float Interval = 1f;
	}
}
