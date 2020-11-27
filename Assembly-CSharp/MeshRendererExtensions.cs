using System;
using UnityEngine;

// Token: 0x02000041 RID: 65
public static class MeshRendererExtensions
{
	// Token: 0x06000174 RID: 372 RVA: 0x00002DB5 File Offset: 0x00000FB5
	public static void SetSprite(this MeshRenderer self, Texture2D spr)
	{
		if (spr != null)
		{
			self.SetCutout(spr);
			self.material.color = Color.white;
			return;
		}
		self.SetCutout(null);
		self.material.color = Color.clear;
	}

	// Token: 0x06000175 RID: 373 RVA: 0x00002DEF File Offset: 0x00000FEF
	public static void SetCutout(this MeshRenderer self, Texture2D txt)
	{
		self.material.SetTexture("_MainTex", txt);
		self.material.SetTexture("_EmissionMap", txt);
	}
}
