using System;

namespace InnerNet
{
	// Token: 0x0200025B RID: 603
	public enum JoinFailureReasons : byte
	{
		// Token: 0x04000C80 RID: 3200
		TooManyPlayers = 1,
		// Token: 0x04000C81 RID: 3201
		GameStarted,
		// Token: 0x04000C82 RID: 3202
		GameNotFound,
		// Token: 0x04000C83 RID: 3203
		HostNotReady,
		// Token: 0x04000C84 RID: 3204
		IncorrectVersion,
		// Token: 0x04000C85 RID: 3205
		Banned,
		// Token: 0x04000C86 RID: 3206
		Kicked
	}
}
