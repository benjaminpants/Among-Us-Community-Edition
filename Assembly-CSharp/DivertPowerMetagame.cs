using System;
using UnityEngine;

// Token: 0x0200007C RID: 124
public class DivertPowerMetagame : Minigame
{
	// Token: 0x060002A8 RID: 680 RVA: 0x00014A9C File Offset: 0x00012C9C
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		Minigame minigame;
		if (this.MyNormTask.taskStep == 0)
		{
			minigame = UnityEngine.Object.Instantiate<Minigame>(this.DistributePrefab, base.transform.parent);
		}
		else
		{
			minigame = UnityEngine.Object.Instantiate<Minigame>(this.ReceivePrefab, base.transform.parent);
		}
		minigame.Begin(task);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	// Token: 0x0400028E RID: 654
	public Minigame DistributePrefab;

	// Token: 0x0400028F RID: 655
	public Minigame ReceivePrefab;
}
