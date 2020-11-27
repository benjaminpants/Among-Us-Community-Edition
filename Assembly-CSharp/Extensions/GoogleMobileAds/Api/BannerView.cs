using System;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	// Token: 0x0200027C RID: 636
	public class BannerView
	{
		// Token: 0x06000E39 RID: 3641 RVA: 0x0003FE14 File Offset: 0x0003E014
		public BannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildBannerClient", BindingFlags.Static | BindingFlags.Public);
			this.client = (IBannerClient)method.Invoke(null, null);
			this.client.CreateBannerView(adUnitId, adSize, position);
			this.ConfigureBannerEvents();
		}

		// Token: 0x06000E3A RID: 3642 RVA: 0x0003FE68 File Offset: 0x0003E068
		public BannerView(string adUnitId, AdSize adSize, int x, int y)
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildBannerClient", BindingFlags.Static | BindingFlags.Public);
			this.client = (IBannerClient)method.Invoke(null, null);
			this.client.CreateBannerView(adUnitId, adSize, x, y);
			this.ConfigureBannerEvents();
		}

		// Token: 0x1400002E RID: 46
		// (add) Token: 0x06000E3B RID: 3643 RVA: 0x0003FEBC File Offset: 0x0003E0BC
		// (remove) Token: 0x06000E3C RID: 3644 RVA: 0x0003FEF4 File Offset: 0x0003E0F4
		public event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x1400002F RID: 47
		// (add) Token: 0x06000E3D RID: 3645 RVA: 0x0003FF2C File Offset: 0x0003E12C
		// (remove) Token: 0x06000E3E RID: 3646 RVA: 0x0003FF64 File Offset: 0x0003E164
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x14000030 RID: 48
		// (add) Token: 0x06000E3F RID: 3647 RVA: 0x0003FF9C File Offset: 0x0003E19C
		// (remove) Token: 0x06000E40 RID: 3648 RVA: 0x0003FFD4 File Offset: 0x0003E1D4
		public event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x14000031 RID: 49
		// (add) Token: 0x06000E41 RID: 3649 RVA: 0x0004000C File Offset: 0x0003E20C
		// (remove) Token: 0x06000E42 RID: 3650 RVA: 0x00040044 File Offset: 0x0003E244
		public event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x14000032 RID: 50
		// (add) Token: 0x06000E43 RID: 3651 RVA: 0x0004007C File Offset: 0x0003E27C
		// (remove) Token: 0x06000E44 RID: 3652 RVA: 0x000400B4 File Offset: 0x0003E2B4
		public event EventHandler<EventArgs> OnAdLeavingApplication;

		// Token: 0x06000E45 RID: 3653 RVA: 0x0000A387 File Offset: 0x00008587
		public void LoadAd(AdRequest request)
		{
			this.client.LoadAd(request);
		}

		// Token: 0x06000E46 RID: 3654 RVA: 0x0000A395 File Offset: 0x00008595
		public void Hide()
		{
			this.client.HideBannerView();
		}

		// Token: 0x06000E47 RID: 3655 RVA: 0x0000A3A2 File Offset: 0x000085A2
		public void Show()
		{
			this.client.ShowBannerView();
		}

		// Token: 0x06000E48 RID: 3656 RVA: 0x0000A3AF File Offset: 0x000085AF
		public void Destroy()
		{
			this.client.DestroyBannerView();
		}

		// Token: 0x06000E49 RID: 3657 RVA: 0x0000A3BC File Offset: 0x000085BC
		public float GetHeightInPixels()
		{
			return this.client.GetHeightInPixels();
		}

		// Token: 0x06000E4A RID: 3658 RVA: 0x0000A3C9 File Offset: 0x000085C9
		public float GetWidthInPixels()
		{
			return this.client.GetWidthInPixels();
		}

		// Token: 0x06000E4B RID: 3659 RVA: 0x0000A3D6 File Offset: 0x000085D6
		public void SetPosition(AdPosition adPosition)
		{
			this.client.SetPosition(adPosition);
		}

		// Token: 0x06000E4C RID: 3660 RVA: 0x0000A3E4 File Offset: 0x000085E4
		public void SetPosition(int x, int y)
		{
			this.client.SetPosition(x, y);
		}

		// Token: 0x06000E4D RID: 3661 RVA: 0x000400EC File Offset: 0x0003E2EC
		private void ConfigureBannerEvents()
		{
			this.client.OnAdLoaded += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLoaded != null)
				{
					this.OnAdLoaded(this, args);
				}
			};
			this.client.OnAdFailedToLoad += delegate(object sender, AdFailedToLoadEventArgs args)
			{
				if (this.OnAdFailedToLoad != null)
				{
					this.OnAdFailedToLoad(this, args);
				}
			};
			this.client.OnAdOpening += delegate(object sender, EventArgs args)
			{
				if (this.OnAdOpening != null)
				{
					this.OnAdOpening(this, args);
				}
			};
			this.client.OnAdClosed += delegate(object sender, EventArgs args)
			{
				if (this.OnAdClosed != null)
				{
					this.OnAdClosed(this, args);
				}
			};
			this.client.OnAdLeavingApplication += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLeavingApplication != null)
				{
					this.OnAdLeavingApplication(this, args);
				}
			};
		}

		// Token: 0x06000E4E RID: 3662 RVA: 0x0000A3F3 File Offset: 0x000085F3
		public string MediationAdapterClassName()
		{
			return this.client.MediationAdapterClassName();
		}

		// Token: 0x04000CF9 RID: 3321
		private IBannerClient client;
	}
}
