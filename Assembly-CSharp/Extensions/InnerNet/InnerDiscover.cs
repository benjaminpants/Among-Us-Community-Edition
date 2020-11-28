using System;
using System.Collections;
using System.Net.Sockets;
using Hazel.Udp;
using UnityEngine;

namespace InnerNet
{
	// Token: 0x02000137 RID: 311
	public class InnerDiscover : DestroyableSingleton<InnerDiscover>
	{
		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000761 RID: 1889 RVA: 0x0002BF70 File Offset: 0x0002A170
		// (remove) Token: 0x06000762 RID: 1890 RVA: 0x0002BFA8 File Offset: 0x0002A1A8
		public event Action<BroadcastPacket> OnPacketGet;

		// Token: 0x06000763 RID: 1891 RVA: 0x0002BFDD File Offset: 0x0002A1DD
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

		// Token: 0x06000764 RID: 1892 RVA: 0x0002C017 File Offset: 0x0002A217
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

		// Token: 0x06000765 RID: 1893 RVA: 0x0002C026 File Offset: 0x0002A226
		public void StopServer()
		{
			if (this.sender != null)
			{
				this.sender.Dispose();
				this.sender = null;
			}
		}

		// Token: 0x06000766 RID: 1894 RVA: 0x0002C044 File Offset: 0x0002A244
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

		// Token: 0x06000767 RID: 1895 RVA: 0x0002C0BC File Offset: 0x0002A2BC
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

		// Token: 0x06000768 RID: 1896 RVA: 0x0002C0CB File Offset: 0x0002A2CB
		public void StopClient()
		{
			if (this.listener != null)
			{
				this.listener.Dispose();
				this.listener = null;
			}
		}

		// Token: 0x06000769 RID: 1897 RVA: 0x0002C0E7 File Offset: 0x0002A2E7
		public override void OnDestroy()
		{
			this.StopServer();
			this.StopClient();
			base.OnDestroy();
		}

		// Token: 0x0400077E RID: 1918
		private UdpBroadcastListener listener;

		// Token: 0x0400077F RID: 1919
		private UdpBroadcaster sender;

		// Token: 0x04000780 RID: 1920
		public int Port = 47777;

		// Token: 0x04000781 RID: 1921
		public float Interval = 1f;
	}
}
