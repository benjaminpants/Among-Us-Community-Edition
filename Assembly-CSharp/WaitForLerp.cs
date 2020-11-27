using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000060 RID: 96
public class WaitForLerp : IEnumerator
{
	// Token: 0x0600020A RID: 522 RVA: 0x000034C9 File Offset: 0x000016C9
	public WaitForLerp(float seconds, Action<float> act)
	{
		this.duration = seconds;
		this.act = act;
	}

	// Token: 0x17000062 RID: 98
	// (get) Token: 0x0600020B RID: 523 RVA: 0x000034DF File Offset: 0x000016DF
	public object Current
	{
		get
		{
			return null;
		}
	}

	// Token: 0x0600020C RID: 524 RVA: 0x00011F2C File Offset: 0x0001012C
	public bool MoveNext()
	{
		this.timer = Mathf.Min(this.timer + Time.deltaTime, this.duration);
		this.act(this.timer / this.duration);
		return this.timer < this.duration;
	}

	// Token: 0x0600020D RID: 525 RVA: 0x000034E2 File Offset: 0x000016E2
	public void Reset()
	{
		this.timer = 0f;
	}

	// Token: 0x040001F9 RID: 505
	private float duration;

	// Token: 0x040001FA RID: 506
	private float timer;

	// Token: 0x040001FB RID: 507
	private Action<float> act;
}
