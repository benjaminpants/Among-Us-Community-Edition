using System;
using System.Reflection;
using GoogleMobileAds.Common;

namespace GoogleMobileAds.Api
{
	// Token: 0x02000283 RID: 643
	public class RewardBasedVideoAd
	{
		// Token: 0x17000217 RID: 535
		// (get) Token: 0x06000E7F RID: 3711 RVA: 0x0000A616 File Offset: 0x00008816
		public static RewardBasedVideoAd Instance
		{
			get
			{
				return RewardBasedVideoAd.instance;
			}
		}

		// Token: 0x06000E80 RID: 3712 RVA: 0x00040480 File Offset: 0x0003E680
		private RewardBasedVideoAd()
		{
			MethodInfo method = Type.GetType("GoogleMobileAds.GoogleMobileAdsClientFactory,Assembly-CSharp").GetMethod("BuildRewardBasedVideoAdClient", BindingFlags.Static | BindingFlags.Public);
			this.client = (IRewardBasedVideoAdClient)method.Invoke(null, null);
			this.client.CreateRewardBasedVideoAd();
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
			this.client.OnAdStarted += delegate(object sender, EventArgs args)
			{
				if (this.OnAdStarted != null)
				{
					this.OnAdStarted(this, args);
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
			this.client.OnAdRewarded += delegate(object sender, Reward args)
			{
				if (this.OnAdRewarded != null)
				{
					this.OnAdRewarded(this, args);
				}
			};
			this.client.OnAdCompleted += delegate(object sender, EventArgs args)
			{
				if (this.OnAdCompleted != null)
				{
					this.OnAdCompleted(this, args);
				}
			};
		}

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x06000E81 RID: 3713 RVA: 0x00040580 File Offset: 0x0003E780
		// (remove) Token: 0x06000E82 RID: 3714 RVA: 0x000405B8 File Offset: 0x0003E7B8
		public event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000039 RID: 57
		// (add) Token: 0x06000E83 RID: 3715 RVA: 0x000405F0 File Offset: 0x0003E7F0
		// (remove) Token: 0x06000E84 RID: 3716 RVA: 0x00040628 File Offset: 0x0003E828
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x1400003A RID: 58
		// (add) Token: 0x06000E85 RID: 3717 RVA: 0x00040660 File Offset: 0x0003E860
		// (remove) Token: 0x06000E86 RID: 3718 RVA: 0x00040698 File Offset: 0x0003E898
		public event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x1400003B RID: 59
		// (add) Token: 0x06000E87 RID: 3719 RVA: 0x000406D0 File Offset: 0x0003E8D0
		// (remove) Token: 0x06000E88 RID: 3720 RVA: 0x00040708 File Offset: 0x0003E908
		public event EventHandler<EventArgs> OnAdStarted;

		// Token: 0x1400003C RID: 60
		// (add) Token: 0x06000E89 RID: 3721 RVA: 0x00040740 File Offset: 0x0003E940
		// (remove) Token: 0x06000E8A RID: 3722 RVA: 0x00040778 File Offset: 0x0003E978
		public event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x1400003D RID: 61
		// (add) Token: 0x06000E8B RID: 3723 RVA: 0x000407B0 File Offset: 0x0003E9B0
		// (remove) Token: 0x06000E8C RID: 3724 RVA: 0x000407E8 File Offset: 0x0003E9E8
		public event EventHandler<Reward> OnAdRewarded;

		// Token: 0x1400003E RID: 62
		// (add) Token: 0x06000E8D RID: 3725 RVA: 0x00040820 File Offset: 0x0003EA20
		// (remove) Token: 0x06000E8E RID: 3726 RVA: 0x00040858 File Offset: 0x0003EA58
		public event EventHandler<EventArgs> OnAdLeavingApplication;

		// Token: 0x1400003F RID: 63
		// (add) Token: 0x06000E8F RID: 3727 RVA: 0x00040890 File Offset: 0x0003EA90
		// (remove) Token: 0x06000E90 RID: 3728 RVA: 0x000408C8 File Offset: 0x0003EAC8
		public event EventHandler<EventArgs> OnAdCompleted;

		// Token: 0x06000E91 RID: 3729 RVA: 0x0000A61D File Offset: 0x0000881D
		public void LoadAd(AdRequest request, string adUnitId)
		{
			this.client.LoadAd(request, adUnitId);
		}

		// Token: 0x06000E92 RID: 3730 RVA: 0x0000A62C File Offset: 0x0000882C
		public bool IsLoaded()
		{
			return this.client.IsLoaded();
		}

		// Token: 0x06000E93 RID: 3731 RVA: 0x0000A639 File Offset: 0x00008839
		public void Show()
		{
			this.client.ShowRewardBasedVideoAd();
		}

		// Token: 0x06000E94 RID: 3732 RVA: 0x0000A646 File Offset: 0x00008846
		public void SetUserId(string userId)
		{
			this.client.SetUserId(userId);
		}

		// Token: 0x06000E95 RID: 3733 RVA: 0x0000A654 File Offset: 0x00008854
		public string MediationAdapterClassName()
		{
			return this.client.MediationAdapterClassName();
		}

		// Token: 0x04000D0E RID: 3342
		private IRewardBasedVideoAdClient client;

		// Token: 0x04000D0F RID: 3343
		private static readonly RewardBasedVideoAd instance = new RewardBasedVideoAd();
	}
}
