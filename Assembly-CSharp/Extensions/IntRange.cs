using System;
using UnityEngine;

[Serializable]
public class IntRange
{
	public int min;

	public int max;

	public IntRange()
	{
	}

	public IntRange(int min, int max)
	{
		this.min = min;
		this.max = max;
	}

	public int Next()
	{
		return UnityEngine.Random.Range(min, max);
	}

	public bool Contains(int value)
	{
		if (min <= value)
		{
			return max >= value;
		}
		return false;
	}

	public static int Next(int max)
	{
		return (int)(UnityEngine.Random.value * (float)max);
	}

	internal static int Next(int min, int max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	internal static byte NextByte(byte max)
	{
		return (byte)UnityEngine.Random.Range(0, max);
	}

	public static void FillRandom(sbyte min, sbyte max, sbyte[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (sbyte)Next(min, max);
		}
	}

	public static int RandomSign()
	{
		if (!BoolRange.Next())
		{
			return -1;
		}
		return 1;
	}

	public static void FillRandomRange(sbyte[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (sbyte)i;
		}
		array.Shuffle();
	}
}
