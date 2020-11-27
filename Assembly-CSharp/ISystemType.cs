using System;
using Hazel;

// Token: 0x020001F3 RID: 499
public interface ISystemType
{
	// Token: 0x06000AC7 RID: 2759
	bool Detoriorate(float deltaTime);

	// Token: 0x06000AC8 RID: 2760
	void RepairDamage(PlayerControl player, byte amount);

	// Token: 0x06000AC9 RID: 2761
	void Serialize(MessageWriter writer, bool initialState);

	// Token: 0x06000ACA RID: 2762
	void Deserialize(MessageReader reader, bool initialState);
}
