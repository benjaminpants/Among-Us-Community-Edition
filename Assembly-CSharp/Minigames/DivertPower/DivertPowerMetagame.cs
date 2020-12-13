using UnityEngine;

public class DivertPowerMetagame : Minigame
{
	public Minigame DistributePrefab;

	public Minigame ReceivePrefab;

	public override void Begin(PlayerTask task)
	{
		base.Begin(task);
		Minigame minigame = ((MyNormTask.taskStep != 0) ? Object.Instantiate(ReceivePrefab, base.transform.parent) : Object.Instantiate(DistributePrefab, base.transform.parent));
		minigame.Begin(task);
		Object.Destroy(base.gameObject);
	}
}
