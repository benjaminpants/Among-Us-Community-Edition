using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

// Token: 0x0200012F RID: 303
public class MainMenuManager : MonoBehaviour
{
	// Token: 0x06000656 RID: 1622 RVA: 0x000262F4 File Offset: 0x000244F4
	public void Start()
	{
		if (!MainMenuManager.SentTelemetry && SaveManager.SendTelemetry)
		{
			MainMenuManager.SentTelemetry = true;
			DateTime utcNow = DateTime.UtcNow;
			if (SaveManager.LastStartDate != DateTime.MinValue && SaveManager.LastStartDate < utcNow)
			{
				TimeSpan timeSpan = utcNow - SaveManager.LastStartDate;
				Analytics.CustomEvent("GameOpened", new Dictionary<string, object>
				{
					{
						"TotalMinutes",
						timeSpan.TotalMinutes
					}
				});
			}
			SaveManager.LastStartDate = utcNow;
		}
		base.StartCoroutine(this.Announcement.Init());
		base.StartCoroutine(this.RunStartUp());
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x00005FA7 File Offset: 0x000041A7
	private IEnumerator RunStartUp()
	{
		yield return this.DataPolicy.Show();
		yield return this.Announcement.Show();
		yield return this.UnlockPop.Show();
		DateTime utcNow = DateTime.UtcNow;
		for (int i = 0; i < DestroyableSingleton<HatManager>.Instance.AllHats.Count; i++)
		{
			HatBehaviour hatBehaviour = DestroyableSingleton<HatManager>.Instance.AllHats[i];
			if (hatBehaviour.LimitedMonth == utcNow.Month && !SaveManager.GetPurchase(hatBehaviour.ProductId))
			{
				SaveManager.SetPurchased(hatBehaviour.ProductId);
			}
		}
		yield break;
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x00005FB6 File Offset: 0x000041B6
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			Application.Quit();
		}
	}

	// Token: 0x04000639 RID: 1593
	public DataCollectScreen DataPolicy;

	// Token: 0x0400063A RID: 1594
	public AdDataCollectScreen AdsPolicy;

	// Token: 0x0400063B RID: 1595
	public AnnouncementPopUp Announcement;

	// Token: 0x0400063C RID: 1596
	public UnlockPopUp UnlockPop;

	// Token: 0x0400063D RID: 1597
	private static bool SentTelemetry;
}
