using System;
using UnityEngine;

// Token: 0x020000FB RID: 251
public class ReportButtonManager : MonoBehaviour
{
	// Token: 0x06000557 RID: 1367 RVA: 0x00022BBC File Offset: 0x00020DBC
	public void SetActive(bool isActive)
	{
		if (isActive)
		{
			this.renderer.color = Palette.EnabledColor;
			this.renderer.material.SetFloat("_Desat", 0f);
			return;
		}
		this.renderer.color = Palette.DisabledColor;
		this.renderer.material.SetFloat("_Desat", 1f);
	}

	// Token: 0x06000558 RID: 1368 RVA: 0x0000560E File Offset: 0x0000380E
	public void DoClick()
	{
		if (!base.isActiveAndEnabled)
		{
			return;
		}
		PlayerControl.LocalPlayer.ReportClosest();
	}

	// Token: 0x0400051F RID: 1311
	public SpriteRenderer renderer;
}
