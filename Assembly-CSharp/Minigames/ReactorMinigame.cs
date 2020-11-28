using System;
using UnityEngine;

// Token: 0x020001A2 RID: 418
public class ReactorMinigame : Minigame
{
	// Token: 0x060008DA RID: 2266 RVA: 0x0002FB40 File Offset: 0x0002DD40
	public override void Begin(PlayerTask task)
	{
		ShipStatus instance = ShipStatus.Instance;
		if (!instance)
		{
			this.reactor = new ReactorSystemType();
		}
		else
		{
			this.reactor = (instance.Systems[SystemTypes.Reactor] as ReactorSystemType);
		}
		this.hand.color = this.bad;
	}

	// Token: 0x060008DB RID: 2267 RVA: 0x0002FB90 File Offset: 0x0002DD90
	public void ButtonDown()
	{
		if (PlayerControl.LocalPlayer.Data.IsImpostor)
		{
			return;
		}
		if (!this.reactor.IsActive)
		{
			return;
		}
		this.isButtonDown = !this.isButtonDown;
		if (this.isButtonDown)
		{
			if (Constants.ShouldPlaySfx())
			{
				SoundManager.Instance.PlaySound(this.HandSound, true, 1f);
			}
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, (int)((byte)(64 | base.ConsoleId)));
		}
		else
		{
			SoundManager.Instance.StopSound(this.HandSound);
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, (int)((byte)(32 | base.ConsoleId)));
		}
		try
		{
			((SabotageTask)this.MyTask).MarkContributed();
		}
		catch
		{
		}
	}

	// Token: 0x060008DC RID: 2268 RVA: 0x0002FC54 File Offset: 0x0002DE54
	public void FixedUpdate()
	{
		if (!this.reactor.IsActive)
		{
			if (this.amClosing == Minigame.CloseState.None)
			{
				this.hand.color = this.good;
				this.statusText.Text = "Reactor Nominal";
				this.sweeper.enabled = false;
				SoundManager.Instance.StopSound(this.HandSound);
				base.StartCoroutine(base.CoStartClose(0.75f));
				return;
			}
		}
		else
		{
			if (!this.isButtonDown)
			{
				this.statusText.Text = "Hold to stop meltdown";
				this.sweeper.enabled = false;
				return;
			}
			this.statusText.Text = "Waiting for second user";
			Vector3 localPosition = this.sweeper.transform.localPosition;
			localPosition.y = this.YSweep.Lerp(Mathf.Sin(Time.time) * 0.5f + 0.5f);
			this.sweeper.transform.localPosition = localPosition;
			this.sweeper.enabled = true;
		}
	}

	// Token: 0x060008DD RID: 2269 RVA: 0x00007695 File Offset: 0x00005895
	public override void Close()
	{
		SoundManager.Instance.StopSound(this.HandSound);
		if (ShipStatus.Instance)
		{
			ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, (int)((byte)(32 | base.ConsoleId)));
		}
		base.Close();
	}

	// Token: 0x04000893 RID: 2195
	private Color bad = new Color(1f, 0.160784319f, 0f);

	// Token: 0x04000894 RID: 2196
	private Color good = new Color(0.3019608f, 0.8862745f, 0.8352941f);

	// Token: 0x04000895 RID: 2197
	private ReactorSystemType reactor;

	// Token: 0x04000896 RID: 2198
	public TextRenderer statusText;

	// Token: 0x04000897 RID: 2199
	public SpriteRenderer hand;

	// Token: 0x04000898 RID: 2200
	private FloatRange YSweep = new FloatRange(-2.15f, 1.56f);

	// Token: 0x04000899 RID: 2201
	public SpriteRenderer sweeper;

	// Token: 0x0400089A RID: 2202
	public AudioClip HandSound;

	// Token: 0x0400089B RID: 2203
	private bool isButtonDown;
}
