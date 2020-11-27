using System;

// Token: 0x0200019E RID: 414
public class RingBuffer<T>
{
	// Token: 0x17000148 RID: 328
	// (get) Token: 0x060008BC RID: 2236 RVA: 0x000075C4 File Offset: 0x000057C4
	// (set) Token: 0x060008BD RID: 2237 RVA: 0x000075CC File Offset: 0x000057CC
	public int Count { get; private set; }

	// Token: 0x17000149 RID: 329
	// (get) Token: 0x060008BE RID: 2238 RVA: 0x000075D5 File Offset: 0x000057D5
	public int Capacity
	{
		get
		{
			return this.Data.Length;
		}
	}

	// Token: 0x060008BF RID: 2239 RVA: 0x000075DF File Offset: 0x000057DF
	public RingBuffer(int size)
	{
		this.Data = new T[size];
	}

	// Token: 0x1700014A RID: 330
	public T this[int i]
	{
		get
		{
			int num = (this.startIdx + i) % this.Data.Length;
			return this.Data[num];
		}
	}

	// Token: 0x060008C1 RID: 2241 RVA: 0x000075F3 File Offset: 0x000057F3
	public T First()
	{
		return this.Data[this.startIdx];
	}

	// Token: 0x060008C2 RID: 2242 RVA: 0x0002F5F8 File Offset: 0x0002D7F8
	public T Last()
	{
		int num = (this.startIdx + this.Count - 1) % this.Data.Length;
		return this.Data[num];
	}

	// Token: 0x060008C3 RID: 2243 RVA: 0x0002F62C File Offset: 0x0002D82C
	public void Add(T item)
	{
		int num = (this.startIdx + this.Count) % this.Data.Length;
		this.Data[num] = item;
		if (this.Count < this.Data.Length)
		{
			int count = this.Count;
			this.Count = count + 1;
			return;
		}
		this.startIdx = (this.startIdx + 1) % this.Data.Length;
	}

	// Token: 0x060008C4 RID: 2244 RVA: 0x0002F698 File Offset: 0x0002D898
	public T RemoveFirst()
	{
		if (this.Count == 0)
		{
			throw new InvalidOperationException("Data is empty");
		}
		T result = this.Data[this.startIdx];
		this.startIdx = (this.startIdx + 1) % this.Data.Length;
		int count = this.Count;
		this.Count = count - 1;
		return result;
	}

	// Token: 0x060008C5 RID: 2245 RVA: 0x00007606 File Offset: 0x00005806
	public void Clear()
	{
		this.startIdx = 0;
		this.Count = 0;
	}

	// Token: 0x04000871 RID: 2161
	private readonly T[] Data;

	// Token: 0x04000872 RID: 2162
	private int startIdx;
}
