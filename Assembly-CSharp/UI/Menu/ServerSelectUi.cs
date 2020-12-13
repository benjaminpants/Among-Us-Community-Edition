using System.Collections.Generic;
using UnityEngine;

public class ServerSelectUi : MonoBehaviour
{
	public ServerSelector ServerButtonPrefab;

	public Vector3 ScrollStartArea;

	public Scroller Slider;

	public ServerSelector CustomServer;

	private List<ServerSelector> myButtons = new List<ServerSelector>();

	public void Start()
	{
		ServerInfo[] availableServers = DestroyableSingleton<ServerManager>.Instance.availableServers;
		Vector3 scrollStartArea = ScrollStartArea;
		for (int i = 0; i < availableServers.Length; i++)
		{
			if (!(availableServers[i].Name == "Custom"))
			{
				ServerSelector serverSelector = Object.Instantiate(ServerButtonPrefab, Slider.Inner);
				serverSelector.name = availableServers[i].Name;
				myButtons.Add(serverSelector);
				serverSelector.transform.localPosition = scrollStartArea;
				serverSelector.Parent = this;
				serverSelector.MyServer = availableServers[i];
				if (availableServers[i].Default)
				{
					serverSelector.Select();
				}
				scrollStartArea.y -= 0.7f;
			}
		}
		CustomServer.Parent = this;
		myButtons.Add(CustomServer);
		if (DestroyableSingleton<ServerManager>.Instance.LastServer.Name == "Custom")
		{
			CustomServer.Select();
		}
		Slider.YBounds.max = ScrollStartArea.y;
		Slider.YBounds.min = scrollStartArea.y;
	}

	internal void SelectServer(ServerSelector selected)
	{
		for (int i = 0; i < myButtons.Count; i++)
		{
			ServerSelector serverSelector = myButtons[i];
			if (!serverSelector.MyServer.Default)
			{
				serverSelector.Unselect();
			}
		}
	}
}
