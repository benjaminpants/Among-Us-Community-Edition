using System;
using UnityEngine;

// Token: 0x0200004E RID: 78
[Serializable]
public struct Vector2Range
{
	// Token: 0x1700005D RID: 93
	// (get) Token: 0x060001CB RID: 459 RVA: 0x000031B9 File Offset: 0x000013B9
	public float Width
	{
		get
		{
			return this.max.x - this.min.x;
		}
	}

	// Token: 0x1700005E RID: 94
	// (get) Token: 0x060001CC RID: 460 RVA: 0x000031D2 File Offset: 0x000013D2
	public float Height
	{
		get
		{
			return this.max.y - this.min.y;
		}
	}

	// Token: 0x060001CD RID: 461 RVA: 0x000031EB File Offset: 0x000013EB
	public Vector2Range(Vector2 min, Vector2 max)
	{
		this.min = min;
		this.max = max;
	}

	// Token: 0x060001CE RID: 462 RVA: 0x00010218 File Offset: 0x0000E418
	public void LerpUnclamped(ref Vector3 output, float t, float z)
	{
		output.Set(Mathf.LerpUnclamped(this.min.x, this.max.x, t), Mathf.LerpUnclamped(this.min.y, this.max.y, t), z);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x00010264 File Offset: 0x0000E464
	public void Lerp(ref Vector3 output, float t, float z)
	{
		output.Set(Mathf.Lerp(this.min.x, this.max.x, t), Mathf.Lerp(this.min.y, this.max.y, t), z);
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x000031FB File Offset: 0x000013FB
	public Vector2 Next()
	{
		return new Vector2(UnityEngine.Random.Range(this.min.x, this.max.x), UnityEngine.Random.Range(this.min.y, this.max.y));
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x000102B0 File Offset: 0x0000E4B0
	public static Vector2 NextEdge()
	{
		float f = 6.28318548f * UnityEngine.Random.value;
		float x = Mathf.Cos(f);
		float y = Mathf.Sin(f);
		return new Vector2(x, y);
	}

	// Token: 0x040001B7 RID: 439
	public Vector2 min;

	// Token: 0x040001B8 RID: 440
	public Vector2 max;
}
