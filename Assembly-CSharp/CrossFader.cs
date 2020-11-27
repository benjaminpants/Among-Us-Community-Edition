using System;
using UnityEngine;

// Token: 0x020001FA RID: 506
public class CrossFader : ISoundPlayer
{
	// Token: 0x1700019E RID: 414
	// (get) Token: 0x06000AE1 RID: 2785 RVA: 0x0000881C File Offset: 0x00006A1C
	// (set) Token: 0x06000AE2 RID: 2786 RVA: 0x00008824 File Offset: 0x00006A24
	public string Name { get; set; }

	// Token: 0x1700019F RID: 415
	// (get) Token: 0x06000AE3 RID: 2787 RVA: 0x0000882D File Offset: 0x00006A2D
	// (set) Token: 0x06000AE4 RID: 2788 RVA: 0x00008835 File Offset: 0x00006A35
	public AudioSource Player { get; set; }

	// Token: 0x06000AE5 RID: 2789 RVA: 0x0003719C File Offset: 0x0003539C
	public void Update(float dt)
	{
		if (this.timer < this.Duration)
		{
			this.timer += dt;
			float num = this.timer / this.Duration;
			if (num < 0.5f)
			{
				this.Player.volume = (1f - num * 2f) * this.MaxVolume;
				return;
			}
			if (!this.didSwitch)
			{
				this.didSwitch = true;
				this.Player.Stop();
				this.Player.clip = this.target;
				if (this.target)
				{
					this.Player.Play();
				}
			}
			this.Player.volume = (num - 0.5f) * 2f * this.MaxVolume;
		}
	}

	// Token: 0x06000AE6 RID: 2790 RVA: 0x00037260 File Offset: 0x00035460
	public void SetTarget(AudioClip clip)
	{
		if (!this.Player.clip)
		{
			this.didSwitch = false;
			this.Player.volume = 0f;
			this.timer = 0.5f;
		}
		else
		{
			if (this.Player.clip == clip)
			{
				return;
			}
			if (this.didSwitch)
			{
				this.didSwitch = false;
				if (this.timer >= this.Duration)
				{
					this.timer = 0f;
				}
				else
				{
					this.timer = this.Duration - this.timer;
				}
			}
		}
		this.target = clip;
	}

	// Token: 0x04000A94 RID: 2708
	public float MaxVolume = 1f;

	// Token: 0x04000A96 RID: 2710
	public AudioClip target;

	// Token: 0x04000A97 RID: 2711
	public float Duration = 1.5f;

	// Token: 0x04000A98 RID: 2712
	private float timer;

	// Token: 0x04000A99 RID: 2713
	private bool didSwitch;
}
