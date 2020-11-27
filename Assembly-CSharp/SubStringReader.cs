using System;

// Token: 0x0200003E RID: 62
public class SubStringReader
{
	// Token: 0x0600016A RID: 362 RVA: 0x00002D4A File Offset: 0x00000F4A
	public SubStringReader(string source)
	{
		this.Source = source;
	}

	// Token: 0x0600016B RID: 363 RVA: 0x0000F12C File Offset: 0x0000D32C
	public SubString ReadLine()
	{
		int position = this.Position;
		if (position >= this.Source.Length)
		{
			return default(SubString);
		}
		int num = this.Position;
		int i = position;
		while (i < this.Source.Length)
		{
			char c = this.Source[i];
			if (c == '\r')
			{
				num = i - 1;
				this.Position = i + 1;
				if (i + 1 < this.Source.Length && this.Source[i + 1] == '\n')
				{
					this.Position = i + 2;
					break;
				}
				break;
			}
			else
			{
				if (c == '\n')
				{
					num = i - 1;
					this.Position = i + 1;
					break;
				}
				i++;
			}
		}
		return new SubString(this.Source, position, num - position);
	}

	// Token: 0x04000169 RID: 361
	private readonly string Source;

	// Token: 0x0400016A RID: 362
	private int Position;
}
