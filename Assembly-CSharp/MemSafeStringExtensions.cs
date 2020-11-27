using System;
using System.Collections.Generic;

// Token: 0x02000040 RID: 64
public static class MemSafeStringExtensions
{
	// Token: 0x06000171 RID: 369 RVA: 0x00002D89 File Offset: 0x00000F89
	public static void SafeSplit(this SubString subString, List<SubString> output, char delim)
	{
		subString.Source.SafeSplit(output, delim, subString.Start, subString.Length);
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00002DA4 File Offset: 0x00000FA4
	public static void SafeSplit(this string source, List<SubString> output, char delim)
	{
		source.SafeSplit(output, delim, 0, source.Length);
	}

	// Token: 0x06000173 RID: 371 RVA: 0x0000F304 File Offset: 0x0000D504
	public static void SafeSplit(this string source, List<SubString> output, char delim, int start, int length)
	{
		output.Clear();
		int num = start;
		int num2 = start + length;
		for (int i = start; i < num2; i++)
		{
			if (source[i] == delim)
			{
				if (num != i)
				{
					output.Add(new SubString(source, num, i - num));
				}
				num = i + 1;
			}
		}
		if (num != num2)
		{
			output.Add(new SubString(source, num, num2 - num));
		}
	}
}
