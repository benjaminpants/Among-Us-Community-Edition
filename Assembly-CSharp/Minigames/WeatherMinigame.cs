using System;
using UnityEngine;

// Token: 0x02000014 RID: 20
public class WeatherMinigame : Minigame
{
	// Token: 0x0600005E RID: 94 RVA: 0x0000C0B8 File Offset: 0x0000A2B8
	public void FixedUpdate()
	{
		if (this.isDown && this.timer < 1f)
		{
			this.timer += Time.fixedDeltaTime / this.RefuelDuration;
			this.MyNormTask.Data[0] = (byte)Mathf.Min(255f, this.timer * 255f);
			if (this.timer >= 1f)
			{
				this.timer = 1f;
				this.MyNormTask.NextStep();
				base.StartCoroutine(base.CoStartClose(0.75f));
			}
		}
		this.destGauge.value = this.timer;
	}

	// Token: 0x0600005F RID: 95 RVA: 0x000024C5 File Offset: 0x000006C5
	public void StartStopFill()
	{
		this.isDown = !this.isDown;
	}

	// Token: 0x0400006B RID: 107
	public float RefuelDuration = 5f;

	// Token: 0x0400006C RID: 108
	public VerticalGauge destGauge;

	// Token: 0x0400006D RID: 109
	private bool isDown;

	// Token: 0x0400006E RID: 110
	private float timer;
}
