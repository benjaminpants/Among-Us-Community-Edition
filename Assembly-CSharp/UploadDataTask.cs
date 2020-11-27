using System;
using System.Linq;
using System.Text;

// Token: 0x0200021E RID: 542
public class UploadDataTask : NormalPlayerTask
{
	// Token: 0x06000BBF RID: 3007 RVA: 0x00039CDC File Offset: 0x00037EDC
	public override bool ValidConsole(global::Console console)
	{
		return (console.Room == this.StartAt && console.ValidTasks.Any((TaskSet set) => this.TaskType == set.taskType && set.taskStep.Contains(this.taskStep))) || (this.taskStep == 1 && console.TaskTypes.Contains(this.TaskType));
	}

	// Token: 0x06000BC0 RID: 3008 RVA: 0x00039D30 File Offset: 0x00037F30
	public override void AppendTaskText(StringBuilder sb)
	{
		if (this.taskStep > 0)
		{
			if (this.IsComplete)
			{
				sb.Append("[00DD00FF]");
			}
			else
			{
				sb.Append("[FFFF00FF]");
			}
		}
		sb.Append(SystemTypeHelpers.StringNames[(int)((this.taskStep == 0) ? this.StartAt : SystemTypes.Admin)]);
		sb.Append(": ");
		sb.Append((this.taskStep == 0) ? "Download" : "Upload");
		sb.Append(" Data (");
		sb.Append(this.taskStep);
		sb.Append("/");
		sb.Append(this.MaxStep);
		sb.AppendLine(") []");
	}
}
