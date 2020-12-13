public class RefuelMinigame : Minigame
{
	public RefuelStage[] Stages;

	private RefuelStage stage;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		stage = Stages[MyNormTask.Data[1]];
		stage.MyNormTask = MyNormTask;
		stage.gameObject.SetActive(value: true);
		stage.Begin();
	}

	public override void Close()
	{
		SoundManager.Instance.StopSound(stage.RefuelSound);
		base.Close();
	}
}
