using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	// Token: 0x0200026C RID: 620
	public interface IRewardBasedVideoAdClient
	{
		// Token: 0x14000018 RID: 24
		// (add) Token: 0x06000D9C RID: 3484
		// (remove) Token: 0x06000D9D RID: 3485
		event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000019 RID: 25
		// (add) Token: 0x06000D9E RID: 3486
		// (remove) Token: 0x06000D9F RID: 3487
		event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x06000DA0 RID: 3488
		// (remove) Token: 0x06000DA1 RID: 3489
		event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x1400001B RID: 27
		// (add) Token: 0x06000DA2 RID: 3490
		// (remove) Token: 0x06000DA3 RID: 3491
		event EventHandler<EventArgs> OnAdStarted;

		// Token: 0x1400001C RID: 28
		// (add) Token: 0x06000DA4 RID: 3492
		// (remove) Token: 0x06000DA5 RID: 3493
		event EventHandler<Reward> OnAdRewarded;

		// Token: 0x1400001D RID: 29
		// (add) Token: 0x06000DA6 RID: 3494
		// (remove) Token: 0x06000DA7 RID: 3495
		event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x1400001E RID: 30
		// (add) Token: 0x06000DA8 RID: 3496
		// (remove) Token: 0x06000DA9 RID: 3497
		event EventHandler<EventArgs> OnAdLeavingApplication;

		// Token: 0x1400001F RID: 31
		// (add) Token: 0x06000DAA RID: 3498
		// (remove) Token: 0x06000DAB RID: 3499
		event EventHandler<EventArgs> OnAdCompleted;

		// Token: 0x06000DAC RID: 3500
		void CreateRewardBasedVideoAd();

		// Token: 0x06000DAD RID: 3501
		void LoadAd(AdRequest request, string adUnitId);

		// Token: 0x06000DAE RID: 3502
		bool IsLoaded();

		// Token: 0x06000DAF RID: 3503
		string MediationAdapterClassName();

		// Token: 0x06000DB0 RID: 3504
		void ShowRewardBasedVideoAd();

		// Token: 0x06000DB1 RID: 3505
		void SetUserId(string userId);
	}
}
