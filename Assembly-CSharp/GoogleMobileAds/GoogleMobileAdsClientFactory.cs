using System;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
	// Token: 0x02000265 RID: 613
	public class GoogleMobileAdsClientFactory
	{
		// Token: 0x06000D31 RID: 3377 RVA: 0x00009EB7 File Offset: 0x000080B7
		public static IBannerClient BuildBannerClient()
		{
			return new DummyClient();
		}

		// Token: 0x06000D32 RID: 3378 RVA: 0x00009EB7 File Offset: 0x000080B7
		public static IInterstitialClient BuildInterstitialClient()
		{
			return new DummyClient();
		}

		// Token: 0x06000D33 RID: 3379 RVA: 0x00009EB7 File Offset: 0x000080B7
		public static IRewardBasedVideoAdClient BuildRewardBasedVideoAdClient()
		{
			return new DummyClient();
		}

		// Token: 0x06000D34 RID: 3380 RVA: 0x00009EBE File Offset: 0x000080BE
		public static IRewardedAdClient BuildRewardedAdClient()
		{
			return new RewardedAdDummyClient();
		}

		// Token: 0x06000D35 RID: 3381 RVA: 0x00009EB7 File Offset: 0x000080B7
		public static IAdLoaderClient BuildAdLoaderClient(AdLoader adLoader)
		{
			return new DummyClient();
		}

		// Token: 0x06000D36 RID: 3382 RVA: 0x00009EB7 File Offset: 0x000080B7
		public static IMobileAdsClient MobileAdsInstance()
		{
			return new DummyClient();
		}
	}
}
