using System;

// Token: 0x0200008B RID: 139
public class RefuelMinigame : Minigame
{
	// Token: 0x060002EB RID: 747 RVA: 0x00016294 File Offset: 0x00014494
	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		this.stage = this.Stages[(int)this.MyNormTask.Data[1]];
		this.stage.MyNormTask = this.MyNormTask;
		this.stage.gameObject.SetActive(true);
		this.stage.Begin();
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00003ED0 File Offset: 0x000020D0
	public override void Close()
	{
		SoundManager.Instance.StopSound(this.stage.RefuelSound);
		base.Close();
	}

	// Token: 0x040002E7 RID: 743
	public RefuelStage[] Stages;

	// Token: 0x040002E8 RID: 744
	private RefuelStage stage;
}
