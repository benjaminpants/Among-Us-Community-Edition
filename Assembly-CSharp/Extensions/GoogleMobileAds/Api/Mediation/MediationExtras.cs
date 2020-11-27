using System;
using System.Collections.Generic;

namespace GoogleMobileAds.Api.Mediation
{
	// Token: 0x02000285 RID: 645
	public abstract class MediationExtras
	{
		// Token: 0x17000218 RID: 536
		// (get) Token: 0x06000EB6 RID: 3766 RVA: 0x0000A7E4 File Offset: 0x000089E4
		// (set) Token: 0x06000EB7 RID: 3767 RVA: 0x0000A7EC File Offset: 0x000089EC
		public Dictionary<string, string> Extras { get; protected set; }

		// Token: 0x06000EB8 RID: 3768 RVA: 0x0000A7F5 File Offset: 0x000089F5
		public MediationExtras()
		{
			this.Extras = new Dictionary<string, string>();
		}

		// Token: 0x17000219 RID: 537
		// (get) Token: 0x06000EB9 RID: 3769
		public abstract string AndroidMediationExtraBuilderClassName { get; }

		// Token: 0x1700021A RID: 538
		// (get) Token: 0x06000EBA RID: 3770
		public abstract string IOSMediationExtraBuilderClassName { get; }
	}
}
