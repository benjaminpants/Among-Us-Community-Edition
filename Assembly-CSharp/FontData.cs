using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000057 RID: 87
[Serializable]
public class FontData
{
	// Token: 0x060001EA RID: 490 RVA: 0x00010AD4 File Offset: 0x0000ECD4
	public float GetKerning(int last, int cur)
	{
		Dictionary<int, float> dictionary;
		float result;
		if (this.kernings.TryGetValue(last, out dictionary) && dictionary.TryGetValue(cur, out result))
		{
			return result;
		}
		return 0f;
	}

	// Token: 0x040001D3 RID: 467
	public Vector2 TextureSize = new Vector2(256f, 256f);

	// Token: 0x040001D4 RID: 468
	public List<Vector4> bounds = new List<Vector4>();

	// Token: 0x040001D5 RID: 469
	public List<Vector3> offsets = new List<Vector3>();

	// Token: 0x040001D6 RID: 470
	public List<Vector4> Channels = new List<Vector4>();

	// Token: 0x040001D7 RID: 471
	public Dictionary<int, int> charMap;

	// Token: 0x040001D8 RID: 472
	public float LineHeight;

	// Token: 0x040001D9 RID: 473
	public Dictionary<int, Dictionary<int, float>> kernings;
}
