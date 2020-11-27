using System;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000284 RID: 644
	public class RewardedAd
	{
		// Token: 0x06000E9F RID: 3743 RVA: 0x00040900 File Offset: 0x0003EB00
		public RewardedAd(string adUnitId)
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildRewardedAdClient", BindingFlags.Static | BindingFlags.Public);
			this.client = (IRewardedAdClient)method.Invoke(null, null);
			this.client.CreateRewardedAd(adUnitId);
			this.client.OnAdLoaded += delegate(object sender, EventArgs args)
			{
				if (this.OnAdLoaded != null)
				{
					this.OnAdLoaded(this, args);
				}
			};
			this.client.OnAdFailedToLoad += delegate(object sender, AdErrorEventArgs args)
			{
				if (this.OnAdFailedToLoad != null)
				{
					this.OnAdFailedToLoad(this, args);
				}
			};
			this.client.OnAdFailedToShow += delegate(object sender, AdErrorEventArgs args)
			{
				if (this.OnAdFailedToShow != null)
				{
					this.OnAdFailedToShow(this, args);
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
			this.client.OnUserEarnedReward += delegate(object sender, Reward args)
			{
				if (this.OnUserEarnedReward != null)
				{
					this.OnUserEarnedReward(this, args);
				}
			};
		}

		// Token: 0x14000040 RID: 64
		// (add) Token: 0x06000EA0 RID: 3744 RVA: 0x000409D4 File Offset: 0x0003EBD4
		// (remove) Token: 0x06000EA1 RID: 3745 RVA: 0x00040A0C File Offset: 0x0003EC0C
		public event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000041 RID: 65
		// (add) Token: 0x06000EA2 RID: 3746 RVA: 0x00040A44 File Offset: 0x0003EC44
		// (remove) Token: 0x06000EA3 RID: 3747 RVA: 0x00040A7C File Offset: 0x0003EC7C
		public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x06000EA4 RID: 3748 RVA: 0x00040AB4 File Offset: 0x0003ECB4
		// (remove) Token: 0x06000EA5 RID: 3749 RVA: 0x00040AEC File Offset: 0x0003ECEC
		public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		// Token: 0x14000043 RID: 67
		// (add) Token: 0x06000EA6 RID: 3750 RVA: 0x00040B24 File Offset: 0x0003ED24
		// (remove) Token: 0x06000EA7 RID: 3751 RVA: 0x00040B5C File Offset: 0x0003ED5C
		public event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x14000044 RID: 68
		// (add) Token: 0x06000EA8 RID: 3752 RVA: 0x00040B94 File Offset: 0x0003ED94
		// (remove) Token: 0x06000EA9 RID: 3753 RVA: 0x00040BCC File Offset: 0x0003EDCC
		public event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x06000EAA RID: 3754 RVA: 0x00040C04 File Offset: 0x0003EE04
		// (remove) Token: 0x06000EAB RID: 3755 RVA: 0x00040C3C File Offset: 0x0003EE3C
		public event EventHandler<Reward> OnUserEarnedReward;

		// Token: 0x06000EAC RID: 3756 RVA: 0x0000A725 File Offset: 0x00008925
		public void LoadAd(AdRequest request)
		{
			this.client.LoadAd(request);
		}

		// Token: 0x06000EAD RID: 3757 RVA: 0x0000A733 File Offset: 0x00008933
		public bool IsLoaded()
		{
			return this.client.IsLoaded();
		}

		// Token: 0x06000EAE RID: 3758 RVA: 0x0000A740 File Offset: 0x00008940
		public void Show()
		{
			this.client.Show();
		}

		// Token: 0x06000EAF RID: 3759 RVA: 0x0000A74D File Offset: 0x0000894D
		public string MediationAdapterClassName()
		{
			return this.client.MediationAdapterClassName();
		}

		// Token: 0x04000D18 RID: 3352
		private IRewardedAdClient client;
	}
}
