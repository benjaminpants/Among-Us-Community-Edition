using System;
using UnityEngine;

// Token: 0x02000019 RID: 25
public class TuneRadioMinigame : Minigame
{
	// Token: 0x0600006C RID: 108 RVA: 0x0000CAC4 File Offset: 0x0000ACC4
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.targetAngle = this.dial.DialRange.Next();
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlayDynamicSound("CommsRadio", this.RadioSound, true, new DynamicSound.GetDynamicsFunction(this.GetRadioVolume), true);
			SoundManager.Instance.PlayDynamicSound("RadioStatic", this.StaticSound, true, new DynamicSound.GetDynamicsFunction(this.GetStaticVolume), true);
		}
	}

	// Token: 0x0600006D RID: 109 RVA: 0x00002550 File Offset: 0x00000750
	private void GetRadioVolume(AudioSource player, float dt)
	{
		player.volume = 1f - this.actualSignal.NoiseLevel;
	}

	// Token: 0x0600006E RID: 110 RVA: 0x00002569 File Offset: 0x00000769
	private void GetStaticVolume(AudioSource player, float dt)
	{
		player.volume = this.actualSignal.NoiseLevel;
	}

	// Token: 0x0600006F RID: 111 RVA: 0x0000CB3C File Offset: 0x0000AD3C
	public void Update()
	{
		if (this.finished)
		{
			return;
		}
		float f = Mathf.Abs((this.targetAngle - this.dial.Value) / this.dial.DialRange.Width) * 2f;
		this.actualSignal.NoiseLevel = Mathf.Clamp(Mathf.Sqrt(f), 0f, 1f);
		if (this.actualSignal.NoiseLevel <= this.Tolerance)
		{
			this.redLight.color = new Color(0.35f, 0f, 0f);
			if (!this.dial.Engaged)
			{
				this.FinishGame();
				return;
			}
			this.steadyTimer += Time.deltaTime;
			if (this.steadyTimer > 1.5f)
			{
				this.FinishGame();
				return;
			}
		}
		else
		{
			this.redLight.color = new Color(1f, 0f, 0f);
			this.steadyTimer = 0f;
		}
	}

	// Token: 0x06000070 RID: 112 RVA: 0x0000CC38 File Offset: 0x0000AE38
	private void FinishGame()
	{
		this.greenLight.color = Color.green;
		this.finished = true;
		this.dial.enabled = false;
		this.dial.SetValue(this.targetAngle);
		this.actualSignal.NoiseLevel = 0f;
		if (PlayerControl.LocalPlayer)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
		}
		base.StartCoroutine(base.CoStartClose(0.75f));
		try
		{
			((SabotageTask)this.MyTask).MarkContributed();
		}
		catch
		{
		}
	}

	// Token: 0x06000071 RID: 113 RVA: 0x0000257C File Offset: 0x0000077C
	public override void Close()
	{
		SoundManager.Instance.StopSound(this.StaticSound);
		SoundManager.Instance.StopSound(this.RadioSound);
		base.Close();
	}

	// Token: 0x04000094 RID: 148
	public RadioWaveBehaviour actualSignal;

	// Token: 0x04000095 RID: 149
	public DialBehaviour dial;

	// Token: 0x04000096 RID: 150
	public SpriteRenderer redLight;

	// Token: 0x04000097 RID: 151
	public SpriteRenderer greenLight;

	// Token: 0x04000098 RID: 152
	public float Tolerance = 0.1f;

	// Token: 0x04000099 RID: 153
	public float targetAngle;

	// Token: 0x0400009A RID: 154
	public bool finished;

	// Token: 0x0400009B RID: 155
	private float steadyTimer;

	// Token: 0x0400009C RID: 156
	public AudioClip StaticSound;

	// Token: 0x0400009D RID: 157
	public AudioClip RadioSound;
}
