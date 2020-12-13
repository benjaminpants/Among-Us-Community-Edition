using System.Linq;
using Hazel;

public class SabotageSystemType : ISystemType
{
	public const float SpecialSabDelay = 30f;

	private IActivatable[] specials;

	private bool dirty;

	public float Timer
	{
		get;
		set;
	}

	public float PercentCool => Timer / 30f;

	public bool AnyActive => specials.Any((IActivatable s) => s.IsActive);

	public SabotageSystemType(IActivatable[] specials)
	{
		this.specials = specials;
	}

	public bool Detoriorate(float deltaTime)
	{
		if (Timer > 0f && !AnyActive)
		{
			Timer -= deltaTime;
			if (Timer <= 0f)
			{
				return true;
			}
		}
		return dirty;
	}

	public void RepairDamage(PlayerControl player, byte amount)
	{
		dirty = true;
		if (Timer > 0f || (bool)MeetingHud.Instance)
		{
			return;
		}
		if (AmongUsClient.Instance.AmHost)
		{
			switch (amount)
			{
			case 3:
				ShipStatus.Instance.RepairSystem(SystemTypes.Reactor, player, 128);
				break;
			case 8:
				ShipStatus.Instance.RepairSystem(SystemTypes.LifeSupp, player, 128);
				break;
			case 14:
				ShipStatus.Instance.RepairSystem(SystemTypes.Comms, player, 128);
				break;
			case 7:
			{
				byte b = 4;
				for (int i = 0; i < 5; i++)
				{
					if (BoolRange.Next())
					{
						b = (byte)(b | (byte)(1 << i));
					}
				}
				ShipStatus.Instance.RpcRepairSystem(SystemTypes.Electrical, (byte)(b | 0x80));
				break;
			}
			}
		}
		Timer = 30f;
	}

	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(Timer);
		if (!initialState)
		{
			dirty = false;
		}
	}

	public void Deserialize(MessageReader reader, bool initialState)
	{
		Timer = reader.ReadSingle();
	}
}
