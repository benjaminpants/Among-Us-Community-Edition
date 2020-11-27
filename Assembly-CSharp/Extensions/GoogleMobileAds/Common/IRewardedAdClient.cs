using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	// Token: 0x0200026D RID: 621
	public interface IRewardedAdClient
	{
		// Token: 0x14000020 RID: 32
		// (add) Token: 0x06000DB2 RID: 3506
		// (remove) Token: 0x06000DB3 RID: 3507
		event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000021 RID: 33
		// (add) Token: 0x06000DB4 RID: 3508
		// (remove) Token: 0x06000DB5 RID: 3509
		event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		// Token: 0x14000022 RID: 34
		// (add) Token: 0x06000DB6 RID: 3510
		// (remove) Token: 0x06000DB7 RID: 3511
		event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		// Token: 0x14000023 RID: 35
		// (add) Token: 0x06000DB8 RID: 3512
		// (remove) Token: 0x06000DB9 RID: 3513
		event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x14000024 RID: 36
		// (add) Token: 0x06000DBA RID: 3514
		// (remove) Token: 0x06000DBB RID: 3515
		event EventHandler<Reward> OnUserEarnedReward;

		// Token: 0x14000025 RID: 37
		// (add) Token: 0x06000DBC RID: 3516
		// (remove) Token: 0x06000DBD RID: 3517
		event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x06000DBE RID: 3518
		void CreateRewardedAd(string adUnitId);

		// Token: 0x06000DBF RID: 3519
		void LoadAd(AdRequest request);

		// Token: 0x06000DC0 RID: 3520
		bool IsLoaded();

		// Token: 0x06000DC1 RID: 3521
		string MediationAdapterClassName();

		// Token: 0x06000DC2 RID: 3522
		void Show();
	}
}
