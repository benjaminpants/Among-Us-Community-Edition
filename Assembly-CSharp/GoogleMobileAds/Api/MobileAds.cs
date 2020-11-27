using System;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000281 RID: 641
	public class MobileAds
	{
		// Token: 0x06000E73 RID: 3699 RVA: 0x0000A58B File Offset: 0x0000878B
		public static void Initialize(string appId)
		{
			MobileAds.client.Initialize(appId);
			MobileAdsEventExecutor.Initialize();
		}

		// Token: 0x06000E74 RID: 3700 RVA: 0x0000A59D File Offset: 0x0000879D
		public static void SetApplicationMuted(bool muted)
		{
			MobileAds.client.SetApplicationMuted(muted);
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x0000A5AA File Offset: 0x000087AA
		public static void SetApplicationVolume(float volume)
		{
			MobileAds.client.SetApplicationVolume(volume);
		}

		// Token: 0x06000E76 RID: 3702 RVA: 0x0000A5B7 File Offset: 0x000087B7
		public static void SetiOSAppPauseOnBackground(bool pause)
		{
			MobileAds.client.SetiOSAppPauseOnBackground(pause);
		}

		// Token: 0x06000E77 RID: 3703 RVA: 0x0000A5C4 File Offset: 0x000087C4
		private static IMobileAdsClient GetMobileAdsClient()
		{
			return (IMobileAdsClient)Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("MobileAdsInstance", BindingFlags.Static | BindingFlags.Public).Invoke(null, null);
		}

		// Token: 0x04000D0B RID: 3339
		private static readonly IMobileAdsClient client = MobileAds.GetMobileAdsClient();
	}
}
