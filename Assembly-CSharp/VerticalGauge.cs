using System;
using UnityEngine;

// Token: 0x02000110 RID: 272
public class VerticalGauge : MonoBehaviour
{
	// Token: 0x060005CE RID: 1486 RVA: 0x00024620 File Offset: 0x00022820
	public void Update()
	{
		if (this.lastValue != this.value)
		{
			this.lastValue = this.value;
			float num = Mathf.Clamp(this.lastValue / this.MaxValue, 0f, 1f) * this.maskScale;
			Vector3 localScale = this.Mask.transform.localScale;
			localScale.y = num;
			this.Mask.transform.localScale = localScale;
			this.Mask.transform.localPosition = new Vector3(0f, -this.Mask.sprite.bounds.size.y * (this.maskScale - num) / 2f, 0f);
		}
	}

	// Token: 0x040005AD RID: 1453
	public float value = 0.5f;

	// Token: 0x040005AE RID: 1454
	public float MaxValue = 1f;

	// Token: 0x040005AF RID: 1455
	public float maskScale = 1f;

	// Token: 0x040005B0 RID: 1456
	public SpriteMask Mask;

	// Token: 0x040005B1 RID: 1457
	private float lastValue = float.MinValue;
}
