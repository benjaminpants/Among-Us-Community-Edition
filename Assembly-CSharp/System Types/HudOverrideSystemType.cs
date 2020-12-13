using Hazel;

internal class HudOverrideSystemType : ISystemType, IActivatable
{
	public const byte DamageBit = 128;

	public const byte TaskMask = 127;

	public bool IsActive
	{
		get;
		private set;
	}

	public bool Detoriorate(float deltaTime)
	{
		if (IsActive && !PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.Comms);
		}
		return false;
	}

	public void RepairDamage(PlayerControl player, byte amount)
	{
		if ((amount & 0x80u) != 0)
		{
			IsActive = true;
		}
		else
		{
			IsActive = false;
		}
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(IsActive);
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		IsActive = reader.ReadBoolean();
	}
}
