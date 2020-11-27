using System;
using UnityEngine;

// Token: 0x02000154 RID: 340
public class MMOnlineManager : DestroyableSingleton<MMOnlineManager>
{
	// Token: 0x0600070E RID: 1806 RVA: 0x000065E8 File Offset: 0x000047E8
	public void Start()
	{
		if (this.HelpMenu)
		{
			if (SaveManager.ShowOnlineHelp)
			{
				SaveManager.ShowOnlineHelp = false;
				return;
			}
			this.HelpMenu.gameObject.SetActive(false);
		}
	}

	// Token: 0x0600070F RID: 1807 RVA: 0x00006616 File Offset: 0x00004816
	private void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
		{
			SceneChanger.ChangeScene("MainMenu");
		}
	}

	// Token: 0x040006CF RID: 1743
	public GameObject HelpMenu;
}
