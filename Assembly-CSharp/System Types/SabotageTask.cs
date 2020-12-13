public abstract class SabotageTask : PlayerTask
{
	protected bool didContribute;

	public void MarkContributed()
	{
		didContribute = true;
	}
}
