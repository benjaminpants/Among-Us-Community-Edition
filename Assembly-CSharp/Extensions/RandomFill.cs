using System.Collections.Generic;
using System.Linq;

public class RandomFill<T>
{
	private T[] values;

	private int idx;

	public void Set(IEnumerable<T> values)
	{
		if (this.values == null)
		{
			this.values = values.ToArray();
			this.values.Shuffle();
			idx = this.values.Length - 1;
		}
	}

	public T Get()
	{
		if (idx < 0)
		{
			values.Shuffle();
			idx = values.Length - 1;
		}
		return values[idx--];
	}
}
