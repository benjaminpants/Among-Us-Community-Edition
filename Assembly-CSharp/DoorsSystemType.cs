using System;
using System.Linq;
using Hazel;

// Token: 0x020001CF RID: 463
public class DoorsSystemType : ISystemType, IActivatable
{
	// Token: 0x17000182 RID: 386
	// (get) Token: 0x06000A06 RID: 2566 RVA: 0x000081F4 File Offset: 0x000063F4
	public bool IsActive
	{
		get
		{
			return this.doors.Any((AutoOpenDoor b) => !b.Open);
		}
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x00008220 File Offset: 0x00006420
	public void SetDoors(AutoOpenDoor[] doors)
	{
		this.doors = doors;
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x00034864 File Offset: 0x00032A64
	public bool Detoriorate(float deltaTime)
	{
		if (this.doors == null)
		{
			return false;
		}
		for (int i = 0; i < this.doors.Length; i++)
		{
			if (this.doors[i].DoUpdate(deltaTime))
			{
				this.dirtyBits |= 1U << i;
			}
		}
		return this.dirtyBits > 0U;
	}

	// Token: 0x06000A09 RID: 2569 RVA: 0x00002265 File Offset: 0x00000465
	public void RepairDamage(PlayerControl player, byte amount)
	{
	}

	// Token: 0x06000A0A RID: 2570 RVA: 0x000348BC File Offset: 0x00032ABC
	public void Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			for (int i = 0; i < this.doors.Length; i++)
			{
				this.doors[i].Serialize(writer);
			}
			return;
		}
		writer.WritePacked(this.dirtyBits);
		for (int j = 0; j < this.doors.Length; j++)
		{
			if ((this.dirtyBits & 1U << j) != 0U)
			{
				this.doors[j].Serialize(writer);
			}
		}
		this.dirtyBits = 0U;
	}

	// Token: 0x06000A0B RID: 2571 RVA: 0x00034934 File Offset: 0x00032B34
	public void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			for (int i = 0; i < this.doors.Length; i++)
			{
				this.doors[i].Deserialize(reader);
			}
			return;
		}
		uint num = reader.ReadPackedUInt32();
		for (int j = 0; j < this.doors.Length; j++)
		{
			if ((num & 1U << j) != 0U)
			{
				this.doors[j].Deserialize(reader);
			}
		}
	}

	// Token: 0x06000A0C RID: 2572 RVA: 0x00008229 File Offset: 0x00006429
	public void SetDoor(AutoOpenDoor door, bool open)
	{
		door.SetDoorway(open);
		this.dirtyBits |= 1U << this.doors.IndexOf(door);
	}

	// Token: 0x06000A0D RID: 2573 RVA: 0x00034998 File Offset: 0x00032B98
	public void CloseDoorsOfType(SystemTypes room)
	{
		for (int i = 0; i < this.doors.Length; i++)
		{
			AutoOpenDoor autoOpenDoor = this.doors[i];
			if (autoOpenDoor.Room == room)
			{
				autoOpenDoor.SetDoorway(false);
				this.dirtyBits |= 1U << i;
			}
		}
	}

	// Token: 0x06000A0E RID: 2574 RVA: 0x000349E4 File Offset: 0x00032BE4
	public float GetTimer(SystemTypes room)
	{
		for (int i = 0; i < this.doors.Length; i++)
		{
			AutoOpenDoor autoOpenDoor = this.doors[i];
			if (autoOpenDoor.Room == room)
			{
				return autoOpenDoor.CooldownTimer;
			}
		}
		return 0f;
	}

	// Token: 0x040009AE RID: 2478
	private AutoOpenDoor[] doors;

	// Token: 0x040009AF RID: 2479
	private uint dirtyBits;
}
