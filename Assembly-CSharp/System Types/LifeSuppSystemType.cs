using System.Collections.Generic;
using Hazel;

public class LifeSuppSystemType : ISystemType, IActivatable
{
	private const float SyncRate = 2f;

	private float timer;

	public const byte StartCountdown = 128;

	public const byte AddUserOp = 64;

	public const byte ClearCountdown = 16;

	public const float CountdownStopped = 10000f;

	public const float LifeSuppDuration = 45f;

	public const byte ConsoleIdMask = 3;

	public const byte RequiredUserCount = 2;

	public float Countdown = 10000f;

	private HashSet<int> CompletedConsoles = new HashSet<int>();

	public int UserCount => CompletedConsoles.Count;

	public bool IsActive => Countdown < 10000f;

	public bool GetConsoleComplete(int consoleId)
	{
		return CompletedConsoles.Contains(consoleId);
	}

	public void RepairDamage(PlayerControl player, byte opCode)
	{
		int item = opCode & 3;
		if (opCode == 128 && !IsActive)
		{
			Countdown = 45f;
			CompletedConsoles.Clear();
		}
		else if (opCode == 16)
		{
			Countdown = 10000f;
		}
		else if (opCode.HasAnyBit((byte)64))
		{
			CompletedConsoles.Add(item);
		}
	}

	public bool Detoriorate(float deltaTime)
	{
		if (IsActive)
		{
			if (DestroyableSingleton<HudManager>.Instance.OxyFlash == null)
			{
				PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.LifeSupp);
			}
			Countdown -= deltaTime;
			if (UserCount >= 2)
			{
				Countdown = 10000f;
				return true;
			}
			timer += deltaTime;
			if (timer > 2f)
			{
				timer = 0f;
				return true;
			}
		}
		else if (DestroyableSingleton<HudManager>.Instance.OxyFlash != null)
		{
			DestroyableSingleton<HudManager>.Instance.StopOxyFlash();
		}
		return false;
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(Countdown);
		writer.WritePacked(CompletedConsoles.Count);
		foreach (int completedConsole in CompletedConsoles)
		{
			writer.WritePacked(completedConsole);
		}
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		Countdown = reader.ReadSingle();
		if (reader.Position < reader.Length)
		{
			CompletedConsoles.Clear();
			int num = reader.ReadPackedInt32();
			for (int i = 0; i < num; i++)
			{
				CompletedConsoles.Add(reader.ReadPackedInt32());
			}
		}
	}
}
