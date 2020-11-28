using System;
using UnityEngine;

// Token: 0x02000172 RID: 370
public class CourseStarBehaviour : MonoBehaviour
{
	// Token: 0x060007AA RID: 1962 RVA: 0x0002BBAC File Offset: 0x00029DAC
	public void Update()
	{
		this.Upper.transform.Rotate(0f, 0f, Time.deltaTime * this.Speed);
		this.Lower.transform.Rotate(0f, 0f, Time.deltaTime * this.Speed);
	}

	// Token: 0x0400078D RID: 1933
	public SpriteRenderer Upper;

	// Token: 0x0400078E RID: 1934
	public SpriteRenderer Lower;

	// Token: 0x0400078F RID: 1935
	public float Speed = 30f;
}
