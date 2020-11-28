using System;
using UnityEngine;

// Token: 0x02000009 RID: 9
public class TowerBehaviour : MonoBehaviour
{
	// Token: 0x06000024 RID: 36 RVA: 0x0000B578 File Offset: 0x00009778
	public void Update()
	{
		this.timer += Time.deltaTime;
		if (this.timer < this.frameTime)
		{
			this.circle.color = Color.white;
			this.middle1.color = (this.middle2.color = (this.outer1.color = (this.outer2.color = Color.black)));
			return;
		}
		if (this.timer < 2f * this.frameTime)
		{
			this.middle1.color = (this.middle2.color = Color.white);
			this.circle.color = (this.outer1.color = (this.outer2.color = Color.black));
			return;
		}
		if (this.timer < 3f * this.frameTime)
		{
			this.outer1.color = (this.outer2.color = Color.white);
			this.middle1.color = (this.middle2.color = (this.circle.color = Color.black));
			return;
		}
		this.timer = 0f;
	}

	// Token: 0x04000031 RID: 49
	public float timer;

	// Token: 0x04000032 RID: 50
	public float frameTime = 0.2f;

	// Token: 0x04000033 RID: 51
	public SpriteRenderer circle;

	// Token: 0x04000034 RID: 52
	public SpriteRenderer middle1;

	// Token: 0x04000035 RID: 53
	public SpriteRenderer middle2;

	// Token: 0x04000036 RID: 54
	public SpriteRenderer outer1;

	// Token: 0x04000037 RID: 55
	public SpriteRenderer outer2;
}
