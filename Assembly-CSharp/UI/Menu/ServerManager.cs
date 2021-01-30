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

	public const string DefaultOnlineServer = "172.98.89.233";

	public static readonly ServerInfo DefaultServer;

	public ServerInfo[] availableServers;

	public ServerInfo LastServer = DefaultServer;

	private string serverInfoFile;

	private UpdateState state;

    public string OnlineNetAddress => LastServer.Ip;
	public int LastPort => LastServer.Port;

	public void Start()
	{
		serverInfoFile = Path.Combine(Application.persistentDataPath, "serverInfo.dat");
        LastServer = DefaultServer;
        /*ServerInfo localfo = new ServerInfo
		{
			Name = "Playtesters Only",
			Ip = "24.181.130.52",
			Port = 25565,
			Default = false
		};*/
        try
        {
            if (!File.Exists(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json")))
            {
                availableServers = new ServerInfo[1]
                {
                LastServer
                };
            }
            else
            {
                availableServers = JsonConvert.DeserializeObject<ServerInfo[]>(File.ReadAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json")));
            }
        }
        catch (Exception E)
        {
            Debug.LogError(E.Message);
            state = UpdateState.Failed;
            return;
        }
		try
		{
			FileStream json = File.Create(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json"));
			json.Close();
			File.WriteAllText(Path.Combine(CE_Extensions.GetGameDirectory(), "servers.json"), JsonConvert.SerializeObject(availableServers, Formatting.Indented));
		}
		catch (Exception E)
		{
			Debug.LogError(E.Message);
			state = UpdateState.Failed;
			return;
		}
		state = UpdateState.Success;
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
			Name = "Primary",
			Ip = "172.98.89.233",
			Port = 27485,
			Default = true,
			Icon = "globe.png"
		};
	}
}
