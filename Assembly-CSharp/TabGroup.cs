using System;
using UnityEngine;

// Token: 0x0200014E RID: 334
public class TabGroup : MonoBehaviour
{
	// Token: 0x060006F5 RID: 1781 RVA: 0x000064AD File Offset: 0x000046AD
	internal void Close()
	{
		this.Button.color = Color.white;
		this.Rollover.OutColor = Color.white;
		this.Content.SetActive(false);
	}

	// Token: 0x060006F6 RID: 1782 RVA: 0x000064DB File Offset: 0x000046DB
	internal void Open()
	{
		this.Button.color = Color.green;
		this.Rollover.OutColor = Color.green;
		this.Content.SetActive(true);
	}

	// Token: 0x040006BF RID: 1727
	public SpriteRenderer Button;

	// Token: 0x040006C0 RID: 1728
	public ButtonRolloverHandler Rollover;

	// Token: 0x040006C1 RID: 1729
	public GameObject Content;
}
