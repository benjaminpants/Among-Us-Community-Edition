using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	// Token: 0x0200026A RID: 618
	public interface IInterstitialClient
	{
		// Token: 0x14000013 RID: 19
		// (add) Token: 0x06000D88 RID: 3464
		// (remove) Token: 0x06000D89 RID: 3465
		event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000014 RID: 20
		// (add) Token: 0x06000D8A RID: 3466
		// (remove) Token: 0x06000D8B RID: 3467
		event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x14000015 RID: 21
		// (add) Token: 0x06000D8C RID: 3468
		// (remove) Token: 0x06000D8D RID: 3469
		event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x14000016 RID: 22
		// (add) Token: 0x06000D8E RID: 3470
		// (remove) Token: 0x06000D8F RID: 3471
		event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x14000017 RID: 23
		// (add) Token: 0x06000D90 RID: 3472
		// (remove) Token: 0x06000D91 RID: 3473
		event EventHandler<EventArgs> OnAdLeavingApplication;

		// Token: 0x06000D92 RID: 3474
		void CreateInterstitialAd(string adUnitId);

		// Token: 0x06000D93 RID: 3475
		void LoadAd(AdRequest request);

		// Token: 0x06000D94 RID: 3476
		bool IsLoaded();

		// Token: 0x06000D95 RID: 3477
		void ShowInterstitial();

		// Token: 0x06000D96 RID: 3478
		void DestroyInterstitial();

		// Token: 0x06000D97 RID: 3479
		string MediationAdapterClassName();
	}
}
