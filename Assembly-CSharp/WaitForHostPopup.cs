using System;
using InnerNet;
using UnityEngine;

// Token: 0x02000232 RID: 562
public class WaitForHostPopup : DestroyableSingleton<WaitForHostPopup>
{
	// Token: 0x06000C00 RID: 3072 RVA: 0x00009312 File Offset: 0x00007512
	public void Show()
	{
		if (AmongUsClient.Instance && AmongUsClient.Instance.ClientId > 0)
		{
			this.Content.SetActive(true);
		}
	}

	// Token: 0x06000C01 RID: 3073 RVA: 0x00009339 File Offset: 0x00007539
	public void ExitGame()
	{
		AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
		this.Content.SetActive(false);
	}

	// Token: 0x06000C02 RID: 3074 RVA: 0x00009352 File Offset: 0x00007552
	public void Hide()
	{
		this.Content.SetActive(false);
	}

	// Token: 0x04000B93 RID: 2963
	public GameObject Content;
}
