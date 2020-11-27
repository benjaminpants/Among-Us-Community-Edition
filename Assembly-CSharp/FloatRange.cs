using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000049 RID: 73
[Serializable]
public class FloatRange
{
	// Token: 0x17000053 RID: 83
	// (get) Token: 0x06000194 RID: 404 RVA: 0x00002F25 File Offset: 0x00001125
	// (set) Token: 0x06000195 RID: 405 RVA: 0x00002F2D File Offset: 0x0000112D
	public float Last { get; private set; }

	// Token: 0x17000054 RID: 84
	// (get) Token: 0x06000196 RID: 406 RVA: 0x00002F36 File Offset: 0x00001136
	public float Width
	{
		get
		{
			return this.max - this.min;
		}
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00002F45 File Offset: 0x00001145
	public FloatRange(float min, float max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00002F5B File Offset: 0x0000115B
	public float ChangeRange(float y, float min, float max)
	{
		return Mathf.Lerp(min, max, (y - this.min) / this.Width);
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00002F73 File Offset: 0x00001173
	public float Clamp(float value)
	{
		return Mathf.Clamp(value, this.min, this.max);
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00002F87 File Offset: 0x00001187
	public bool Contains(float t)
	{
		return this.min <= t && this.max >= t;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x0000FDE4 File Offset: 0x0000DFE4
	public float CubicLerp(float v)
	{
		if (this.min >= this.max)
		{
			return this.min;
		}
		v = Mathf.Clamp(0f, 1f, v);
		return v * v * v * (this.max - this.min) + this.min;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00002FA0 File Offset: 0x000011A0
	public float EitherOr()
	{
		if (UnityEngine.Random.value <= 0.5f)
		{
			return this.max;
		}
		return this.min;
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00002FBB File Offset: 0x000011BB
	public float LerpUnclamped(float v)
	{
		return Mathf.LerpUnclamped(this.min, this.max, v);
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00002FCF File Offset: 0x000011CF
	public float Lerp(float v)
	{
		return Mathf.Lerp(this.min, this.max, v);
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00002FE3 File Offset: 0x000011E3
	public float ExpOutLerp(float v)
	{
		return this.Lerp(1f - Mathf.Pow(2f, -10f * v));
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00003002 File Offset: 0x00001202
	public static float ExpOutLerp(float v, float min, float max)
	{
		return Mathf.Lerp(min, max, 1f - Mathf.Pow(2f, -10f * v));
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00003022 File Offset: 0x00001222
	public static float Next(float min, float max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x0000FE34 File Offset: 0x0000E034
	public float Next()
	{
		return this.Last = UnityEngine.Random.Range(this.min, this.max);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x0000302B File Offset: 0x0000122B
	public IEnumerable<float> Range(int numStops)
	{
		float num;
		for (float i = 0f; i <= (float)numStops; i = num)
		{
			yield return Mathf.Lerp(this.min, this.max, i / (float)numStops);
			num = i + 1f;
		}
		yield break;
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00003042 File Offset: 0x00001242
	public IEnumerable<float> RandomRange(int numStops)
	{
		float num;
		for (float i = 0f; i <= (float)numStops; i = num)
		{
			yield return this.Next();
			num = i + 1f;
		}
		yield break;
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00003059 File Offset: 0x00001259
	internal float ReverseLerp(float t)
	{
		return Mathf.Clamp((t - this.min) / this.Width, 0f, 1f);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x0000FE5C File Offset: 0x0000E05C
	public static float ReverseLerp(float t, float min, float max)
	{
		float num = max - min;
		return Mathf.Clamp((t - min) / num, 0f, 1f);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x00003079 File Offset: 0x00001279
	public IEnumerable<float> SpreadToEdges(int stops)
	{
		return FloatRange.SpreadToEdges(this.min, this.max, stops);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0000308D File Offset: 0x0000128D
	public IEnumerable<float> SpreadEvenly(int stops)
	{
		return FloatRange.SpreadEvenly(this.min, this.max, stops);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x000030A1 File Offset: 0x000012A1
	public static IEnumerable<float> SpreadToEdges(float min, float max, int stops)
	{
		if (stops == 1)
		{
			yield break;
		}
		int num;
		for (int i = 0; i < stops; i = num)
		{
			yield return Mathf.Lerp(min, max, (float)i / ((float)stops - 1f));
			num = i + 1;
		}
		yield break;
	}

	// Token: 0x060001AA RID: 426 RVA: 0x000030BF File Offset: 0x000012BF
	public static IEnumerable<float> SpreadEvenly(float min, float max, int stops)
	{
		float step = 1f / ((float)stops + 1f);
		for (float i = step; i < 1f; i += step)
		{
			yield return Mathf.Lerp(min, max, i);
		}
		yield break;
	}

	// Token: 0x04000191 RID: 401
	public float min;

	// Token: 0x04000192 RID: 402
	public float max;
}
