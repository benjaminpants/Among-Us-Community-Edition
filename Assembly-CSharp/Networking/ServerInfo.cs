using System.IO;
using System.Net;

public class ServerInfo
{
	public string Name = "Custom";

	public string Ip = "0.0.0.0";

	public int Port = 25565;

	public string Icon = "skeld.png";

	public bool Default;

	public void Serialize(BinaryWriter writer)
	{
		writer.Write(Name);
		writer.Write(Ip);
		writer.Write(Port);
		writer.Write(Default);
	}

	public static ServerInfo Deserialize(BinaryReader reader)
	{
		ServerInfo serverInfo = new ServerInfo();
		serverInfo.Name = reader.ReadString();
		serverInfo.Ip = reader.ReadString();
		serverInfo.Port = reader.ReadInt32();
		if (!IPAddress.TryParse(serverInfo.Ip, out var _))
		{
			return null;
		}
		serverInfo.Default = reader.ReadBoolean();
		return serverInfo;
	}

	internal static ServerInfo Deserialize(string[] parts)
	{
		return new ServerInfo
		{
			Name = parts[0],
			Ip = parts[1],
			Default = bool.Parse(parts[2])
		};
	}
}
