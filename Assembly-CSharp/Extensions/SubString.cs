using System;

public struct SubString
{
	public readonly int Start;

	public readonly int Length;

	public readonly string Source;

	public SubString(string source, int start, int length)
	{
		Source = source;
		Start = start;
		Length = length;
	}

	public override string ToString()
	{
		return Source.Substring(Start, Length);
	}

	public int GetKvpValue()
	{
		int num = Start + Length;
		for (int i = Start; i < num; i++)
		{
			if (Source[i] == '=')
			{
				i++;
				return new SubString(Source, i, num - i).ToInt();
			}
		}
		throw new InvalidCastException();
	}

	public int ToInt()
	{
		int num = 0;
		int num2 = Start + Length;
		bool flag = false;
		for (int i = Start; i < num2; i++)
		{
			char c = Source[i];
			switch (c)
			{
			case '-':
				flag = true;
				break;
			case '0':
			case '1':
			case '2':
			case '3':
			case '4':
			case '5':
			case '6':
			case '7':
			case '8':
			case '9':
			{
				int num3 = c - 48;
				num = 10 * num + num3;
				break;
			}
			}
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	public bool StartsWith(string v)
	{
		if (v.Length > Length)
		{
			return false;
		}
		for (int i = 0; i < v.Length; i++)
		{
			if (Source[i + Start] != v[i])
			{
				return false;
			}
		}
		return true;
	}
}
