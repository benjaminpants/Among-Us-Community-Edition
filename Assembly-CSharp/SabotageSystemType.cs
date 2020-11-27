using System;
using System.Linq;
using Hazel;

// Token: 0x020001E2 RID: 482
public class SabotageSystemType : ISystemType
{
	// Token: 0x17000195 RID: 405
	// (get) Token: 0x06000A60 RID: 2656 RVA: 0x00008434 File Offset: 0x00006634
	// (set) Token: 0x06000A61 RID: 2657 RVA: 0x0000843C File Offset: 0x0000663C
	public float Timer { get; set; }

	// Token: 0x17000196 RID: 406
	// (get) Token: 0x06000A62 RID: 2658 RVA: 0x00008445 File Offset: 0x00006645
	public float PercentCool
	{
		get
		{
			return this.Timer / 30f;
		}
	}

	// Token: 0x17000197 RID: 407
	// (get) Token: 0x06000A63 RID: 2659 RVA: 0x00008453 File Offset: 0x00006653
	public bool AnyActive
	{
		get
		{
			return this.specials.Any((IActivatable s) => s.IsActive);
		}
	}

	// Token: 0x06000A64 RID: 2660 RVA: 0x0000847F File Offset: 0x0000667F
	public SabotageSystemType(IActivatable[] specials)
	{
		this.specials = specials;
	}

	// Token: 0x06000A65 RID: 2661 RVA: 0x0000848E File Offset: 0x0000668E
	public bool Detoriorate(float deltaTime)
	{
		if (this.Timer > 0f && !this.AnyActive)
		{
			this.Timer -= deltaTime;
			if (this.Timer <= 0f)
			{
				return true;
			}
		}
		return this.dirty;
	}

	// Token: 0x06000A66 RID: 2662 RVA: 0x000357B4 File Offset: 0x000339B4
	public void RepairDamage(PlayerControl player, byte amount)
	{
		this.dirty = true;
		if (this.Timer > 0f)
		{
			return;
		}
		if (MeetingHud.Instance)
		{
			return;
		}
		if (AmongUsClient.Instance.AmHost)
		{
			if (amount <= 7)
			{
				if (amount != 3)
				{
					if (amount == 7)
					{
						byte b = 4;
						for (int i = 0; i < 5; i++)
						{
							if (BoolRange.Next(0.5f))
							{
								b |= (byte)(1 << i);
							}
						}
						ShipStatus.Instance.RpcRepairSystem(SystemTypes.Electrical, (int)(b | 128));
					}
				}
				else
				{
					ShipStatus.Instance.RepairSystem(SystemTypes.Reactor, player, 128);
				}
			}
			else if (amount != 8)
			{
				if (amount == 14)
				{
					ShipStatus.Instance.RepairSystem(SystemTypes.Comms, player, 128);
				}
			}
			else
			{
				ShipStatus.Instance.RepairSystem(SystemTypes.LifeSupp, player, 128);
			}
		}
		this.Timer = 30f;
	}

	// Token: 0x06000A67 RID: 2663 RVA: 0x000084C8 File Offset: 0x000066C8
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(this.Timer);
		if (!initialState)
		{
			this.dirty = false;
		}
	}

	// Token: 0x06000A68 RID: 2664 RVA: 0x000084E0 File Offset: 0x000066E0
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.Timer = reader.ReadSingle();
	}

	// Token: 0x04000A08 RID: 2568
	public const float SpecialSabDelay = 30f;

	// Token: 0x04000A0A RID: 2570
	private IActivatable[] specials;

	// Token: 0x04000A0B RID: 2571
	private bool dirty;
}
