using System;
using System.Collections.Generic;
using Hazel;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class MedScanSystem : ISystemType
{
	// Token: 0x1700011B RID: 283
	// (get) Token: 0x06000764 RID: 1892 RVA: 0x00006935 File Offset: 0x00004B35
	// (set) Token: 0x06000765 RID: 1893 RVA: 0x0000693D File Offset: 0x00004B3D
	public byte CurrentUser { get; private set; } = byte.MaxValue;

	// Token: 0x06000766 RID: 1894 RVA: 0x0002A64C File Offset: 0x0002884C
	public bool Detoriorate(float deltaTime)
	{
		if (this.UsersList.Count > 0)
		{
			if (this.CurrentUser != this.UsersList[0])
			{
				if (this.CurrentUser != 255)
				{
					Debug.Log("Released scanner from: " + this.CurrentUser);
				}
				this.CurrentUser = this.UsersList[0];
				Debug.Log("Acquired scanner for: " + this.CurrentUser);
				return true;
			}
		}
		else if (this.CurrentUser != 255)
		{
			Debug.Log("Released scanner from: " + this.CurrentUser);
			this.CurrentUser = byte.MaxValue;
			return true;
		}
		return false;
	}

	// Token: 0x06000767 RID: 1895 RVA: 0x0002A708 File Offset: 0x00028908
	public void RepairDamage(PlayerControl player, byte data)
	{
		byte playerId = (byte)(data & 31);
		if ((data & 128) != 0)
		{
			if (!this.UsersList.Contains(playerId))
			{
				Debug.Log("Added to queue: " + playerId);
				this.UsersList.Add(playerId);
				return;
			}
		}
		else if ((data & 64) != 0)
		{
			Debug.Log("Removed from queue: " + playerId);
			this.UsersList.RemoveAll((byte v) => v == playerId);
		}
	}

	// Token: 0x06000768 RID: 1896 RVA: 0x0002A7A8 File Offset: 0x000289A8
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.WritePacked(this.UsersList.Count);
		for (int i = 0; i < this.UsersList.Count; i++)
		{
			writer.Write(this.UsersList[i]);
		}
	}

	// Token: 0x06000769 RID: 1897 RVA: 0x0002A7F0 File Offset: 0x000289F0
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.UsersList.Clear();
		int num = reader.ReadPackedInt32();
		for (int i = 0; i < num; i++)
		{
			this.UsersList.Add(reader.ReadByte());
		}
	}

	// Token: 0x04000738 RID: 1848
	public const byte Request = 128;

	// Token: 0x04000739 RID: 1849
	public const byte Release = 64;

	// Token: 0x0400073A RID: 1850
	public const byte NumMask = 31;

	// Token: 0x0400073B RID: 1851
	public const byte NoPlayer = 255;

	// Token: 0x0400073C RID: 1852
	public List<byte> UsersList = new List<byte>();
}
