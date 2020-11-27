using System;
using System.Linq;
using System.Text;

// Token: 0x0200020E RID: 526
public class DivertPowerTask : NormalPlayerTask
{
	// Token: 0x06000B5D RID: 2909 RVA: 0x00038D30 File Offset: 0x00036F30
	public override bool ValidConsole(global::Console console)
	{
		return (console.Room == this.TargetSystem && console.ValidTasks.Any((TaskSet set) => this.TaskType == set.taskType && set.taskStep.Contains(this.taskStep))) || (this.taskStep == 0 && console.TaskTypes.Contains(this.TaskType));
	}

	// Token: 0x06000B5E RID: 2910 RVA: 0x00038D84 File Offset: 0x00036F84
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
		if (this.taskStep == 0)
		{
			sb.Append(this.StartAt);
			sb.Append(": Divert Power to ");
			sb.Append(this.TargetSystem);
		}
		else
		{
			sb.Append(this.TargetSystem);
			sb.Append(": Accept diverted power");
		}
		sb.Append(" (");
		sb.Append(this.taskStep);
		sb.Append("/");
		sb.Append(this.MaxStep);
		sb.AppendLine(")");
		if (this.taskStep > 0)
		{
			sb.Append("[]");
		}
	}

	// Token: 0x04000AFA RID: 2810
	public SystemTypes TargetSystem;
}
