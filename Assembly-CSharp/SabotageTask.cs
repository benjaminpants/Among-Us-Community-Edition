using System;

// Token: 0x0200021A RID: 538
public abstract class SabotageTask : PlayerTask
{
	// Token: 0x06000BBB RID: 3003 RVA: 0x00009108 File Offset: 0x00007308
	public void MarkContributed()
	{
		this.didContribute = true;
	}

	// Token: 0x04000B29 RID: 2857
	protected bool didContribute;
}
