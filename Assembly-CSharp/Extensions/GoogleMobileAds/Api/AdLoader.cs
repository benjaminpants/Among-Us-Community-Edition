using System;
using System.Collections.Generic;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000276 RID: 630
	public class AdLoader
	{
		// Token: 0x06000DED RID: 3565 RVA: 0x0003FA94 File Offset: 0x0003DC94
		private AdLoader(AdLoader.Builder builder)
		{
			this.AdUnitId = string.Copy(builder.AdUnitId);
			this.CustomNativeTemplateClickHandlers = new Dictionary<string, Action<CustomNativeTemplateAd, string>>(builder.CustomNativeTemplateClickHandlers);
			this.TemplateIds = new HashSet<string>(builder.TemplateIds);
			this.AdTypes = new HashSet<NativeAdType>(builder.AdTypes);
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildAdLoaderClient", BindingFlags.Static | BindingFlags.Public);
			this.adLoaderClient = (IAdLoaderClient)method.Invoke(null, new object[]
			{
				this
			});
			Utils.CheckInitialization();
			this.adLoaderClient.OnCustomNativeTemplateAdLoaded += delegate(object sender, CustomNativeEventArgs args)
			{
				this.OnCustomNativeTemplateAdLoaded(this, args);
			};
			this.adLoaderClient.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs args)
			{
				if (this.OnAdFailedToLoad != null)
				{
					this.OnAdFailedToLoad(this, args);
				}
			};
		}

		// Token: 0x1400002C RID: 44
		// (add) Token: 0x06000DEE RID: 3566 RVA: 0x0003FB54 File Offset: 0x0003DD54
		// (remove) Token: 0x06000DEF RID: 3567 RVA: 0x0003FB8C File Offset: 0x0003DD8C
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x1400002D RID: 45
		// (add) Token: 0x06000DF0 RID: 3568 RVA: 0x0003FBC4 File Offset: 0x0003DDC4
		// (remove) Token: 0x06000DF1 RID: 3569 RVA: 0x0003FBFC File Offset: 0x0003DDFC
		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x06000DF2 RID: 3570 RVA: 0x0000A090 File Offset: 0x00008290
		// (set) Token: 0x06000DF3 RID: 3571 RVA: 0x0000A098 File Offset: 0x00008298
		public Dictionary<string, Action<CustomNativeTemplateAd, string>> CustomNativeTemplateClickHandlers { get; private set; }

		// Token: 0x170001FC RID: 508
		// (get) Token: 0x06000DF4 RID: 3572 RVA: 0x0000A0A1 File Offset: 0x000082A1
		// (set) Token: 0x06000DF5 RID: 3573 RVA: 0x0000A0A9 File Offset: 0x000082A9
		public string AdUnitId { get; private set; }

		// Token: 0x170001FD RID: 509
		// (get) Token: 0x06000DF6 RID: 3574 RVA: 0x0000A0B2 File Offset: 0x000082B2
		// (set) Token: 0x06000DF7 RID: 3575 RVA: 0x0000A0BA File Offset: 0x000082BA
		public HashSet<NativeAdType> AdTypes { get; private set; }

		// Token: 0x170001FE RID: 510
		// (get) Token: 0x06000DF8 RID: 3576 RVA: 0x0000A0C3 File Offset: 0x000082C3
		// (set) Token: 0x06000DF9 RID: 3577 RVA: 0x0000A0CB File Offset: 0x000082CB
		public HashSet<string> TemplateIds { get; private set; }

		// Token: 0x06000DFA RID: 3578 RVA: 0x0000A0D4 File Offset: 0x000082D4
		public void LoadAd(AdRequest request)
		{
			this.adLoaderClient.LoadAd(request);
		}

		// Token: 0x04000CCD RID: 3277
		private IAdLoaderClient adLoaderClient;

		// Token: 0x02000277 RID: 631
		public class Builder
		{
			// Token: 0x06000DFD RID: 3581 RVA: 0x0000A108 File Offset: 0x00008308
			public Builder(string adUnitId)
			{
				this.AdUnitId = adUnitId;
				this.AdTypes = new HashSet<NativeAdType>();
				this.TemplateIds = new HashSet<string>();
				this.CustomNativeTemplateClickHandlers = new Dictionary<string, Action<CustomNativeTemplateAd, string>>();
			}

			// Token: 0x170001FF RID: 511
			// (get) Token: 0x06000DFE RID: 3582 RVA: 0x0000A138 File Offset: 0x00008338
			// (set) Token: 0x06000DFF RID: 3583 RVA: 0x0000A140 File Offset: 0x00008340
			internal string AdUnitId { get; private set; }

			// Token: 0x17000200 RID: 512
			// (get) Token: 0x06000E00 RID: 3584 RVA: 0x0000A149 File Offset: 0x00008349
			// (set) Token: 0x06000E01 RID: 3585 RVA: 0x0000A151 File Offset: 0x00008351
			internal HashSet<NativeAdType> AdTypes { get; private set; }

			// Token: 0x17000201 RID: 513
			// (get) Token: 0x06000E02 RID: 3586 RVA: 0x0000A15A File Offset: 0x0000835A
			// (set) Token: 0x06000E03 RID: 3587 RVA: 0x0000A162 File Offset: 0x00008362
			internal HashSet<string> TemplateIds { get; private set; }

			// Token: 0x17000202 RID: 514
			// (get) Token: 0x06000E04 RID: 3588 RVA: 0x0000A16B File Offset: 0x0000836B
			// (set) Token: 0x06000E05 RID: 3589 RVA: 0x0000A173 File Offset: 0x00008373
			internal Dictionary<string, Action<CustomNativeTemplateAd, string>> CustomNativeTemplateClickHandlers { get; private set; }

			// Token: 0x06000E06 RID: 3590 RVA: 0x0000A17C File Offset: 0x0000837C
			public AdLoader.Builder ForCustomNativeAd(string templateId)
			{
				this.TemplateIds.Add(templateId);
				this.AdTypes.Add(NativeAdType.CustomTemplate);
				return this;
			}

			// Token: 0x06000E07 RID: 3591 RVA: 0x0000A199 File Offset: 0x00008399
			public AdLoader.Builder ForCustomNativeAd(string templateId, Action<CustomNativeTemplateAd, string> callback)
			{
				this.TemplateIds.Add(templateId);
				this.CustomNativeTemplateClickHandlers[templateId] = callback;
				this.AdTypes.Add(NativeAdType.CustomTemplate);
				return this;
			}

			// Token: 0x06000E08 RID: 3592 RVA: 0x0000A1C3 File Offset: 0x000083C3
			public AdLoader Build()
			{
				return new AdLoader(this);
			}
		}
	}
}
