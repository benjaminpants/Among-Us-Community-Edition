using UnityEngine;

public class BoolRange
{
	public static bool Next(float p = 0.5f)
	{
		return Random.value <= p;
	}
}
