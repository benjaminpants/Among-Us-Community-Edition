using System;

namespace InnerNet
{
	// Token: 0x02000245 RID: 581
	public enum DisconnectReasons
	{
		// Token: 0x04000C04 RID: 3076
		ExitGame,
		// Token: 0x04000C05 RID: 3077
		GameFull,
		// Token: 0x04000C06 RID: 3078
		GameStarted,
		// Token: 0x04000C07 RID: 3079
		GameNotFound,
		// Token: 0x04000C08 RID: 3080
		IncorrectVersion = 5,
		// Token: 0x04000C09 RID: 3081
		Banned,
		// Token: 0x04000C0A RID: 3082
		Kicked,
		// Token: 0x04000C0B RID: 3083
		Custom,
		// Token: 0x04000C0C RID: 3084
		Destroy = 16,
		// Token: 0x04000C0D RID: 3085
		Error,
		// Token: 0x04000C0E RID: 3086
		IncorrectGame,
		// Token: 0x04000C0F RID: 3087
		ServerRequest,
		// Token: 0x04000C10 RID: 3088
		ServerFull,
		// Token: 0x04000C11 RID: 3089
		IntentionalLeaving = 208,
		// Token: 0x04000C12 RID: 3090
		FocusLostBackground = 207,
		// Token: 0x04000C13 RID: 3091
		FocusLost = 209,
		// Token: 0x04000C14 RID: 3092
		NewConnection
	}
}
