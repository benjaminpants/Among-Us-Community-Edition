using System;
using System.Reflection;
using GoogleMobileAds.Api;
using UnityEngine;

namespace GoogleMobileAds.Common
{
	public class DummyClient : IBannerClient, IInterstitialClient, IRewardBasedVideoAdClient, IAdLoaderClient, IMobileAdsClient
	{
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

		public event EventHandler<EventArgs> OnAdLoaded;

		public event EventHandler<AdFailedToLoadEventArgs> OnAdFailedToLoad;

		public event EventHandler<EventArgs> OnAdOpening;

		public event EventHandler<EventArgs> OnAdStarted;

		public event EventHandler<EventArgs> OnAdClosed;

		public event EventHandler<Reward> OnAdRewarded;

		public event EventHandler<EventArgs> OnAdLeavingApplication;

		public event EventHandler<EventArgs> OnAdCompleted;

		public event EventHandler<CustomNativeEventArgs> OnCustomNativeTemplateAdLoaded;

		public DummyClient()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void Initialize(string appId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetApplicationMuted(bool muted)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetApplicationVolume(float volume)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetiOSAppPauseOnBackground(bool pause)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, AdPosition position)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateBannerView(string adUnitId, AdSize adSize, int positionX, int positionY)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void LoadAd(AdRequest request)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void ShowBannerView()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void HideBannerView()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void DestroyBannerView()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public float GetHeightInPixels()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0f;
		}

		public float GetWidthInPixels()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return 0f;
		}

		public void SetPosition(AdPosition adPosition)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetPosition(int x, int y)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateInterstitialAd(string adUnitId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public bool IsLoaded()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return true;
		}

		public void ShowInterstitial()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void DestroyInterstitial()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateRewardBasedVideoAd()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetUserId(string userId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void LoadAd(AdRequest request, string adUnitId)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void DestroyRewardBasedVideoAd()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void ShowRewardBasedVideoAd()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void CreateAdLoader(AdLoader.Builder builder)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void Load(AdRequest request)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public void SetAdSize(AdSize adSize)
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
		}

		public string MediationAdapterClassName()
		{
			Debug.Log("Dummy " + MethodBase.GetCurrentMethod().Name);
			return null;
		}
	}
}
