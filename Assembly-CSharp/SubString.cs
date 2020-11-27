using System;

// Token: 0x0200003F RID: 63
public struct SubString
{
	// Token: 0x0600016C RID: 364 RVA: 0x00002D59 File Offset: 0x00000F59
	public SubString(string source, int start, int length)
	{
		this.Source = source;
		this.Start = start;
		this.Length = length;
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00002D70 File Offset: 0x00000F70
	public override string ToString()
	{
		return this.Source.Substring(this.Start, this.Length);
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000F1E8 File Offset: 0x0000D3E8
	public int GetKvpValue()
	{
		int num = this.Start + this.Length;
		for (int i = this.Start; i < num; i++)
		{
			if (this.Source[i] == '=')
			{
				i++;
				return new SubString(this.Source, i, num - i).ToInt();
			}
		}
		throw new InvalidCastException();
	}

	// Token: 0x0600016F RID: 367 RVA: 0x0000F248 File Offset: 0x0000D448
	public int ToInt()
	{
		int num = 0;
		int num2 = this.Start + this.Length;
		bool flag = false;
		for (int i = this.Start; i < num2; i++)
		{
			char c = this.Source[i];
			if (c == '-')
			{
				flag = true;
			}
			else if (c >= '0' && c <= '9')
			{
				int num3 = (int)(c - '0');
				num = 10 * num + num3;
			}
		}
		if (!flag)
		{
			return num;
		}
		return -num;
	}

	// Token: 0x06000170 RID: 368 RVA: 0x0000F2B4 File Offset: 0x0000D4B4
	public bool StartsWith(string v)
	{
		if (v.Length > this.Length)
		{
			return false;
		}
		for (int i = 0; i < v.Length; i++)
		{
			if (this.Source[i + this.Start] != v[i])
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0400016B RID: 363
	public readonly int Start;

	// Token: 0x0400016C RID: 364
	public readonly int Length;

	// Token: 0x0400016D RID: 365
	public readonly string Source;
}
