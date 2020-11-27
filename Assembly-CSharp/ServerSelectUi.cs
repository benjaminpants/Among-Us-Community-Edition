using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001B8 RID: 440
public class ServerSelectUi : MonoBehaviour
{
	// Token: 0x06000989 RID: 2441 RVA: 0x00031AA8 File Offset: 0x0002FCA8
	public void Start()
	{
		ServerInfo[] availableServers = DestroyableSingleton<ServerManager>.Instance.availableServers;
		Vector3 scrollStartArea = this.ScrollStartArea;
		for (int i = 0; i < availableServers.Length; i++)
		{
			if (!(availableServers[i].Name == "Custom"))
			{
				ServerSelector serverSelector = UnityEngine.Object.Instantiate<ServerSelector>(this.ServerButtonPrefab, this.Slider.Inner);
				serverSelector.name = availableServers[i].Name;
				this.myButtons.Add(serverSelector);
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
		this.CustomServer.Parent = this;
		this.myButtons.Add(this.CustomServer);
		if (DestroyableSingleton<ServerManager>.Instance.LastServer.Name == "Custom")
		{
			this.CustomServer.Select();
		}
		this.Slider.YBounds.max = this.ScrollStartArea.y;
		this.Slider.YBounds.min = scrollStartArea.y;
	}

	// Token: 0x0600098A RID: 2442 RVA: 0x00031BD0 File Offset: 0x0002FDD0
	internal void SelectServer(ServerSelector selected)
	{
		for (int i = 0; i < this.myButtons.Count; i++)
		{
			ServerSelector serverSelector = this.myButtons[i];
			if (!serverSelector.MyServer.Default)
			{
				serverSelector.Unselect();
			}
		}
	}

	// Token: 0x04000915 RID: 2325
	public ServerSelector ServerButtonPrefab;

	// Token: 0x04000916 RID: 2326
	public Vector3 ScrollStartArea;

	// Token: 0x04000917 RID: 2327
	public Scroller Slider;

	// Token: 0x04000918 RID: 2328
	public ServerSelector CustomServer;

	// Token: 0x04000919 RID: 2329
	private List<ServerSelector> myButtons = new List<ServerSelector>();
}
