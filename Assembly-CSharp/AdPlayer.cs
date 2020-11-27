using System;
using System.Collections;
using GoogleMobileAds.Api;
using InnerNet;
using UnityEngine;

// Token: 0x02000012 RID: 18
public static class AdPlayer
{
	// Token: 0x06000052 RID: 82 RVA: 0x00002498 File Offset: 0x00000698
	public static void ShowInterstitial(MonoBehaviour parent)
	{
		parent.StartCoroutine(AdPlayer.CoShowAd());
	}

	// Token: 0x06000053 RID: 83 RVA: 0x000024A6 File Offset: 0x000006A6
	private static IEnumerator CoShowAd()
	{
		if (TempData.playAgain)
		{
			yield return DestroyableSingleton<EndGameManager>.Instance.CoJoinGame();
		}
		else
		{
			AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
		}
		yield break;
	}

	// Token: 0x06000054 RID: 84 RVA: 0x0000BED4 File Offset: 0x0000A0D4
	public static void RequestInterstitial()
	{
		MobileAds.Initialize("unexpected_platform");
		if (AdPlayer.interstitial == null)
		{
			AdPlayer.interstitial = new InterstitialAd("unexpected_platform");
			AdRequest adRequest = new AdRequest.Builder().AddTestDevice("437025455EDB8E2BDE7B6F4837D3D19F").Build();
			if (SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.NonPersonalized))
			{
				adRequest.Extras.Add("npa", "1");
			}
			AdPlayer.interstitial.OnAdFailedToLoad += AdPlayer.Interstitial_OnAdFailedToLoad;
			AdPlayer.interstitial.OnAdClosed += AdPlayer.Interstitial_OnAdClosed;
			AdPlayer.interstitial.OnAdLeavingApplication += AdPlayer.Interstitial_OnAdLeavingApplication;
			AdPlayer.interstitial.LoadAd(adRequest);
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x0000BF94 File Offset: 0x0000A194
	private static void Interstitial_OnAdLeavingApplication(object sender, EventArgs e)
	{
		try
		{
			AdPlayer.interstitial.Destroy();
			Debug.LogError("Ad leaving app");
		}
		finally
		{
			AdPlayer.interstitial = null;
		}
	}

	// Token: 0x06000056 RID: 86 RVA: 0x0000BFD0 File Offset: 0x0000A1D0
	private static void Interstitial_OnAdFailedToLoad(object sender, AdFailedToLoadEventArgs e)
	{
		try
		{
			AdPlayer.interstitial.Destroy();
			Debug.LogError("Couldn't load ad: " + (e.Message ?? "No Message"));
		}
		finally
		{
			AdPlayer.interstitial = null;
		}
	}

	// Token: 0x06000057 RID: 87 RVA: 0x0000C020 File Offset: 0x0000A220
	private static void Interstitial_OnAdClosed(object sender, EventArgs e)
	{
		try
		{
			AdPlayer.interstitial.Destroy();
			Debug.LogError("Ad closed");
		}
		finally
		{
			AdPlayer.interstitial = null;
		}
	}

	// Token: 0x04000066 RID: 102
	private static InterstitialAd interstitial;

	// Token: 0x04000067 RID: 103
	private const string appId = "unexpected_platform";

	// Token: 0x04000068 RID: 104
	private const string adUnitId = "unexpected_platform";
}
