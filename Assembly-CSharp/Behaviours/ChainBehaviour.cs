using System;
using UnityEngine;

// Token: 0x020001CA RID: 458
public class ChainBehaviour : MonoBehaviour
{
	// Token: 0x060009EE RID: 2542 RVA: 0x00034248 File Offset: 0x00032448
	public void Awake()
	{
		this.swingTime = FloatRange.Next(0f, this.SwingPeriod);
		this.vec.z = this.SwingRange.Lerp(Mathf.Sin(this.swingTime));
		base.transform.eulerAngles = this.vec;
	}

	// Token: 0x060009EF RID: 2543 RVA: 0x000342A0 File Offset: 0x000324A0
	public void Update()
	{
		this.swingTime += Time.deltaTime / this.SwingPeriod;
		this.vec.z = this.SwingRange.Lerp(Mathf.Sin(this.swingTime * 3.14159274f) / 2f + 0.5f);
		base.transform.eulerAngles = this.vec;
	}

	// Token: 0x04000991 RID: 2449
	public FloatRange SwingRange = new FloatRange(0f, 30f);

	// Token: 0x04000992 RID: 2450
	public float SwingPeriod = 2f;

	// Token: 0x04000993 RID: 2451
	public float swingTime;

	// Token: 0x04000994 RID: 2452
	private Vector3 vec;
}
