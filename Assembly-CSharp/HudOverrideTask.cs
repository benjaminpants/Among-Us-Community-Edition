using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x02000210 RID: 528
public class HudOverrideTask : SabotageTask
{
	// Token: 0x170001BA RID: 442
	// (get) Token: 0x06000B69 RID: 2921 RVA: 0x00008E23 File Offset: 0x00007023
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

	// Token: 0x170001BB RID: 443
	// (get) Token: 0x06000B6A RID: 2922 RVA: 0x00008E30 File Offset: 0x00007030
	public override bool IsComplete
	{
		get
		{
			return this.isComplete;
		}
	}

	// Token: 0x06000B6B RID: 2923 RVA: 0x00038F84 File Offset: 0x00037184
	public override void Initialize()
	{
		ShipStatus instance = ShipStatus.Instance;
		this.system = (instance.Systems[SystemTypes.Comms] as HudOverrideSystemType);
		List<Vector2> list = base.FindObjectsPos();
		for (int i = 0; i < list.Count; i++)
		{
			this.Arrows[i].target = list[i];
			this.Arrows[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x06000B6C RID: 2924 RVA: 0x00008E38 File Offset: 0x00007038
	private void FixedUpdate()
	{
		if (this.isComplete)
		{
			return;
		}
		if (!this.system.IsActive)
		{
			this.Complete();
		}
	}

	// Token: 0x06000B6D RID: 2925 RVA: 0x00008E56 File Offset: 0x00007056
	public override bool ValidConsole(global::Console console)
	{
		return console.TaskTypes.Contains(TaskTypes.FixComms);
	}

	// Token: 0x06000B6E RID: 2926 RVA: 0x00038FF4 File Offset: 0x000371F4
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

	// Token: 0x06000B6F RID: 2927 RVA: 0x00039030 File Offset: 0x00037230
	public override void AppendTaskText(StringBuilder sb)
	{
		this.even = !this.even;
		Color color = this.even ? Color.yellow : Color.red;
		sb.Append(color.ToTextColor() + "Hud Sabotaged[]");
		for (int i = 0; i < this.Arrows.Length; i++)
		{
			this.Arrows[i].GetComponent<SpriteRenderer>().color = color;
		}
	}

	// Token: 0x04000AFF RID: 2815
	public ArrowBehaviour[] Arrows;

	// Token: 0x04000B00 RID: 2816
	private bool isComplete;

	// Token: 0x04000B01 RID: 2817
	private HudOverrideSystemType system;

	// Token: 0x04000B02 RID: 2818
	private bool even;
}
