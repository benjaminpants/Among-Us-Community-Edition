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
			Icon = "single_crewmate.png"
		};
        try
        {
            if (!File.Exists(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json")))
            {
                availableServers = new ServerInfo[2]
                {
                LastServer,
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
			Ip = "172.98.89.239",
			Port = 41855,
			Default = true,
			Icon = "globe.png"
		};
	}
}
