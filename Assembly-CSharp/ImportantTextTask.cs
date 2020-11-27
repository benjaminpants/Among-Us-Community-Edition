using System;
using System.Text;

// Token: 0x02000211 RID: 529
public class ImportantTextTask : PlayerTask
{
	// Token: 0x170001BC RID: 444
	// (get) Token: 0x06000B71 RID: 2929 RVA: 0x00002723 File Offset: 0x00000923
	public override int TaskStep
	{
		get
		{
			return 0;
		}
	}

	// Token: 0x170001BD RID: 445
	// (get) Token: 0x06000B72 RID: 2930 RVA: 0x00002723 File Offset: 0x00000923
	public override bool IsComplete
	{
		get
		{
			return false;
		}
	}

	// Token: 0x06000B73 RID: 2931 RVA: 0x00002265 File Offset: 0x00000465
	public override void Initialize()
	{
	}

	// Token: 0x06000B74 RID: 2932 RVA: 0x00002723 File Offset: 0x00000923
	public override bool ValidConsole(global::Console console)
	{
		return false;
	}

	// Token: 0x06000B75 RID: 2933 RVA: 0x00002265 File Offset: 0x00000465
	public override void Complete()
	{
	}

	// Token: 0x06000B76 RID: 2934 RVA: 0x00008E65 File Offset: 0x00007065
	public override void AppendTaskText(StringBuilder sb)
	{
		sb.AppendLine("[FF0000FF]" + this.Text + "[]");
	}

	// Token: 0x04000B03 RID: 2819
	public string Text;
}
