using System.Text;

public class ImportantTextTask : PlayerTask
{
	public string Text;

	public override int TaskStep => 0;

	public override bool IsComplete => false;

	public override void Initialize()
	{
	}

	public override bool ValidConsole(Console console)
	{
		return false;
	}

	public override void Complete()
	{
	}

	public override void AppendTaskText(StringBuilder sb)
	{
		sb.AppendLine("[FF0000FF]" + Text + "[]");
	}
}
