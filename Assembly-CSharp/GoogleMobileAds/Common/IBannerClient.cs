using System;
using GoogleMobileAds.Api;

namespace GoogleMobileAds.Common
{
	// Token: 0x02000268 RID: 616
	public interface IBannerClient
	{
		// Token: 0x1400000E RID: 14
		// (add) Token: 0x06000D6D RID: 3437
		// (remove) Token: 0x06000D6E RID: 3438
		event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x1400000F RID: 15
		// (add) Token: 0x06000D6F RID: 3439
		// (remove) Token: 0x06000D70 RID: 3440
		event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x14000010 RID: 16
		// (add) Token: 0x06000D71 RID: 3441
		// (remove) Token: 0x06000D72 RID: 3442
		event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x14000011 RID: 17
		// (add) Token: 0x06000D73 RID: 3443
		// (remove) Token: 0x06000D74 RID: 3444
		event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x14000012 RID: 18
		// (add) Token: 0x06000D75 RID: 3445
		// (remove) Token: 0x06000D76 RID: 3446
		event EventHandler<EventArgs> OnAdLeavingApplication;

		// Token: 0x06000D77 RID: 3447
		void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position);

		// Token: 0x06000D78 RID: 3448
		void CreateBannerView(string adUnitId, AdSize adSize, int x, int y);

		// Token: 0x06000D79 RID: 3449
		void LoadAd(AdRequest request);

		// Token: 0x06000D7A RID: 3450
		void ShowBannerView();

		// Token: 0x06000D7B RID: 3451
		void HideBannerView();

		// Token: 0x06000D7C RID: 3452
		void DestroyBannerView();

		// Token: 0x06000D7D RID: 3453
		float GetHeightInPixels();

		// Token: 0x06000D7E RID: 3454
		float GetWidthInPixels();

		// Token: 0x06000D7F RID: 3455
		void SetPosition(AdPosition adPosition);

		// Token: 0x06000D80 RID: 3456
		void SetPosition(int x, int y);

		// Token: 0x06000D81 RID: 3457
		string MediationAdapterClassName();
	}
}
