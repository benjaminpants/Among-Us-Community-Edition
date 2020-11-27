using System;
using UnityEngine;

// Token: 0x02000047 RID: 71
public class BoolRange
{
	// Token: 0x06000188 RID: 392 RVA: 0x00002EA7 File Offset: 0x000010A7
	public static bool Next(float p = 0.5f)
	{
		return UnityEngine.Random.value <= p;
	}
}
