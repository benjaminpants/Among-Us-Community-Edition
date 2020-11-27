using System;
using System.Reflection;
using GoogleMobileAds.Api;
using UnityEngine;

namespace GoogleMobileAds.Common
{
	// Token: 0x02000266 RID: 614
	public class DummyClient : IBannerClient, IInterstitialClient, IRewardBasedVideoAdClient, IAdLoaderClient, IMobileAdsClient
	{
		// Token: 0x06000D38 RID: 3384 RVA: 0x00009EC5 File Offset: 0x000080C5
		public DummyClient()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x14000003 RID: 3
		// (add) Token: 0x06000D39 RID: 3385 RVA: 0x0003F318 File Offset: 0x0003D518
		// (remove) Token: 0x06000D3A RID: 3386 RVA: 0x0003F350 File Offset: 0x0003D550
		public event EventHandler<EventArgs> OnAdLoaded;

		// Token: 0x14000004 RID: 4
		// (add) Token: 0x06000D3B RID: 3387 RVA: 0x0003F388 File Offset: 0x0003D588
		// (remove) Token: 0x06000D3C RID: 3388 RVA: 0x0003F3C0 File Offset: 0x0003D5C0
		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		// Token: 0x14000005 RID: 5
		// (add) Token: 0x06000D3D RID: 3389 RVA: 0x0003F3F8 File Offset: 0x0003D5F8
		// (remove) Token: 0x06000D3E RID: 3390 RVA: 0x0003F430 File Offset: 0x0003D630
		public event EventHandler<EventArgs> OnAdOpening;

		// Token: 0x14000006 RID: 6
		// (add) Token: 0x06000D3F RID: 3391 RVA: 0x0003F468 File Offset: 0x0003D668
		// (remove) Token: 0x06000D40 RID: 3392 RVA: 0x0003F4A0 File Offset: 0x0003D6A0
		public event EventHandler<EventArgs> OnAdStarted;

		// Token: 0x14000007 RID: 7
		// (add) Token: 0x06000D41 RID: 3393 RVA: 0x0003F4D8 File Offset: 0x0003D6D8
		// (remove) Token: 0x06000D42 RID: 3394 RVA: 0x0003F510 File Offset: 0x0003D710
		public event EventHandler<EventArgs> OnAdClosed;

		// Token: 0x14000008 RID: 8
		// (add) Token: 0x06000D43 RID: 3395 RVA: 0x0003F548 File Offset: 0x0003D748
		// (remove) Token: 0x06000D44 RID: 3396 RVA: 0x0003F580 File Offset: 0x0003D780
		public event EventHandler<Reward> OnAdRewarded;

		// Token: 0x14000009 RID: 9
		// (add) Token: 0x06000D45 RID: 3397 RVA: 0x0003F5B8 File Offset: 0x0003D7B8
		// (remove) Token: 0x06000D46 RID: 3398 RVA: 0x0003F5F0 File Offset: 0x0003D7F0
		public event EventHandler<EventArgs> OnAdLeavingApplication;

		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06000D47 RID: 3399 RVA: 0x0003F628 File Offset: 0x0003D828
		// (remove) Token: 0x06000D48 RID: 3400 RVA: 0x0003F660 File Offset: 0x0003D860
		public event EventHandler<EventArgs> OnAdCompleted;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06000D49 RID: 3401 RVA: 0x0003F698 File Offset: 0x0003D898
		// (remove) Token: 0x06000D4A RID: 3402 RVA: 0x0003F6D0 File Offset: 0x0003D8D0
		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x06000D4B RID: 3403 RVA: 0x00009EE6 File Offset: 0x000080E6
		// (set) Token: 0x06000D4C RID: 3404 RVA: 0x00009F06 File Offset: 0x00008106
		public string UserId
		{
			get
			{
				Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
				return "UserId";
			}
			set
			{
				Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			}
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x00009F06 File Offset: 0x00008106
		public void Initialize(string appId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x00009F06 File Offset: 0x00008106
		public void SetApplicationMuted(bool muted)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x00009F06 File Offset: 0x00008106
		public void SetApplicationVolume(float volume)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x00009F06 File Offset: 0x00008106
		public void SetiOSAppPauseOnBackground(bool pause)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D51 RID: 3409 RVA: 0x00009F06 File Offset: 0x00008106
		public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D52 RID: 3410 RVA: 0x00009F06 File Offset: 0x00008106
		public void CreateBannerView(string adUnitId, AdSize adSize, int positionX, int positionY)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D53 RID: 3411 RVA: 0x00009F06 File Offset: 0x00008106
		public void LoadAd(AdRequest request)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x00009F06 File Offset: 0x00008106
		public void ShowBannerView()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x00009F06 File Offset: 0x00008106
		public void HideBannerView()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x00009F06 File Offset: 0x00008106
		public void DestroyBannerView()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D57 RID: 3415 RVA: 0x00009F21 File Offset: 0x00008121
		public float GetHeightInPixels()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0f;
		}

		// Token: 0x06000D58 RID: 3416 RVA: 0x00009F21 File Offset: 0x00008121
		public float GetWidthInPixels()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0f;
		}

		// Token: 0x06000D59 RID: 3417 RVA: 0x00009F06 File Offset: 0x00008106
		public void SetPosition(AdPosition adPosition)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x00009F06 File Offset: 0x00008106
		public void SetPosition(int x, int y)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x00009F06 File Offset: 0x00008106
		public void CreateInterstitialAd(string adUnitId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x00009F41 File Offset: 0x00008141
		public bool IsLoaded()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return true;
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x00009F06 File Offset: 0x00008106
		public void ShowInterstitial()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D5E RID: 3422 RVA: 0x00009F06 File Offset: 0x00008106
		public void DestroyInterstitial()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D5F RID: 3423 RVA: 0x00009F06 File Offset: 0x00008106
		public void CreateRewardBasedVideoAd()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D60 RID: 3424 RVA: 0x00009F06 File Offset: 0x00008106
		public void SetUserId(string userId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D61 RID: 3425 RVA: 0x00009F06 File Offset: 0x00008106
		public void LoadAd(AdRequest request, string adUnitId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D62 RID: 3426 RVA: 0x00009F06 File Offset: 0x00008106
		public void DestroyRewardBasedVideoAd()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D63 RID: 3427 RVA: 0x00009F06 File Offset: 0x00008106
		public void ShowRewardBasedVideoAd()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D64 RID: 3428 RVA: 0x00009F06 File Offset: 0x00008106
		public void CreateAdLoader(AdLoader.Builder builder)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D65 RID: 3429 RVA: 0x00009F06 File Offset: 0x00008106
		public void Load(AdRequest request)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D66 RID: 3430 RVA: 0x00009F06 File Offset: 0x00008106
		public void SetAdSize(AdSize adSize)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		// Token: 0x06000D67 RID: 3431 RVA: 0x00009F5D File Offset: 0x0000815D
		public string MediationAdapterClassName()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return null;
		}
	}
}
