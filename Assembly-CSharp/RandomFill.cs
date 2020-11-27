using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000046 RID: 70
public class RandomFill<T>
{
	// Token: 0x06000185 RID: 389 RVA: 0x00002E76 File Offset: 0x00001076
	public void Set(IEnumerable<T> values)
	{
		if (this.values == null)
		{
			this.values = values.ToArray<T>();
			this.values.Shuffle<T>();
			this.idx = this.values.Length - 1;
		}
	}

	// Token: 0x06000186 RID: 390 RVA: 0x0000FD44 File Offset: 0x0000DF44
	public T Get()
	{
		if (this.idx < 0)
		{
			this.values.Shuffle<T>();
			this.idx = this.values.Length - 1;
		}
		T[] array = this.values;
		int num = this.idx;
		this.idx = num - 1;
		return array[num];
	}

	// Token: 0x0400018D RID: 397
	private T[] values;

	// Token: 0x0400018E RID: 398
	private int idx;
}
