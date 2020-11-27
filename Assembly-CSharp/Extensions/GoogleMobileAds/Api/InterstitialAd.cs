using System;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000280 RID: 640
	public class InterstitialAd
	{
		// Token: 0x06000E5E RID: 3678 RVA: 0x00040194 File Offset: 0x0003E394
		public InterstitialAd(string adUnitId)
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildInterstitialClient", BindingFlags.Static | BindingFlags.Public);
			this.client = (IInterstitialClient)method.Invoke(null, null);
			this.client.CreateInterstitialAd(adUnitId);
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

		// Token: 0x14000033 RID: 51
		// (add) Token: 0x06000E5F RID: 3679 RVA: 0x00040250 File Offset: 0x0003E450
		// (remove) Token: 0x06000E60 RID: 3680 RVA: 0x00040288 File Offset: 0x0003E488
		public event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000034 RID: 52
		// (add) Token: 0x06000E61 RID: 3681 RVA: 0x000402C0 File Offset: 0x0003E4C0
		// (remove) Token: 0x06000E62 RID: 3682 RVA: 0x000402F8 File Offset: 0x0003E4F8
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x14000035 RID: 53
		// (add) Token: 0x06000E63 RID: 3683 RVA: 0x00040330 File Offset: 0x0003E530
		// (remove) Token: 0x06000E64 RID: 3684 RVA: 0x00040368 File Offset: 0x0003E568
		public event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x14000036 RID: 54
		// (add) Token: 0x06000E65 RID: 3685 RVA: 0x000403A0 File Offset: 0x0003E5A0
		// (remove) Token: 0x06000E66 RID: 3686 RVA: 0x000403D8 File Offset: 0x0003E5D8
		public event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x14000037 RID: 55
		// (add) Token: 0x06000E67 RID: 3687 RVA: 0x00040410 File Offset: 0x0003E610
		// (remove) Token: 0x06000E68 RID: 3688 RVA: 0x00040448 File Offset: 0x0003E648
		public event EventHandler<EventArgs> OnAdLeavingApplication;

		// Token: 0x06000E69 RID: 3689 RVA: 0x0000A4D6 File Offset: 0x000086D6
		public void LoadAd(AdRequest request)
		{
			this.client.LoadAd(request);
		}

		// Token: 0x06000E6A RID: 3690 RVA: 0x0000A4E4 File Offset: 0x000086E4
		public bool IsLoaded()
		{
			return this.client.IsLoaded();
		}

		// Token: 0x06000E6B RID: 3691 RVA: 0x0000A4F1 File Offset: 0x000086F1
		public void Show()
		{
			this.client.ShowInterstitial();
		}

		// Token: 0x06000E6C RID: 3692 RVA: 0x0000A4FE File Offset: 0x000086FE
		public void Destroy()
		{
			this.client.DestroyInterstitial();
		}

		// Token: 0x06000E6D RID: 3693 RVA: 0x0000A50B File Offset: 0x0000870B
		public string MediationAdapterClassName()
		{
			return this.client.MediationAdapterClassName();
		}

		// Token: 0x04000D05 RID: 3333
		private IInterstitialClient client;
	}
}
