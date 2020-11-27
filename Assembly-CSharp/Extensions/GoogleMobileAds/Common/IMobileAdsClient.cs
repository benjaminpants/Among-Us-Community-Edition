using System;

namespace GoogleMobileAds.Common
{
	// Token: 0x0200026B RID: 619
	public interface IMobileAdsClient
	{
		// Token: 0x06000D98 RID: 3480
		void Initialize(string appId);

		// Token: 0x06000D99 RID: 3481
		void SetApplicationVolume(float volume);

		// Token: 0x06000D9A RID: 3482
		void SetApplicationMuted(bool muted);

		// Token: 0x06000D9B RID: 3483
		void SetiOSAppPauseOnBackground(bool pause);
	}
}
