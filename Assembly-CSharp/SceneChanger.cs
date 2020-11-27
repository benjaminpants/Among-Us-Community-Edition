using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x02000050 RID: 80
public class SceneChanger : MonoBehaviour
{
	// Token: 0x060001D6 RID: 470 RVA: 0x00003257 File Offset: 0x00001457
	public void Click()
	{
		this.BeforeSceneChange.Invoke();
		SceneChanger.ChangeScene(this.TargetScene);
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x0000326F File Offset: 0x0000146F
	public static void ChangeScene(string target)
	{
		SceneManager.LoadScene(target);
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00003277 File Offset: 0x00001477
	public void ExitGame()
	{
		Application.Quit();
	}

	// Token: 0x040001BA RID: 442
	public string TargetScene;

	// Token: 0x040001BB RID: 443
	public Button.ButtonClickedEvent BeforeSceneChange;
}
