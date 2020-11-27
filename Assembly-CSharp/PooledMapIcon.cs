using System;
using UnityEngine;

// Token: 0x020000E2 RID: 226
public class PooledMapIcon : PoolableBehavior
{
	// Token: 0x060004C7 RID: 1223 RVA: 0x0001BCD0 File Offset: 0x00019ED0
	public void Update()
	{
		if (this.alphaPulse.enabled)
		{
			float num = Mathf.Abs(Mathf.Cos((this.alphaPulse.Offset + Time.time) * 3.14159274f / this.alphaPulse.Duration));
			if ((double)num > 0.9)
			{
				num -= 0.9f;
				num = this.NormalSize + num;
				base.transform.localScale = new Vector3(num, num, num);
			}
		}
	}

	// Token: 0x060004C8 RID: 1224 RVA: 0x00004F7D File Offset: 0x0000317D
	public override void Reset()
	{
		this.lastMapTaskStep = -1;
		this.alphaPulse.enabled = false;
		this.rend.material.SetFloat("_Outline", 0f);
		base.Reset();
	}

	// Token: 0x0400049A RID: 1178
	public float NormalSize = 0.3f;

	// Token: 0x0400049B RID: 1179
	public int lastMapTaskStep = -1;

	// Token: 0x0400049C RID: 1180
	public SpriteRenderer rend;

	// Token: 0x0400049D RID: 1181
	public AlphaPulse alphaPulse;
}
