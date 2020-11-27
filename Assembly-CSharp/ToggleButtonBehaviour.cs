using System;
using UnityEngine;

// Token: 0x0200010A RID: 266
public class ToggleButtonBehaviour : MonoBehaviour
{
	// Token: 0x060005B3 RID: 1459 RVA: 0x000241C0 File Offset: 0x000223C0
	public void UpdateText(bool on)
	{
		Color color = on ? new Color(0f, 1f, 0.164705887f, 1f) : Color.white;
		this.Background.color = color;
		this.Text.Text = this.BaseText + ": " + (on ? "On" : "Off");
		if (this.Rollover)
		{
			this.Rollover.OutColor = color;
		}
	}

	// Token: 0x04000591 RID: 1425
	public string BaseText;

	// Token: 0x04000592 RID: 1426
	public TextRenderer Text;

	// Token: 0x04000593 RID: 1427
	public SpriteRenderer Background;

	// Token: 0x04000594 RID: 1428
	public ButtonRolloverHandler Rollover;
}
