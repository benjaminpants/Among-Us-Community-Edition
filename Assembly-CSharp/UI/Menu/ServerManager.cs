using System.Collections;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using System;

public class ServerManager : DestroyableSingleton<ServerManager>
{
	private enum UpdateState
	{
		Connecting,
		Failed,
		Success
	}

	public const string DefaultOnlineServer = "no";

    public static int LanGamePort = 22023;

	public static string LanGameIP = "DEFAULT";

	public static readonly ServerInfo DefaultServer;

	public ServerInfo[] availableServers;

	public ServerInfo LastServer = DefaultServer;

	private UpdateState state;

    public string OnlineNetAddress => LastServer.Ip;
	public int LastPort => LastServer.Port;

	public void Start()
	{
        LastServer = DefaultServer;
        ServerInfo localfo = new ServerInfo
        {
            Name = "Local Host",
            Ip = "127.0.0.1",
            Port = 25565,
            Default = false,
            Icon = "vent.png"
        };
		ServerInfo eurofo = new ServerInfo
		{
			Name = "Primary [FFFF00FF](France)[]",
			Ip = "amogus",
			Port = 696969,
			Default = true,
			Icon = "globe.png"
		};
		/*if (!File.Exists(Path.Combine(CE_Extensions.GetGameDirectory(), "landata.txt")))
        {
			try
			{
				File.Create(Path.Combine(CE_Extensions.GetGameDirectory(), "landata.txt")).Close();
				File.WriteAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "landata.txt"), LanGameIP + "\n" + LanGamePort);
			}
			catch
            {
				Debug.LogError("Failed to create landata.txt!");
            }
		}
		else
        {
			string[] lines = File.ReadAllLines(Path.Combine(CE_Extensions.GetGameDirectory(), "landata.txt"));
			try
            {
                LanGamePort = Int32.Parse(lines[1]);
				LanGameIP = lines[0];

			}
			catch
            {
				
            }
        }*/
		try
        {
            if (!File.Exists(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json")))
            {
                availableServers = new ServerInfo[2]
                {
				DefaultServer,
                localfo
                };
				FileStream json = File.Create(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json"));
				json.Close();
				File.WriteAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json"), JsonConvert.SerializeObject(availableServers, Formatting.Indented));
			}
            else
            {
                availableServers = JsonConvert.DeserializeObject<ServerInfo[]>(File.ReadAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json")));
				LastServer = Array.Find<ServerInfo>(availableServers,IsDefault);
            }
        }
        catch (Exception E)
        {
            Debug.LogError(E.Message);
            state = UpdateState.Failed;
            return;
        }
		state = UpdateState.Success;
	}


	public static bool IsDefault(ServerInfo si)
	{
		return si.Default;
	}
	public IEnumerator WaitForServers()
	{
		while (state == UpdateState.Connecting)
		{
			yield return null;
		}
	}

	static ServerManager()
	{
		DefaultServer = new ServerInfo
		{
			Name = "Primary [FFFF00FF](Canada)[]",
			Ip = "162.216.47.184",
			Port = 38837,
			Default = true,
			Icon = "vent.png"
		};
	}
}
