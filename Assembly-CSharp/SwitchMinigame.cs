using System;
using UnityEngine;

// Token: 0x02000081 RID: 129
public class SwitchMinigame : Minigame
{
	// Token: 0x060002B8 RID: 696 RVA: 0x00015330 File Offset: 0x00013530
	public override void Begin(PlayerTask task)
	{
		this.ship = UnityEngine.Object.FindObjectOfType<ShipStatus>();
		SwitchSystem switchSystem = this.ship.Systems[SystemTypes.Electrical] as SwitchSystem;
		for (int i = 0; i < this.switches.Length; i++)
		{
			byte b = (byte)(1 << i);
			int num = (int)(switchSystem.ActualSwitches & b);
			this.lights[i].color = ((num == (int)(switchSystem.ExpectedSwitches & b)) ? this.OnColor : this.OffColor);
			this.switches[i].flipY = (num >> i == 0);
		}
	}

	// Token: 0x060002B9 RID: 697 RVA: 0x000153C0 File Offset: 0x000135C0
	public void FixedUpdate()
	{
		if (this.amClosing != Minigame.CloseState.None)
		{
			return;
		}
		int num = 0;
		SwitchSystem switchSystem = this.ship.Systems[SystemTypes.Electrical] as SwitchSystem;
		for (int i = 0; i < this.switches.Length; i++)
		{
			byte b = (byte)(1 << i);
			int num2 = (int)(switchSystem.ActualSwitches & b);
			if (num2 == (int)(switchSystem.ExpectedSwitches & b))
			{
				num++;
				this.lights[i].color = this.OnColor;
			}
			else
			{
				this.lights[i].color = this.OffColor;
			}
			this.switches[i].flipY = (num2 >> i == 0);
		}
		float num3 = (float)num / (float)this.switches.Length;
		this.bottom.Center = 0.47f * num3;
		this.top.NoiseLevel = 1f - num3;
		this.middle.Value = switchSystem.Level + (Mathf.PerlinNoise(0f, Time.time * 51f) - 0.5f) * 0.04f;
		if (num == this.switches.Length)
		{
			base.StartCoroutine(base.CoStartClose(0.5f));
		}
	}

	// Token: 0x060002BA RID: 698 RVA: 0x000154E8 File Offset: 0x000136E8
	public void FlipSwitch(int switchIdx)
	{
		if (this.amClosing != Minigame.CloseState.None)
		{
			return;
		}
		int num = 0;
		SwitchSystem switchSystem = this.ship.Systems[SystemTypes.Electrical] as SwitchSystem;
		for (int i = 0; i < this.switches.Length; i++)
		{
			byte b = (byte)(1 << i);
			if ((switchSystem.ActualSwitches & b) == (switchSystem.ExpectedSwitches & b))
			{
				num++;
			}
		}
		if (num == this.switches.Length)
		{
			return;
		}
		ShipStatus.Instance.RpcRepairSystem(SystemTypes.Electrical, (int)((byte)switchIdx));
		try
		{
			((SabotageTask)this.MyTask).MarkContributed();
		}
		catch
		{
		}
	}

	// Token: 0x040002AD RID: 685
	public Color OnColor = Color.green;

	// Token: 0x040002AE RID: 686
	public Color OffColor = new Color(0.1f, 0.3f, 0.1f);

	// Token: 0x040002AF RID: 687
	private ShipStatus ship;

	// Token: 0x040002B0 RID: 688
	public SpriteRenderer[] switches;

	// Token: 0x040002B1 RID: 689
	public SpriteRenderer[] lights;

	// Token: 0x040002B2 RID: 690
	public RadioWaveBehaviour top;

	// Token: 0x040002B3 RID: 691
	public HorizontalGauge middle;

	// Token: 0x040002B4 RID: 692
	public FlatWaveBehaviour bottom;
}
