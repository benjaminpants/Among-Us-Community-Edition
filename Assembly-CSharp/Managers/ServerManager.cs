using System;
using System.Collections;
using System.IO;
using UnityEngine;

// Token: 0x02000116 RID: 278
public class ServerManager : DestroyableSingleton<ServerManager>
{
	// Token: 0x170000E5 RID: 229
	// (get) Token: 0x060005E6 RID: 1510 RVA: 0x00005B6C File Offset: 0x00003D6C
	public string OnlineNetAddress
	{
		get
		{
			return this.LastServer.Ip;
		}
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00005B79 File Offset: 0x00003D79
	public void Start()
	{
		this.serverInfoFile = Path.Combine(Application.persistentDataPath, "serverInfo.dat");
		this.LastServer = ServerManager.DefaultServer;
		this.availableServers = new ServerInfo[]
		{
			this.LastServer
		};
		this.state = ServerManager.UpdateState.Success;
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x00005BB7 File Offset: 0x00003DB7
	public IEnumerator WaitForServers()
	{
		while (this.state == ServerManager.UpdateState.Connecting)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x040005C5 RID: 1477
	public const string DefaultOnlineServer = "24.181.130.52";

	// Token: 0x040005C6 RID: 1478
	public static readonly ServerInfo DefaultServer = new ServerInfo
	{
		Name = "Primary",
		Ip = "24.181.130.52",
		Default = true
	};

	// Token: 0x040005C7 RID: 1479
	public ServerInfo[] availableServers;

	// Token: 0x040005C8 RID: 1480
	public ServerInfo LastServer = ServerManager.DefaultServer;

	// Token: 0x040005C9 RID: 1481
	private string serverInfoFile;

	// Token: 0x040005CA RID: 1482
	private ServerManager.UpdateState state;

	// Token: 0x02000117 RID: 279
	private enum UpdateState
	{
		// Token: 0x040005CC RID: 1484
		Connecting,
		// Token: 0x040005CD RID: 1485
		Failed,
		// Token: 0x040005CE RID: 1486
		Success
	}
}
