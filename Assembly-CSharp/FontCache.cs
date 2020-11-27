using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class FontCache : MonoBehaviour
{
	// Token: 0x060001E2 RID: 482 RVA: 0x000032CD File Offset: 0x000014CD
	public void OnEnable()
	{
		if (!FontCache.Instance)
		{
			FontCache.Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			return;
		}
		if (FontCache.Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0001094C File Offset: 0x0000EB4C
	public void SetFont(TextRenderer self, string name)
	{
		if (self.FontData.name == name)
		{
			return;
		}
		for (int i = 0; i < this.DefaultFonts.Count; i++)
		{
			if (this.DefaultFonts[i].name == name)
			{
				MeshRenderer component = self.GetComponent<MeshRenderer>();
				Material material = component.material;
				self.FontData = this.DefaultFonts[i];
				component.sharedMaterial = this.DefaultFontMaterials[i];
				component.material.SetColor("_OutlineColor", material.GetColor("_OutlineColor"));
				component.material.SetInt("_Mask", material.GetInt("_Mask"));
				return;
			}
		}
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00010A0C File Offset: 0x0000EC0C
	public FontData LoadFont(TextAsset dataSrc)
	{
		if (this.cache == null)
		{
			this.cache = new Dictionary<string, FontData>();
		}
		FontData fontData;
		if (this.cache.TryGetValue(dataSrc.name, out fontData))
		{
			return fontData;
		}
		int num = this.extraData.FindIndex((FontExtensionData ed) => ed.FontName.Equals(dataSrc.name, StringComparison.OrdinalIgnoreCase));
		FontExtensionData eData = null;
		if (num >= 0)
		{
			eData = this.extraData[num];
		}
		TextAsset textAsset = this.BinaryFonts.FirstOrDefault((TextAsset fnt) => fnt.name.Equals(dataSrc.name));
		fontData = FontCache.LoadFontUncached(textAsset ? textAsset : dataSrc, textAsset, eData);
		this.cache[dataSrc.name] = fontData;
		return fontData;
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00003305 File Offset: 0x00001505
	public static FontData LoadFontUncached(TextAsset dataSrc, bool loadBinary, FontExtensionData eData = null)
	{
		if (loadBinary)
		{
			return FontLoader.FromBinary(dataSrc, eData);
		}
		return FontLoader.FromText(dataSrc, eData);
	}

	// Token: 0x040001CC RID: 460
	public static FontCache Instance;

	// Token: 0x040001CD RID: 461
	private Dictionary<string, FontData> cache = new Dictionary<string, FontData>();

	// Token: 0x040001CE RID: 462
	public List<FontExtensionData> extraData = new List<FontExtensionData>();

	// Token: 0x040001CF RID: 463
	public List<TextAsset> BinaryFonts = new List<TextAsset>();

	// Token: 0x040001D0 RID: 464
	public List<TextAsset> DefaultFonts = new List<TextAsset>();

	// Token: 0x040001D1 RID: 465
	public List<Material> DefaultFontMaterials = new List<Material>();
}
