using System;
using UnityEngine;

// Token: 0x020000CF RID: 207
public static class CooldownHelpers
{
	// Token: 0x0600045D RID: 1117 RVA: 0x0001ABC8 File Offset: 0x00018DC8
	public static void SetCooldownNormalizedUvs(this SpriteRenderer myRend)
	{
		Vector2[] uv = myRend.sprite.uv;
		Vector4 vector = new Vector4(2f, -1f);
		for (int i = 0; i < uv.Length; i++)
		{
			if (vector.x > uv[i].y)
			{
				vector.x = uv[i].y;
			}
			if (vector.y < uv[i].y)
			{
				vector.y = uv[i].y;
			}
		}
		vector.y -= vector.x;
		myRend.material.SetVector("_NormalizedUvs", vector);
	}
}
