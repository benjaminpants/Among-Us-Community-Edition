using System;
using Hazel;

public class SwitchSystem : ISystemType, IActivatable
{
	public const byte MaxValue = byte.MaxValue;

	public const int NumSwitches = 5;

	public const byte DamageSystem = 128;

	public const byte SwitchesMask = 31;

	public float DetoriorationTime = 0.03f;

	public byte Value = byte.MaxValue;

	private float timer;

	public byte ExpectedSwitches;

	public byte ActualSwitches;

	public float Level => (float)(int)Value / 255f;

	public bool IsActive => ExpectedSwitches != ActualSwitches;

	public SwitchSystem()
	{
		Random random = new Random();
		ExpectedSwitches = (byte)((uint)random.Next() & 0x1Fu);
		ActualSwitches = ExpectedSwitches;
	}

	public bool Detoriorate(float deltaTime)
	{
		timer += deltaTime;
		if (timer >= DetoriorationTime)
		{
			timer = 0f;
			if (ExpectedSwitches != ActualSwitches)
			{
				if (Value > 0)
				{
					Value = (byte)Math.Max(Value - 3, 0);
				}
				if (!HasTask<ElectricTask>())
				{
					PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.Electrical);
				}
			}
			else if (Value < byte.MaxValue)
			{
				Value = (byte)Math.Min(Value + 3, 255);
			}
		}
		return false;
	}

	public void RepairDamage(PlayerControl player, byte amount)
	{
		if (amount.HasBit(128))
		{
			ActualSwitches ^= (byte)(amount & 0x1F);
		}
		else
		{
			ActualSwitches ^= (byte)(1 << (int)amount);
		}
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(ExpectedSwitches);
		writer.Write(ActualSwitches);
		writer.Write(Value);
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		ExpectedSwitches = reader.ReadByte();
		ActualSwitches = reader.ReadByte();
		Value = reader.ReadByte();
	}

	protected static bool HasTask<T>()
	{
		for (int num = PlayerControl.LocalPlayer.myTasks.Count - 1; num > 0; num--)
		{
			if (PlayerControl.LocalPlayer.myTasks[num] is T)
			{
				return true;
			}
		}
		return false;
	}
}
