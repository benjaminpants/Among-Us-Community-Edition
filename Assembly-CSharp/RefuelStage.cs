using System;
using UnityEngine;

// Token: 0x0200008C RID: 140
public class RefuelStage : MonoBehaviour
{
	// Token: 0x1700007E RID: 126
	// (get) Token: 0x060002EE RID: 750 RVA: 0x00003EED File Offset: 0x000020ED
	// (set) Token: 0x060002EF RID: 751 RVA: 0x00003EF5 File Offset: 0x000020F5
	public NormalPlayerTask MyNormTask { get; set; }

	// Token: 0x060002F0 RID: 752 RVA: 0x00003EFE File Offset: 0x000020FE
	public void Begin()
	{
		this.timer = (float)this.MyNormTask.Data[0] / 255f;
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x000162F0 File Offset: 0x000144F0
	public void FixedUpdate()
	{
		if (this.complete)
		{
			return;
		}
		if (this.isDown && this.timer < 1f)
		{
			this.timer += Time.fixedDeltaTime / this.RefuelDuration;
			this.MyNormTask.Data[0] = (byte)Mathf.Min(255f, this.timer * 255f);
			if (this.timer >= 1f)
			{
				this.complete = true;
				this.greenLight.color = this.green;
				this.redLight.color = this.darkRed;
				this.MyNormTask.Data[0] = 0;
				byte[] data = this.MyNormTask.Data;
				int num = 1;
				data[num] += 1;
				if (this.MyNormTask.Data[1] % 2 == 0)
				{
					this.MyNormTask.NextStep();
				}
				this.MyNormTask.UpdateArrow();
			}
		}
		this.destGauge.value = this.timer;
		if (this.srcGauge)
		{
			this.srcGauge.value = 1f - this.timer;
		}
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00016418 File Offset: 0x00014618
	public void Refuel()
	{
		if (this.complete)
		{
			base.transform.parent.GetComponent<Minigame>().Close();
			return;
		}
		this.isDown = !this.isDown;
		this.redLight.color = (this.isDown ? this.red : this.darkRed);
		if (this.isDown)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlayDynamicSound("Refuel", this.RefuelSound, true, new DynamicSound.GetDynamicsFunction(this.GetRefuelDynamics), true);
				return;
			}
		}
		else
		{
			SoundManager.Instance.StopSound(this.RefuelSound);
		}
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x00003F1A File Offset: 0x0000211A
	private void GetRefuelDynamics(AudioSource player, float dt)
	{
		player.volume = 1f;
		player.pitch = Mathf.Lerp(0.75f, 1.25f, this.timer);
	}

	// Token: 0x040002EA RID: 746
	public float RefuelDuration = 5f;

	// Token: 0x040002EB RID: 747
	private Color darkRed = new Color32(90, 0, 0, byte.MaxValue);

	// Token: 0x040002EC RID: 748
	private Color red = new Color32(byte.MaxValue, 58, 0, byte.MaxValue);

	// Token: 0x040002ED RID: 749
	private Color green = Color.green;

	// Token: 0x040002EE RID: 750
	public SpriteRenderer redLight;

	// Token: 0x040002EF RID: 751
	public SpriteRenderer greenLight;

	// Token: 0x040002F0 RID: 752
	public VerticalGauge srcGauge;

	// Token: 0x040002F1 RID: 753
	public VerticalGauge destGauge;

	// Token: 0x040002F2 RID: 754
	public AudioClip RefuelSound;

	// Token: 0x040002F3 RID: 755
	private float timer;

	// Token: 0x040002F4 RID: 756
	private bool isDown;

	// Token: 0x040002F5 RID: 757
	private bool complete;
}
