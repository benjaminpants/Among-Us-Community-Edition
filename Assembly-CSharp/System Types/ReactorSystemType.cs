using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;

public class ReactorSystemType : ISystemType, IActivatable
{
	private const float SyncRate = 2f;

	private float timer;

	public const byte StartCountdown = 128;

	public const byte AddUserOp = 64;

	public const byte RemoveUserOp = 32;

	public const byte ClearCountdown = 16;

	public const float CountdownStopped = 10000f;

	public const float ReactorDuration = 30f;

	public const byte ConsoleIdMask = 3;

	public const byte RequiredUserCount = 2;

	public float Countdown = 10000f;

	private HashSet<Tuple<byte, byte>> UserConsolePairs = new HashSet<Tuple<byte, byte>>();

	public int UserCount
	{
		get
		{
			int num = 0;
			int num2 = 0;
			foreach (Tuple<byte, byte> userConsolePair in UserConsolePairs)
			{
				int num3 = 1 << (int)userConsolePair.Item2;
				if ((num3 & num2) == 0)
				{
					num++;
					num2 |= num3;
				}
			}
			return num;
		}
	}

	public bool IsActive => Countdown < 10000f;

	public bool GetConsoleComplete(int consoleId)
	{
		return UserConsolePairs.Any((Tuple<byte, byte> kvp) => kvp.Item2 == consoleId);
	}

	public void RepairDamage(PlayerControl player, byte opCode)
	{
		int num = opCode & 3;
		if (opCode == 128 && !IsActive)
		{
			Countdown = 30f;
			UserConsolePairs.Clear();
		}
		else if (opCode == 16)
		{
			Countdown = 10000f;
		}
		else if (opCode.HasAnyBit((byte)64))
		{
			UserConsolePairs.Add(new Tuple<byte, byte>(player.PlayerId, (byte)num));
			if (UserCount >= 2)
			{
				Countdown = 10000f;
			}
		}
		else if (opCode.HasAnyBit((byte)32))
		{
			UserConsolePairs.Remove(new Tuple<byte, byte>(player.PlayerId, (byte)num));
		}
	}

	public bool Detoriorate(float deltaTime)
	{
		if (IsActive)
		{
			if (DestroyableSingleton<HudManager>.Instance.ReactorFlash == null)
			{
				PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.Reactor);
			}
			Countdown -= deltaTime;
			timer += deltaTime;
			if (timer > 2f)
			{
				timer = 0f;
				return true;
			}
		}
		else if (DestroyableSingleton<HudManager>.Instance.ReactorFlash != null)
		{
			((ReactorShipRoom)ShipStatus.Instance.AllRooms.First((ShipRoom r) => r.RoomId == SystemTypes.Reactor)).StopMeltdown();
		}
		return false;
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(Countdown);
		writer.WritePacked(UserConsolePairs.Count);
		foreach (Tuple<byte, byte> userConsolePair in UserConsolePairs)
		{
			writer.Write(userConsolePair.Item1);
			writer.Write(userConsolePair.Item2);
		}
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		Countdown = reader.ReadSingle();
		UserConsolePairs.Clear();
		int num = reader.ReadPackedInt32();
		for (int i = 0; i < num; i++)
		{
			UserConsolePairs.Add(new Tuple<byte, byte>(reader.ReadByte(), reader.ReadByte()));
		}
	}
}
