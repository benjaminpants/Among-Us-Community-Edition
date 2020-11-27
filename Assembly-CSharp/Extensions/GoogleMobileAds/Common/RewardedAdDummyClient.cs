using System;
using System.Reflection;
using GoogleMobileAds.Api;
using UnityEngine;

namespace GoogleMobileAds.Common
{
	// Token: 0x0200026F RID: 623
	public class RewardedAdDummyClient : IRewardedAdClient
	{
		// Token: 0x06000DCB RID: 3531 RVA: 0x00009EC5 File Offset: 0x000080C5
		public RewardedAdDummyClient()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x14000026 RID: 38
		// (add) Token: 0x06000DCC RID: 3532 RVA: 0x0003F7F4 File Offset: 0x0003D9F4
		// (remove) Token: 0x06000DCD RID: 3533 RVA: 0x0003F82C File Offset: 0x0003DA2C
		public event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000027 RID: 39
		// (add) Token: 0x06000DCE RID: 3534 RVA: 0x0003F864 File Offset: 0x0003DA64
		// (remove) Token: 0x06000DCF RID: 3535 RVA: 0x0003F89C File Offset: 0x0003DA9C
		public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

		// Token: 0x14000028 RID: 40
		// (add) Token: 0x06000DD0 RID: 3536 RVA: 0x0003F8D4 File Offset: 0x0003DAD4
		// (remove) Token: 0x06000DD1 RID: 3537 RVA: 0x0003F90C File Offset: 0x0003DB0C
		public event EventHandler<AdErrorEventArgs> OnAdFailedToShow;

		// Token: 0x14000029 RID: 41
		// (add) Token: 0x06000DD2 RID: 3538 RVA: 0x0003F944 File Offset: 0x0003DB44
		// (remove) Token: 0x06000DD3 RID: 3539 RVA: 0x0003F97C File Offset: 0x0003DB7C
		public event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x1400002A RID: 42
		// (add) Token: 0x06000DD4 RID: 3540 RVA: 0x0003F9B4 File Offset: 0x0003DBB4
		// (remove) Token: 0x06000DD5 RID: 3541 RVA: 0x0003F9EC File Offset: 0x0003DBEC
		public event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x1400002B RID: 43
		// (add) Token: 0x06000DD6 RID: 3542 RVA: 0x0003FA24 File Offset: 0x0003DC24
		// (remove) Token: 0x06000DD7 RID: 3543 RVA: 0x0003FA5C File Offset: 0x0003DC5C
		public event EventHandler<Reward> OnUserEarnedReward;

		// Token: 0x06000DD8 RID: 3544 RVA: 0x00009F06 File Offset: 0x00008106
		public void CreateRewardedAd(string adUnitId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000DD9 RID: 3545 RVA: 0x00009F06 File Offset: 0x00008106
		public void LoadAd(AdRequest request)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000DDA RID: 3546 RVA: 0x00009F41 File Offset: 0x00008141
		public bool IsLoaded()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return true;
		}

		// Token: 0x06000DDB RID: 3547 RVA: 0x00009F06 File Offset: 0x00008106
		public void Show()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000DDC RID: 3548 RVA: 0x00009F5D File Offset: 0x0000815D
		public string MediationAdapterClassName()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return null;
		}
	}
}
