using UnityEngine;

public static class StringExtensions
{
	private static char[] buffer = new char[256];

	public static string Lerp(string a, string b, float t)
	{
		int num = Mathf.Max(a.Length, b.Length);
		int num2 = (int)Mathf.Lerp(0f, num, t);
		for (int i = 0; i < num; i++)
		{
			if (i < num2)
			{
				if (i < b.Length)
				{
					buffer[i] = b[i];
				}
				else
				{
					buffer[i] = ' ';
				}
			}
			else if (i < a.Length)
			{
				buffer[i] = a[i];
			}
		}
		return new string(buffer, 0, num);
	}
}
