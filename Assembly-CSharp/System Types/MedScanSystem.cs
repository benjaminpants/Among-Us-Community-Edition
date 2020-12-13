using System.Collections.Generic;
using Hazel;
using UnityEngine;

public class MedScanSystem : ISystemType
{
	public const byte Request = 128;

	public const byte Release = 64;

	public const byte NumMask = 31;

	public const byte NoPlayer = byte.MaxValue;

	public List<byte> UsersList = new List<byte>();

	public byte CurrentUser
	{
		get;
		private set;
	} = byte.MaxValue;


	public bool Detoriorate(float deltaTime)
	{
		if (UsersList.Count > 0)
		{
			if (CurrentUser != UsersList[0])
			{
				if (CurrentUser != byte.MaxValue)
				{
					Debug.Log("Released scanner from: " + CurrentUser);
				}
				CurrentUser = UsersList[0];
				Debug.Log("Acquired scanner for: " + CurrentUser);
				return true;
			}
		}
		else if (CurrentUser != byte.MaxValue)
		{
			Debug.Log("Released scanner from: " + CurrentUser);
			CurrentUser = byte.MaxValue;
			return true;
		}
		return false;
	}

	public void RepairDamage(PlayerControl player, byte data)
	{
		byte playerId = (byte)(data & 0x1Fu);
		if ((data & 0x80u) != 0)
		{
			if (!UsersList.Contains(playerId))
			{
				Debug.Log("Added to queue: " + playerId);
				UsersList.Add(playerId);
			}
		}
		else if ((data & 0x40u) != 0)
		{
			Debug.Log("Removed from queue: " + playerId);
			UsersList.RemoveAll((byte v) => v == playerId);
		}
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.WritePacked(UsersList.Count);
		for (int i = 0; i < UsersList.Count; i++)
		{
			writer.Write(UsersList[i]);
		}
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		UsersList.Clear();
		int num = reader.ReadPackedInt32();
		for (int i = 0; i < num; i++)
		{
			UsersList.Add(reader.ReadByte());
		}
	}
}
