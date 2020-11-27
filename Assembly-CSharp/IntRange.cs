using System;
using UnityEngine;

// Token: 0x02000048 RID: 72
[Serializable]
public class IntRange
{
	// Token: 0x0600018A RID: 394 RVA: 0x000023C4 File Offset: 0x000005C4
	public IntRange()
	{
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00002EB4 File Offset: 0x000010B4
	public IntRange(int min, int max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00002ECA File Offset: 0x000010CA
	public int Next()
	{
		return UnityEngine.Random.Range(this.min, this.max);
	}

	// Token: 0x0600018D RID: 397 RVA: 0x00002EDD File Offset: 0x000010DD
	public bool Contains(int value)
	{
		return this.min <= value && this.max >= value;
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00002EF6 File Offset: 0x000010F6
	public static int Next(int max)
	{
		return (int)(UnityEngine.Random.value * (float)max);
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00002F01 File Offset: 0x00001101
	internal static int Next(int min, int max)
	{
		return UnityEngine.Random.Range(min, max);
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00002F0A File Offset: 0x0000110A
	internal static byte NextByte(byte max)
	{
		return (byte)UnityEngine.Random.Range(0, (int)max);
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000FD94 File Offset: 0x0000DF94
	public static void FillRandom(sbyte min, sbyte max, sbyte[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (sbyte)IntRange.Next((int)min, (int)max);
		}
	}

	// Token: 0x06000192 RID: 402 RVA: 0x00002F14 File Offset: 0x00001114
	public static int RandomSign()
	{
		if (!BoolRange.Next(0.5f))
		{
			return -1;
		}
		return 1;
	}

	// Token: 0x06000193 RID: 403 RVA: 0x0000FDBC File Offset: 0x0000DFBC
	public static void FillRandomRange(sbyte[] array)
	{
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (sbyte)i;
		}
		array.Shuffle<sbyte>();
	}

	// Token: 0x0400018F RID: 399
	public int min;

	// Token: 0x04000190 RID: 400
	public int max;
}
