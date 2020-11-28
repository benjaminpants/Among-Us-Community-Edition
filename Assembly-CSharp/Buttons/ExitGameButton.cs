using System;
using InnerNet;
using UnityEngine;

// Token: 0x02000135 RID: 309
public class ExitGameButton : MonoBehaviour
{
	// Token: 0x06000688 RID: 1672 RVA: 0x0000611A File Offset: 0x0000431A
	public void Start()
	{
		if (!DestroyableSingleton<HudManager>.InstanceExists)
		{
			base.gameObject.SetActive(false);
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0000612F File Offset: 0x0000432F
	public void OnClick()
	{
		if (AmongUsClient.Instance)
		{
			AmongUsClient.Instance.ExitGame(DisconnectReasons.ExitGame);
			return;
		}
		SceneChanger.ChangeScene("MainMenu");
	}
}
