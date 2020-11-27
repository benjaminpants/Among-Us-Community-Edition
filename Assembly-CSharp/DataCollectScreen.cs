using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200012D RID: 301
public class DataCollectScreen : MonoBehaviour
{
	// Token: 0x06000649 RID: 1609 RVA: 0x00005F1F File Offset: 0x0000411F
	private void Start()
	{
		DataCollectScreen.Instance = this;
		this.UpdateButtons();
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00005F2D File Offset: 0x0000412D
	public IEnumerator Show()
	{
		if (!SaveManager.SendDataScreen)
		{
			base.gameObject.SetActive(true);
			while (base.gameObject.activeSelf)
			{
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00005F3C File Offset: 0x0000413C
	public void Close()
	{
		SaveManager.SendDataScreen = true;
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x00005F44 File Offset: 0x00004144
	public void ToggleSendTelemetry()
	{
		SaveManager.SendTelemetry = !SaveManager.SendTelemetry;
		this.UpdateButtons();
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x00005F59 File Offset: 0x00004159
	public void ToggleSendName()
	{
		SaveManager.SendName = !SaveManager.SendName;
		this.UpdateButtons();
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00005F6E File Offset: 0x0000416E
	public void UpdateButtons()
	{
		this.SendNameButton.UpdateText(SaveManager.SendName);
		this.SendTelemButton.UpdateText(SaveManager.SendTelemetry);
	}

	// Token: 0x04000632 RID: 1586
	public static DataCollectScreen Instance;

	// Token: 0x04000633 RID: 1587
	public ToggleButtonBehaviour SendNameButton;

	// Token: 0x04000634 RID: 1588
	public ToggleButtonBehaviour SendTelemButton;

	// Token: 0x04000635 RID: 1589
	public AdDataCollectScreen AdPolicy;
}
