using System;
using InnerNet;

// Token: 0x02000111 RID: 273
public interface IDisconnectHandler
{
	// Token: 0x060005D0 RID: 1488
	void HandleDisconnect(PlayerControl pc, DisconnectReasons reason);

	// Token: 0x060005D1 RID: 1489
	void HandleDisconnect();
}
