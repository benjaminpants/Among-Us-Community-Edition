using System;
using System.IO;
using System.Net;

// Token: 0x02000115 RID: 277
public class ServerInfo
{
	// Token: 0x060005E2 RID: 1506 RVA: 0x00005B01 File Offset: 0x00003D01
	public void Serialize(BinaryWriter writer)
	{
		writer.Write(this.Name);
		writer.Write(this.Ip);
		writer.Write(this.Default);
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00024D14 File Offset: 0x00022F14
	public static ServerInfo Deserialize(BinaryReader reader)
	{
		ServerInfo serverInfo = new ServerInfo();
		serverInfo.Name = reader.ReadString();
		serverInfo.Ip = reader.ReadString();
		IPAddress ipaddress;
		if (!IPAddress.TryParse(serverInfo.Ip, out ipaddress))
		{
			return null;
		}
		serverInfo.Default = reader.ReadBoolean();
		return serverInfo;
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x00005B27 File Offset: 0x00003D27
	internal static ServerInfo Deserialize(string[] parts)
	{
		return new ServerInfo
		{
			Name = parts[0],
			Ip = parts[1],
			Default = bool.Parse(parts[2])
		};
	}

	// Token: 0x040005C2 RID: 1474
	public string Name = "Custom";

	// Token: 0x040005C3 RID: 1475
	public string Ip = "0.0.0.0";

	// Token: 0x040005C4 RID: 1476
	public bool Default;
}
