using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000230 RID: 560
public class UnlockPopUp : MonoBehaviour
{
	// Token: 0x06000BF8 RID: 3064 RVA: 0x000092EC File Offset: 0x000074EC
	public IEnumerator Show()
	{
		DateTime utcNow = DateTime.UtcNow;
		if ((utcNow.DayOfYear < 350 && utcNow.DayOfYear > 4) || SaveManager.GetPurchase("hats_newyears2018"))
		{
			yield break;
		}
		base.gameObject.SetActive(true);
		SaveManager.SetPurchased("hats_newyears2018");
		while (base.isActiveAndEnabled)
		{
			yield return null;
		}
		yield break;
	}
}
