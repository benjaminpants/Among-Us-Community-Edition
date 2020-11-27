using System;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x02000212 RID: 530
public class NoOxyTask : SabotageTask
{
	// Token: 0x170001BE RID: 446
	// (get) Token: 0x06000B78 RID: 2936 RVA: 0x00008E8B File Offset: 0x0000708B
	public override int TaskStep
	{
		get
		{
			return this.reactor.UserCount;
		}
	}

	// Token: 0x170001BF RID: 447
	// (get) Token: 0x06000B79 RID: 2937 RVA: 0x00008E98 File Offset: 0x00007098
	public override bool IsComplete
	{
		get
		{
			return this.isComplete;
		}
	}

	// Token: 0x06000B7A RID: 2938 RVA: 0x000390A0 File Offset: 0x000372A0
	public override void Initialize()
	{
		this.targetNumber = IntRange.Next(0, 99999);
		ShipStatus instance = ShipStatus.Instance;
		this.reactor = (LifeSuppSystemType)instance.Systems[SystemTypes.LifeSupp];
		global::Console[] array = (from c in base.FindConsoles()
		orderby c.ConsoleId
		select c).ToArray<global::Console>();
		for (int i = 0; i < array.Length; i++)
		{
			this.Arrows[i].target = array[i].transform.position;
			this.Arrows[i].gameObject.SetActive(true);
		}
		DestroyableSingleton<HudManager>.Instance.StartOxyFlash();
	}

	// Token: 0x06000B7B RID: 2939 RVA: 0x00039150 File Offset: 0x00037350
	private void FixedUpdate()
	{
		if (this.isComplete)
		{
			return;
		}
		if (!this.reactor.IsActive)
		{
			this.Complete();
			return;
		}
		for (int i = 0; i < this.Arrows.Length; i++)
		{
			this.Arrows[i].gameObject.SetActive(!this.reactor.GetConsoleComplete(i));
		}
	}

	// Token: 0x06000B7C RID: 2940 RVA: 0x00008EA0 File Offset: 0x000070A0
	public override bool ValidConsole(global::Console console)
	{
		return !this.reactor.GetConsoleComplete(console.ConsoleId) && console.TaskTypes.Contains(TaskTypes.RestoreOxy);
	}

	// Token: 0x06000B7D RID: 2941 RVA: 0x00002265 File Offset: 0x00000465
	public override void OnRemove()
	{
	}

	// Token: 0x06000B7E RID: 2942 RVA: 0x000391B0 File Offset: 0x000373B0
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

	// Token: 0x06000B7F RID: 2943 RVA: 0x000391EC File Offset: 0x000373EC
	public override void AppendTaskText(StringBuilder sb)
	{
		this.even = !this.even;
		Color color = this.even ? Color.yellow : Color.red;
		sb.Append(color.ToTextColor() + "Oxygen depleted in " + (int)this.reactor.Countdown);
		sb.AppendLine(string.Concat(new object[]
		{
			" (",
			this.reactor.UserCount,
			"/",
			2,
			")[]"
		}));
		for (int i = 0; i < this.Arrows.Length; i++)
		{
			this.Arrows[i].GetComponent<SpriteRenderer>().color = color;
		}
	}

	// Token: 0x04000B04 RID: 2820
	public ArrowBehaviour[] Arrows;

	// Token: 0x04000B05 RID: 2821
	private bool isComplete;

	// Token: 0x04000B06 RID: 2822
	private LifeSuppSystemType reactor;

	// Token: 0x04000B07 RID: 2823
	private bool even;

	// Token: 0x04000B08 RID: 2824
	public int targetNumber;
}
