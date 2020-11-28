using System;
using UnityEngine;

// Token: 0x020001C9 RID: 457
public class ShieldMinigame : Minigame
{
	// Token: 0x060009E9 RID: 2537 RVA: 0x000080D3 File Offset: 0x000062D3
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.shields = this.MyNormTask.Data[0];
		this.UpdateButtons();
	}

	// Token: 0x060009EA RID: 2538 RVA: 0x00034088 File Offset: 0x00032288
	public void ToggleShield(int i)
	{
		if (!this.MyNormTask.IsComplete)
		{
			byte b = (byte)(1 << i);
			this.shields ^= b;
			this.MyNormTask.Data[0] = this.shields;
			if ((this.shields & b) != 0)
			{
				if (Constants.ShouldPlaySfx())
				{
					SoundManager.Instance.PlaySound(this.ShieldOnSound, false, 1f);
				}
			}
			else if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.ShieldOffSound, false, 1f);
			}
			if (this.shields == 127)
			{
				this.MyNormTask.NextStep();
				base.StartCoroutine(base.CoStartClose(0.75f));
				if (!ShipStatus.Instance.ShieldsImages[0].IsPlaying((AnimationClip)null))
				{
					ShipStatus.Instance.StartShields();
					PlayerControl.LocalPlayer.RpcPlayAnimation(1);
				}
			}
		}
	}

	// Token: 0x060009EB RID: 2539 RVA: 0x000080F5 File Offset: 0x000062F5
	public void FixedUpdate()
	{
		this.UpdateButtons();
	}

	// Token: 0x060009EC RID: 2540 RVA: 0x00034168 File Offset: 0x00032368
	private void UpdateButtons()
	{
		int num = 0;
		for (int i = 0; i < this.Shields.Length; i++)
		{
			bool flag = ((int)this.shields & 1 << i) == 0;
			if (!flag)
			{
				num++;
			}
			this.Shields[i].color = (flag ? this.OffColor : this.OnColor);
		}
		if (this.shields == 127)
		{
			this.Gauge.transform.Rotate(0f, 0f, Time.fixedDeltaTime * 45f);
			this.Gauge.color = new Color(1f, 1f, 1f, 1f);
			return;
		}
		float num2 = Mathf.Lerp(0.1f, 0.5f, (float)num / 6f);
		this.Gauge.color = new Color(1f, num2, num2, 1f);
	}

	// Token: 0x0400098A RID: 2442
	public Color OnColor = Color.white;

	// Token: 0x0400098B RID: 2443
	public Color OffColor = Color.red;

	// Token: 0x0400098C RID: 2444
	public SpriteRenderer[] Shields;

	// Token: 0x0400098D RID: 2445
	public SpriteRenderer Gauge;

	// Token: 0x0400098E RID: 2446
	private byte shields;

	// Token: 0x0400098F RID: 2447
	public AudioClip ShieldOnSound;

	// Token: 0x04000990 RID: 2448
	public AudioClip ShieldOffSound;
}
