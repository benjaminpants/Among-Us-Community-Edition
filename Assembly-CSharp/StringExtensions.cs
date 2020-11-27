using System;
using UnityEngine;

// Token: 0x02000054 RID: 84
public static class StringExtensions
{
	// Token: 0x060001E0 RID: 480 RVA: 0x000108C4 File Offset: 0x0000EAC4
	public static string Lerp(string a, string b, float t)
	{
		int num = Mathf.Max(a.Length, b.Length);
		int num2 = (int)Mathf.Lerp(0f, (float)num, t);
		for (int i = 0; i < num; i++)
		{
			if (i < num2)
			{
				if (i < b.Length)
				{
					StringExtensions.buffer[i] = b[i];
				}
				else
				{
					StringExtensions.buffer[i] = ' ';
				}
			}
			else if (i < a.Length)
			{
				StringExtensions.buffer[i] = a[i];
			}
		}
		return new string(StringExtensions.buffer, 0, num);
	}

	// Token: 0x040001CB RID: 459
	private static char[] buffer = new char[256];
}
