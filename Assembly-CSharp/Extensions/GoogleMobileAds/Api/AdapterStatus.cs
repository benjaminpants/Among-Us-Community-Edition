using System;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000271 RID: 625
	public class AdapterStatus
	{
		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06000DE0 RID: 3552 RVA: 0x0000A016 File Offset: 0x00008216
		// (set) Token: 0x06000DE1 RID: 3553 RVA: 0x0000A01E File Offset: 0x0000821E
		public AdapterState InitializationState { get; private set; }

		// Token: 0x170001F7 RID: 503
		// (get) Token: 0x06000DE2 RID: 3554 RVA: 0x0000A027 File Offset: 0x00008227
		// (set) Token: 0x06000DE3 RID: 3555 RVA: 0x0000A02F File Offset: 0x0000822F
		public string Description { get; private set; }

		// Token: 0x170001F8 RID: 504
		// (get) Token: 0x06000DE4 RID: 3556 RVA: 0x0000A038 File Offset: 0x00008238
		// (set) Token: 0x06000DE5 RID: 3557 RVA: 0x0000A040 File Offset: 0x00008240
		public int Latency { get; private set; }

		// Token: 0x06000DE6 RID: 3558 RVA: 0x0000A049 File Offset: 0x00008249
		internal AdapterStatus(AdapterState state, string description, int latency)
		{
			this.InitializationState = state;
			this.Description = description;
			this.Latency = latency;
		}
	}
}
