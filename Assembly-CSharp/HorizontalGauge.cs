using System;
using UnityEngine;

// Token: 0x020000C1 RID: 193
public class HorizontalGauge : MonoBehaviour
{
	// Token: 0x0600040F RID: 1039 RVA: 0x00019D80 File Offset: 0x00017F80
	public void Update()
	{
		if (this.MaxValue != 0f && this.lastValue != this.Value)
		{
			this.lastValue = this.Value;
			float num = this.lastValue / this.MaxValue * this.maskScale;
			this.Mask.transform.localScale = new Vector3(num, 1f, 1f);
			this.Mask.transform.localPosition = new Vector3(-this.Mask.sprite.bounds.size.x * (this.maskScale - num) / 2f, 0f, 0f);
		}
	}

	// Token: 0x04000400 RID: 1024
	public float Value = 0.5f;

	// Token: 0x04000401 RID: 1025
	public float MaxValue = 1f;

	// Token: 0x04000402 RID: 1026
	public float maskScale = 1f;

	// Token: 0x04000403 RID: 1027
	public SpriteMask Mask;

	// Token: 0x04000404 RID: 1028
	private float lastValue = float.MinValue;
}
