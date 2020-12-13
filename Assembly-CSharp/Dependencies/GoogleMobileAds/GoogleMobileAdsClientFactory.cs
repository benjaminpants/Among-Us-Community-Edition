using GoogleMobileAds.Api;
using GoogleMobileAds.Common;

namespace GoogleMobileAds
{
	public class GoogleMobileAdsClientFactory
	{
		public static IBannerClient BuildBannerClient()
		{
			return new DummyClient();
		}

		public static IInterstitialClient BuildInterstitialClient()
		{
			return new DummyClient();
		}

		public static IRewardBasedVideoAdClient BuildRewardBasedVideoAdClient()
		{
			return new DummyClient();
		}

		public static IRewardedAdClient BuildRewardedAdClient()
		{
			return new RewardedAdDummyClient();
		}

		public static IAdLoaderClient BuildAdLoaderClient(AdLoader adLoader)
		{
			return new DummyClient();
		}

		public static IMobileAdsClient MobileAdsInstance()
		{
			return new DummyClient();
		}
	}
}
