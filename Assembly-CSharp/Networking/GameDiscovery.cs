using System.Collections.Generic;
using System.Linq;
using Hazel.Udp;
using InnerNet;
using UnityEngine;

public class GameDiscovery : MonoBehaviour
{
	public JoinGameButton ButtonPrefab;

	public Transform ItemLocation;

	public float YStart = 0.56f;

	public float YOffset = -0.75f;

	private Dictionary<string, JoinGameButton> received = new Dictionary<string, JoinGameButton>();

	public void Start()
	{
		InnerDiscover component = GetComponent<InnerDiscover>();
		component.OnPacketGet += Receive;
		component.StartAsClient();
	}

	public void Update()
	{
		float time = Time.time;
		string[] array = received.Keys.ToArray();
		int num = 0;
		foreach (string key in array)
		{
			JoinGameButton joinGameButton = received[key];
			if (time - joinGameButton.timeRecieved > 3f)
			{
				received.Remove(key);
				Object.Destroy(joinGameButton.gameObject);
			}
			else
			{
				joinGameButton.transform.localPosition = new Vector3(0f, YStart + (float)num * YOffset, -1f);
				num++;
			}
		}
	}

	private void Receive(BroadcastPacket packet)
	{
		string[] array = packet.Data.Split('~');
		string address = packet.GetAddress();
		if (received.TryGetValue(address, out var value))
		{
			value.timeRecieved = Time.time;
			value.SetGameName(array);
		}
		else if (array[1].Equals("Open"))
		{
			CreateButtonForAddess(address, array);
		}
	}

	private void CreateButtonForAddess(string fromAddress, string[] gameNameParts)
	{
		if (received.TryGetValue(fromAddress, out var value))
		{
			Object.Destroy(value.gameObject);
		}
		JoinGameButton joinGameButton = Object.Instantiate(ButtonPrefab, ItemLocation);
		joinGameButton.transform.localPosition = new Vector3(0f, YStart + (float)(ItemLocation.childCount - 1) * YOffset, -1f);
		joinGameButton.netAddress = fromAddress;
		joinGameButton.timeRecieved = Time.time;
		joinGameButton.SetGameName(gameNameParts);
		joinGameButton.GetComponentInChildren<MeshRenderer>().material.SetInt("_Mask", 4);
		received[fromAddress] = joinGameButton;
	}
}
