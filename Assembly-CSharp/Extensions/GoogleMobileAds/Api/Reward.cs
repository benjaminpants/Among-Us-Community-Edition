using System;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000282 RID: 642
	public class Reward : EventArgs
	{
		// Token: 0x17000215 RID: 533
		// (get) Token: 0x06000E7A RID: 3706 RVA: 0x0000A5F4 File Offset: 0x000087F4
		// (set) Token: 0x06000E7B RID: 3707 RVA: 0x0000A5FC File Offset: 0x000087FC
		public string Type { get; set; }

		// Token: 0x17000216 RID: 534
		// (get) Token: 0x06000E7C RID: 3708 RVA: 0x0000A605 File Offset: 0x00008805
		// (set) Token: 0x06000E7D RID: 3709 RVA: 0x0000A60D File Offset: 0x0000880D
		public double Amount { get; set; }
	}
}
