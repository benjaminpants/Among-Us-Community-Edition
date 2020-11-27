using System;

// Token: 0x020001D7 RID: 471
public interface IUsable
{
	// Token: 0x17000189 RID: 393
	// (get) Token: 0x06000A31 RID: 2609
	float UsableDistance { get; }

	// Token: 0x1700018A RID: 394
	// (get) Token: 0x06000A32 RID: 2610
	float PercentCool { get; }

	// Token: 0x06000A33 RID: 2611
	void SetOutline(bool on, bool mainTarget);

	// Token: 0x06000A34 RID: 2612
	float CanUse(GameData.PlayerInfo pc, out bool canUse, out bool couldUse);

	// Token: 0x06000A35 RID: 2613
	void Use();
}
