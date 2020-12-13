public class SubStringReader
{
	private readonly string Source;

	private int Position;

	public SubStringReader(string source)
	{
		Source = source;
	}

	public SubString ReadLine()
	{
		int position = Position;
		if (position >= Source.Length)
		{
			return default(SubString);
		}
		int num = Position;
		for (int i = position; i < Source.Length; i++)
		{
			switch (Source[i])
			{
			case '\r':
				num = i - 1;
				Position = i + 1;
				if (i + 1 < Source.Length && Source[i + 1] == '\n')
				{
					Position = i + 2;
				}
				break;
			case '\n':
				num = i - 1;
				Position = i + 1;
				break;
			default:
				continue;
			}
			break;
		}
		return new SubString(Source, position, num - position);
	}
}
