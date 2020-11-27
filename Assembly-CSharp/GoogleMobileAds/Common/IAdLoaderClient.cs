using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	// Token: 0x02000267 RID: 615
	public interface IAdLoaderClient
	{
		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06000D68 RID: 3432
		// (remove) Token: 0x06000D69 RID: 3433
		event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06000D6A RID: 3434
		// (remove) Token: 0x06000D6B RID: 3435
		event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		// Token: 0x06000D6C RID: 3436
		void LoadAd(AdRequest request);
	}
}
