using System;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x0200020F RID: 527
public class ElectricTask : SabotageTask
{
	// Token: 0x170001B8 RID: 440
	// (get) Token: 0x06000B61 RID: 2913 RVA: 0x00008DCE File Offset: 0x00006FCE
	public override int TaskStep
	{
		get
		{
			if (!this.isComplete)
			{
				return 0;
			}
			return 1;
		}
	}

	// Token: 0x170001B9 RID: 441
	// (get) Token: 0x06000B62 RID: 2914 RVA: 0x00008DDB File Offset: 0x00006FDB
	public override bool IsComplete
	{
		get
		{
			return this.isComplete;
		}
	}

	// Token: 0x06000B63 RID: 2915 RVA: 0x00038E68 File Offset: 0x00037068
	public override void Initialize()
	{
		ShipStatus instance = ShipStatus.Instance;
		this.system = (SwitchSystem)instance.Systems[SystemTypes.Electrical];
		this.Arrow.target = base.FindObjectPos().transform.position;
		this.Arrow.gameObject.SetActive(true);
	}

	// Token: 0x06000B64 RID: 2916 RVA: 0x00008DE3 File Offset: 0x00006FE3
	private void FixedUpdate()
	{
		if (this.isComplete)
		{
			return;
		}
		if (this.system.ExpectedSwitches == this.system.ActualSwitches)
		{
			this.Complete();
		}
	}

	// Token: 0x06000B65 RID: 2917 RVA: 0x00008E0C File Offset: 0x0000700C
	public override bool ValidConsole(global::Console console)
	{
		return console.TaskTypes.Contains(TaskTypes.FixLights);
	}

	// Token: 0x06000B66 RID: 2918 RVA: 0x00038EC0 File Offset: 0x000370C0
	public override void Complete()
	{
		this.isComplete = true;
		PlayerControl.LocalPlayer.RemoveTask(this);
		if (this.didContribute)
		{
			StatsManager instance = StatsManager.Instance;
			uint sabsFixed = instance.SabsFixed;
			instance.SabsFixed = sabsFixed + 1U;
		}
	}

	// Token: 0x06000B67 RID: 2919 RVA: 0x00038EFC File Offset: 0x000370FC
	public override void AppendTaskText(StringBuilder sb)
	{
		this.even = !this.even;
		Color color = this.even ? Color.yellow : Color.red;
		sb.Append(color.ToTextColor() + "Fix lights ");
		sb.AppendLine(" (%" + (int)(this.system.Level * 100f) + ")[]");
		this.Arrow.GetComponent<SpriteRenderer>().color = color;
	}

	// Token: 0x04000AFB RID: 2811
	public ArrowBehaviour Arrow;

	// Token: 0x04000AFC RID: 2812
	private bool isComplete;

	// Token: 0x04000AFD RID: 2813
	private SwitchSystem system;

	// Token: 0x04000AFE RID: 2814
	private bool even;
}
