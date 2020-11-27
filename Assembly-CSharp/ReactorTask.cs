using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// Token: 0x02000218 RID: 536
public class ReactorTask : SabotageTask
{
	// Token: 0x170001C8 RID: 456
	// (get) Token: 0x06000BAF RID: 2991 RVA: 0x000090BA File Offset: 0x000072BA
	public override int TaskStep
	{
		get
		{
			return this.reactor.UserCount;
		}
	}

	// Token: 0x170001C9 RID: 457
	// (get) Token: 0x06000BB0 RID: 2992 RVA: 0x000090C7 File Offset: 0x000072C7
	public override bool IsComplete
	{
		get
		{
			return this.isComplete;
		}
	}

	// Token: 0x06000BB1 RID: 2993 RVA: 0x00039A80 File Offset: 0x00037C80
	public override void Initialize()
	{
		ShipStatus instance = ShipStatus.Instance;
		this.reactor = (ReactorSystemType)instance.Systems[SystemTypes.Reactor];
		((ReactorShipRoom)instance.AllRooms.First((ShipRoom r) => r.RoomId == SystemTypes.Reactor)).StartMeltdown();
		List<Vector2> list = base.FindObjectsPos();
		for (int i = 0; i < list.Count; i++)
		{
			this.Arrows[i].target = list[i];
			this.Arrows[i].gameObject.SetActive(true);
		}
	}

	// Token: 0x06000BB2 RID: 2994 RVA: 0x000090CF File Offset: 0x000072CF
	private void FixedUpdate()
	{
		if (this.isComplete)
		{
			return;
		}
		if (!this.reactor.IsActive)
		{
			this.Complete();
		}
	}

	// Token: 0x06000BB3 RID: 2995 RVA: 0x000090ED File Offset: 0x000072ED
	public override bool ValidConsole(global::Console console)
	{
		return console.TaskTypes.Contains(TaskTypes.ResetReactor);
	}

	// Token: 0x06000BB4 RID: 2996 RVA: 0x00002265 File Offset: 0x00000465
	public override void OnRemove()
	{
	}

	// Token: 0x06000BB5 RID: 2997 RVA: 0x00039B24 File Offset: 0x00037D24
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

	// Token: 0x06000BB6 RID: 2998 RVA: 0x00039B60 File Offset: 0x00037D60
	public override void AppendTaskText(StringBuilder sb)
	{
		this.even = !this.even;
		Color color = this.even ? Color.yellow : Color.red;
		sb.Append(color.ToTextColor() + "Reactor Meltdown in " + (int)this.reactor.Countdown);
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

	// Token: 0x04000B23 RID: 2851
	public ArrowBehaviour[] Arrows;

	// Token: 0x04000B24 RID: 2852
	private bool isComplete;

	// Token: 0x04000B25 RID: 2853
	private ReactorSystemType reactor;

	// Token: 0x04000B26 RID: 2854
	private bool even;
}
