using System;
using System.Collections.Generic;
using System.Linq;
using Hazel.Udp;
using InnerNet;
using UnityEngine;

// Token: 0x0200013A RID: 314
public class GameDiscovery : MonoBehaviour
{
	// Token: 0x060006A2 RID: 1698 RVA: 0x00006249 File Offset: 0x00004449
	public void Start()
	{
		InnerDiscover component = base.GetComponent<InnerDiscover>();
		component.OnPacketGet += this.Receive;
		component.StartAsClient();
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x00027474 File Offset: 0x00025674
	public void Update()
	{
		float time = Time.time;
		string[] array = this.received.Keys.ToArray<string>();
		int num = 0;
		foreach (string key in array)
		{
			JoinGameButton joinGameButton = this.received[key];
			if (time - joinGameButton.timeRecieved > 3f)
			{
				this.received.Remove(key);
				UnityEngine.Object.Destroy(joinGameButton.gameObject);
			}
			else
			{
				joinGameButton.transform.localPosition = new Vector3(0f, this.YStart + (float)num * this.YOffset, -1f);
				num++;
			}
		}
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x00027518 File Offset: 0x00025718
	private void Receive(BroadcastPacket packet)
	{
		string[] array = packet.Data.Split(new char[]
		{
			'~'
		});
		string address = packet.GetAddress();
		JoinGameButton joinGameButton;
		if (this.received.TryGetValue(address, out joinGameButton))
		{
			joinGameButton.timeRecieved = Time.time;
			joinGameButton.SetGameName(array);
			return;
		}
		if (array[1].Equals("Open"))
		{
			this.CreateButtonForAddess(address, array);
		}
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x00027580 File Offset: 0x00025780
	private void CreateButtonForAddess(string fromAddress, string[] gameNameParts)
	{
		JoinGameButton joinGameButton;
		if (this.received.TryGetValue(fromAddress, out joinGameButton))
		{
			UnityEngine.Object.Destroy(joinGameButton.gameObject);
		}
		JoinGameButton joinGameButton2 = UnityEngine.Object.Instantiate<JoinGameButton>(this.ButtonPrefab, this.ItemLocation);
		joinGameButton2.transform.localPosition = new Vector3(0f, this.YStart + (float)(this.ItemLocation.childCount - 1) * this.YOffset, -1f);
		joinGameButton2.netAddress = fromAddress;
		joinGameButton2.timeRecieved = Time.time;
		joinGameButton2.SetGameName(gameNameParts);
		joinGameButton2.GetComponentInChildren<MeshRenderer>().material.SetInt("_Mask", 4);
		this.received[fromAddress] = joinGameButton2;
	}

	// Token: 0x04000663 RID: 1635
	public JoinGameButton ButtonPrefab;

	// Token: 0x04000664 RID: 1636
	public Transform ItemLocation;

	// Token: 0x04000665 RID: 1637
	public float YStart = 0.56f;

	// Token: 0x04000666 RID: 1638
	public float YOffset = -0.75f;

	// Token: 0x04000667 RID: 1639
	private Dictionary<string, JoinGameButton> received = new Dictionary<string, JoinGameButton>();
}
