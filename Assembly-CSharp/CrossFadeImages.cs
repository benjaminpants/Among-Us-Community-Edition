using System;
using UnityEngine;

// Token: 0x02000067 RID: 103
public class CrossFadeImages : MonoBehaviour
{
	// Token: 0x0600022E RID: 558 RVA: 0x00012474 File Offset: 0x00010674
	private void Update()
	{
		Color white = Color.white;
		white.a = Mathf.Clamp((Mathf.Sin(3.14159274f * Time.time / this.Period) + 0.75f) * 0.75f, 0f, 1f);
		this.Image1.color = white;
		white.a = 1f - white.a;
		this.Image2.color = white;
	}

	// Token: 0x04000211 RID: 529
	public SpriteRenderer Image1;

	// Token: 0x04000212 RID: 530
	public SpriteRenderer Image2;

	// Token: 0x04000213 RID: 531
	public float Period = 5f;
}
