using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000126 RID: 294
public class AdDataCollectScreen : MonoBehaviour
{
	// Token: 0x06000627 RID: 1575 RVA: 0x00005E0A File Offset: 0x0000400A
	private void Start()
	{
		this.UpdateButtons();
	}

	// Token: 0x06000628 RID: 1576 RVA: 0x00005E12 File Offset: 0x00004012
	public IEnumerator Show()
	{
		if (!SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.Accepted) && !SaveManager.BoughtNoAds)
		{
			base.gameObject.SetActive(true);
			while (base.gameObject.activeSelf)
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x06000629 RID: 1577 RVA: 0x00005E21 File Offset: 0x00004021
	public void Close()
	{
		SaveManager.ShowAdsScreen |= ShowAdsState.Accepted;
	}

	// Token: 0x0600062A RID: 1578 RVA: 0x00005E33 File Offset: 0x00004033
	public void Update()
	{
		if (SaveManager.BoughtNoAds)
		{
			base.GetComponent<TransitionOpen>().Close();
		}
	}

	// Token: 0x0600062B RID: 1579 RVA: 0x00025E98 File Offset: 0x00024098
	public void TogglePersonalizedAd()
	{
		ShowAdsState showAdsState = SaveManager.ShowAdsScreen & (ShowAdsState)127;
		if (showAdsState != ShowAdsState.Personalized)
		{
			if (showAdsState == ShowAdsState.NonPersonalized)
			{
				SaveManager.ShowAdsScreen = ShowAdsState.Personalized;
				goto IL_34;
			}
			if (showAdsState == ShowAdsState.Purchased)
			{
				SaveManager.ShowAdsScreen = ShowAdsState.Purchased;
				goto IL_34;
			}
		}
		SaveManager.ShowAdsScreen = ShowAdsState.NonPersonalized;
		IL_34:
		this.UpdateButtons();
	}

	// Token: 0x0600062C RID: 1580 RVA: 0x00005E47 File Offset: 0x00004047
	public void UpdateButtons()
	{
		this.PersonalizedAdsButton.UpdateText(!SaveManager.ShowAdsScreen.HasFlag(ShowAdsState.NonPersonalized));
	}

	// Token: 0x0400061A RID: 1562
	public ToggleButtonBehaviour PersonalizedAdsButton;
}
