using System.Linq;
using Hazel;

public class DoorsSystemType : ISystemType, IActivatable
{
	private AutoOpenDoor[] doors;

	private uint dirtyBits;

	public bool IsActive => doors.Any((AutoOpenDoor b) => !b.Open);

	public void SetDoors(AutoOpenDoor[] doors)
	{
		this.doors = doors;
	}

	public bool Detoriorate(float deltaTime)
	{
		if (doors == null)
		{
			return false;
		}
		for (int i = 0; i < doors.Length; i++)
		{
			if (doors[i].DoUpdate(deltaTime))
			{
				dirtyBits |= (uint)(1 << i);
			}
		}
		return dirtyBits != 0;
	}

	public void RepairDamage(PlayerControl player, byte amount)
	{
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		if (initialState)
		{
			for (int i = 0; i < doors.Length; i++)
			{
				doors[i].Serialize(writer);
			}
			return;
		}
		writer.WritePacked(dirtyBits);
		for (int j = 0; j < doors.Length; j++)
		{
			if ((dirtyBits & (uint)(1 << j)) != 0)
			{
				doors[j].Serialize(writer);
			}
		}
		dirtyBits = 0u;
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		if (initialState)
		{
			for (int i = 0; i < doors.Length; i++)
			{
				doors[i].Deserialize(reader);
			}
			return;
		}
		uint num = reader.ReadPackedUInt32();
		for (int j = 0; j < doors.Length; j++)
		{
			if ((num & (uint)(1 << j)) != 0)
			{
				doors[j].Deserialize(reader);
			}
		}
	}

	public void SetDoor(AutoOpenDoor door, bool open)
	{
		door.SetDoorway(open);
		dirtyBits |= (uint)(1 << doors.IndexOf(door));
	}

	public void CloseDoorsOfType(SystemTypes room)
	{
		for (int i = 0; i < doors.Length; i++)
		{
			AutoOpenDoor autoOpenDoor = doors[i];
			if (autoOpenDoor.Room == room)
			{
				autoOpenDoor.SetDoorway(open: false);
				dirtyBits |= (uint)(1 << i);
			}
		}
	}

	public float GetTimer(SystemTypes room)
	{
		for (int i = 0; i < doors.Length; i++)
		{
			AutoOpenDoor autoOpenDoor = doors[i];
			if (autoOpenDoor.Room == room)
			{
				return autoOpenDoor.CooldownTimer;
			}
		}
		return 0f;
	}
}
