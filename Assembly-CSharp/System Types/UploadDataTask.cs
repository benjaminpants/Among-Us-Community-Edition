using System.Linq;
using System.Text;

public class UploadDataTask : NormalPlayerTask
{
	public override bool ValidConsole(Console console)
	{
		if (console.Room != StartAt || !console.ValidTasks.Any((TaskSet set) => TaskType == set.taskType && set.taskStep.Contains(taskStep)))
		{
			if (taskStep == 1)
			{
				return console.TaskTypes.Contains(TaskType);
			}
			return false;
		}
		return true;
	}

	public override void AppendTaskText(StringBuilder sb)
	{
		if (taskStep > 0)
		{
			if (IsComplete)
			{
				sb.Append("[00DD00FF]");
			}
			else
			{
				sb.Append("[FFFF00FF]");
			}
		}
		sb.Append(SystemTypeHelpers.GetName(((taskStep == 0) ? StartAt : SystemTypes.Admin)));
		sb.Append(": ");
		sb.Append((taskStep == 0) ? "Download" : "Upload");
		sb.Append(" Data (");
		sb.Append(taskStep);
		sb.Append("/");
		sb.Append(MaxStep);
		sb.AppendLine(") []");
	}
}
