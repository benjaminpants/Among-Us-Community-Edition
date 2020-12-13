using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FloatRange
{
	public float min;

	public float max;

	public float Last
	{
		get;
		private set;
	}

	public float Width => max - min;

	public FloatRange(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	public float ChangeRange(float y, float min, float max)
	{
		return Mathf.Lerp(min, max, (y - this.min) / Width);
	}

	public float Clamp(float value)
	{
		return Mathf.Clamp(value, min, max);
	}

	public bool Contains(float t)
	{
		if (min <= t)
		{
			return max >= t;
		}
		return false;
	}

	public float CubicLerp(float v)
	{
		if (min >= max)
		{
			return min;
		}
		v = Mathf.Clamp(0f, 1f, v);
		return v * v * v * (max - min) + min;
	}

	public float EitherOr()
	{
		if (!(UnityEngine.Random.value > 0.5f))
		{
			return max;
		}
		return min;
	}

	public float LerpUnclamped(float v)
	{
		return Mathf.LerpUnclamped(min, max, v);
	}

	public float Lerp(float v)
	{
		return Mathf.Lerp(min, max, v);
	}

	public float ExpOutLerp(float v)
	{
		return Lerp(1f - Mathf.Pow(2f, -10f * v));
	}

	public static float ExpOutLerp(float v, float min, float max)
	{
		return Mathf.Lerp(min, max, 1f - Mathf.Pow(2f, -10f * v));
	}

	public static float Next(float min, float max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	public float Next()
	{
		return Last = UnityEngine.Random.Range(min, max);
	}

	public IEnumerable<float> Range(int numStops)
	{
		float i = 0f;
		while (i <= (float)numStops)
		{
			yield return Mathf.Lerp(min, max, i / (float)numStops);
			float num = i + 1f;
			i = num;
		}
	}

	public IEnumerable<float> RandomRange(int numStops)
	{
		float i = 0f;
		while (i <= (float)numStops)
		{
			yield return Next();
			float num = i + 1f;
			i = num;
		}
	}

	internal float ReverseLerp(float t)
	{
		return Mathf.Clamp((t - min) / Width, 0f, 1f);
	}

	public static float ReverseLerp(float t, float min, float max)
	{
		float num = max - min;
		return Mathf.Clamp((t - min) / num, 0f, 1f);
	}

	public IEnumerable<float> SpreadToEdges(int stops)
	{
		return SpreadToEdges(min, max, stops);
	}

	public IEnumerable<float> SpreadEvenly(int stops)
	{
		return SpreadEvenly(min, max, stops);
	}

	public static IEnumerable<float> SpreadToEdges(float min, float max, int stops)
	{
		if (stops != 1)
		{
			int i = 0;
			while (i < stops)
			{
				yield return Mathf.Lerp(min, max, (float)i / ((float)stops - 1f));
				int num = i + 1;
				i = num;
			}
		}
	}

	public static IEnumerable<float> SpreadEvenly(float min, float max, int stops)
	{
		float step = 1f / ((float)stops + 1f);
		for (float i = step; i < 1f; i += step)
		{
			yield return Mathf.Lerp(min, max, i);
		}
	}
}
