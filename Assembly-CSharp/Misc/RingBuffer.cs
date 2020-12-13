using System;

public class RingBuffer<T>
{
	private readonly T[] Data;

	private int startIdx;

	public int Count
	{
		get;
		private set;
	}

	public int Capacity => Data.Length;

	public T this[int i]
	{
		get
		{
			int num = (startIdx + i) % Data.Length;
			return Data[num];
		}
	}

	public RingBuffer(int size)
	{
		Data = new T[size];
	}

	public T First()
	{
		return Data[startIdx];
	}

	public T Last()
	{
		int num = (startIdx + Count - 1) % Data.Length;
		return Data[num];
	}

	public void Add(T item)
	{
		int num = (startIdx + Count) % Data.Length;
		Data[num] = item;
		if (Count < Data.Length)
		{
			Count++;
		}
		else
		{
			startIdx = (startIdx + 1) % Data.Length;
		}
	}

	public T RemoveFirst()
	{
		if (Count == 0)
		{
			throw new InvalidOperationException("Data is empty");
		}
		T result = Data[startIdx];
		startIdx = (startIdx + 1) % Data.Length;
		Count--;
		return result;
	}

	public void Clear()
	{
		startIdx = 0;
		Count = 0;
	}
}
