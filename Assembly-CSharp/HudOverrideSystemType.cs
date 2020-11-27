using System;
using Hazel;

// Token: 0x020001D6 RID: 470
internal class HudOverrideSystemType : ISystemType, IActivatable
{
	// Token: 0x17000188 RID: 392
	// (get) Token: 0x06000A2A RID: 2602 RVA: 0x000082E2 File Offset: 0x000064E2
	// (set) Token: 0x06000A2B RID: 2603 RVA: 0x000082EA File Offset: 0x000064EA
	public bool IsActive { get; private set; }

	// Token: 0x06000A2C RID: 2604 RVA: 0x000082F3 File Offset: 0x000064F3
	public bool Detoriorate(float deltaTime)
	{
		if (this.IsActive && !PlayerTask.PlayerHasHudTask(PlayerControl.LocalPlayer))
		{
			PlayerControl.LocalPlayer.AddSystemTask(SystemTypes.Comms);
		}
		return false;
	}

	// Token: 0x06000A2D RID: 2605 RVA: 0x00008316 File Offset: 0x00006516
	public void RepairDamage(PlayerControl player, byte amount)
	{
		if ((amount & 128) > 0)
		{
			this.IsActive = true;
			return;
		}
		this.IsActive = false;
	}

	// Token: 0x06000A2E RID: 2606 RVA: 0x00008333 File Offset: 0x00006533
	public void Serialize(MessageWriter writer, bool initialState)
	{
		writer.Write(this.IsActive);
	}

	// Token: 0x06000A2F RID: 2607 RVA: 0x00008341 File Offset: 0x00006541
	public void Deserialize(MessageReader reader, bool initialState)
	{
		this.IsActive = reader.ReadBoolean();
	}

	// Token: 0x040009CA RID: 2506
	public const byte DamageBit = 128;

	// Token: 0x040009CB RID: 2507
	public const byte TaskMask = 127;
}
