using System.Collections;
using UnityEngine;

public class AdDataCollectScreen : MonoBehaviour
{
	public ToggleButtonBehaviour PersonalizedAdsButton;

	private void Start()
	{
		UpdateButtons();
	}

	public IEnumerator Show()
	{
		if (!SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.Accepted) && !SaveManager.BoughtNoAds)
		{
			base.gameObject.SetActive(value: true);
			while (base.gameObject.activeSelf)
			{
				yield return null;
			}
		}
	}

	public void Close()
	{
		SaveManager.ShowAdsScreen |= ShowAdsState.Accepted;
	}

	public void Update()
	{
		if (SaveManager.BoughtNoAds)
		{
			GetComponent<TransitionOpen>().Close();
		}
	}

	public void TogglePersonalizedAd()
	{
		switch (SaveManager.ShowAdsScreen & (ShowAdsState)127)
		{
		case ShowAdsState.NonPersonalized:
			SaveManager.ShowAdsScreen = ShowAdsState.Personalized;
			break;
		default:
			SaveManager.ShowAdsScreen = ShowAdsState.NonPersonalized;
			break;
		case ShowAdsState.Purchased:
			SaveManager.ShowAdsScreen = ShowAdsState.Purchased;
			break;
		}
		UpdateButtons();
	}

	public void UpdateButtons()
	{
		PersonalizedAdsButton.UpdateText(!SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.NonPersonalized));
	}
}
