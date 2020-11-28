using System;
using UnityEngine;

// Token: 0x0200007F RID: 127
public class SweepMinigame : Minigame
{
	// Token: 0x060002AF RID: 687 RVA: 0x00003BB6 File Offset: 0x00001DB6
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.ResetGauges();
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.SpinningSound, true, 1f);
		}
	}

	// Token: 0x060002B0 RID: 688 RVA: 0x00014E08 File Offset: 0x00013008
	public void FixedUpdate()
	{
		float num = Mathf.Clamp(2f - this.timer / 30f, 1f, 2f);
		this.timer += Time.fixedDeltaTime * num;
		if (this.spinnerIdx < this.Spinners.Length)
		{
			float num2 = this.CalcXPerc();
			this.Gauges[this.spinnerIdx].Value = ((num2 < 13f) ? 0.9f : 0.1f);
			Quaternion localRotation = Quaternion.Euler(0f, 0f, this.timer * this.SpinRate);
			this.Spinners[this.spinnerIdx].transform.localRotation = localRotation;
			this.Shadows[this.spinnerIdx].transform.localRotation = localRotation;
			this.Lights[this.spinnerIdx].enabled = (num2 < 13f);
		}
		for (int i = 0; i < this.Gauges.Length; i++)
		{
			HorizontalGauge horizontalGauge = this.Gauges[i];
			if (i < this.spinnerIdx)
			{
				horizontalGauge.Value = 0.95f;
			}
			if (i > this.spinnerIdx)
			{
				horizontalGauge.Value = 0.05f;
			}
			horizontalGauge.Value += (Mathf.PerlinNoise((float)i, Time.time * 51f) - 0.5f) * 0.025f;
		}
	}

	// Token: 0x060002B1 RID: 689 RVA: 0x00014F68 File Offset: 0x00013168
	private float CalcXPerc()
	{
		int num = (int)(this.timer * this.SpinRate) % 360;
		return (float)Mathf.Min(360 - num, num);
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00014F98 File Offset: 0x00013198
	public void HitButton(int i)
	{
		if (i != this.spinnerIdx)
		{
			return;
		}
		if (this.CalcXPerc() < 13f)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.AcceptSound, false, 1f);
			}
			this.Spinners[this.spinnerIdx].transform.localRotation = Quaternion.identity;
			this.Shadows[this.spinnerIdx].transform.localRotation = Quaternion.identity;
			this.spinnerIdx++;
			this.timer = this.initialTimer;
			if (this.spinnerIdx >= this.Gauges.Length)
			{
				SoundManager.Instance.StopSound(this.SpinningSound);
				this.MyNormTask.NextStep();
				base.StartCoroutine(base.CoStartClose(0.75f));
				return;
			}
		}
		else
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.RejectSound, false, 1f);
			}
			this.ResetGauges();
		}
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00015094 File Offset: 0x00013294
	private void ResetGauges()
	{
		this.spinnerIdx = 0;
		this.timer = FloatRange.Next(1f, 3f);
		this.initialTimer = this.timer;
		for (int i = 0; i < this.Gauges.Length; i++)
		{
			this.Lights[i].enabled = false;
			this.Spinners[i].transform.localRotation = Quaternion.Euler(0f, 0f, this.timer * this.SpinRate);
			this.Shadows[i].transform.localRotation = Quaternion.Euler(0f, 0f, this.timer * this.SpinRate);
		}
	}

	// Token: 0x04000298 RID: 664
	public SpriteRenderer[] Spinners;

	// Token: 0x04000299 RID: 665
	public SpriteRenderer[] Shadows;

	// Token: 0x0400029A RID: 666
	public SpriteRenderer[] Lights;

	// Token: 0x0400029B RID: 667
	public HorizontalGauge[] Gauges;

	// Token: 0x0400029C RID: 668
	private int spinnerIdx;

	// Token: 0x0400029D RID: 669
	private float timer;

	// Token: 0x0400029E RID: 670
	public float SpinRate = 45f;

	// Token: 0x0400029F RID: 671
	private float initialTimer;

	// Token: 0x040002A0 RID: 672
	public AudioClip SpinningSound;

	// Token: 0x040002A1 RID: 673
	public AudioClip AcceptSound;

	// Token: 0x040002A2 RID: 674
	public AudioClip RejectSound;
}
