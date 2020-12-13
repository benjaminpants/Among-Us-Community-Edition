using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FontCache : MonoBehaviour
{
	public static FontCache Instance;

	private Dictionary<string, FontData> cache = new Dictionary<string, FontData>();

	public List<FontExtensionData> extraData = new List<FontExtensionData>();

	public List<TextAsset> BinaryFonts = new List<TextAsset>();

	public List<TextAsset> DefaultFonts = new List<TextAsset>();

	public List<Material> DefaultFontMaterials = new List<Material>();

	public void OnEnable()
	{
		if (!Instance)
		{
			Instance = this;
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}
		else if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	public void SetFont(TextRenderer self, string name)
	{
		if (self.FontData.name == name)
		{
			return;
		}
		for (int i = 0; i < DefaultFonts.Count; i++)
		{
			if (DefaultFonts[i].name == name)
			{
				MeshRenderer component = self.GetComponent<MeshRenderer>();
				Material material = component.material;
				self.FontData = DefaultFonts[i];
				component.sharedMaterial = DefaultFontMaterials[i];
				component.material.SetColor("_OutlineColor", material.GetColor("_OutlineColor"));
				component.material.SetInt("_Mask", material.GetInt("_Mask"));
				break;
			}
		}
	}

	public FontData LoadFont(TextAsset dataSrc)
	{
		if (cache == null)
		{
			cache = new Dictionary<string, FontData>();
		}
		if (cache.TryGetValue(dataSrc.name, out var value))
		{
			return value;
		}
		int num = extraData.FindIndex((FontExtensionData ed) => ed.FontName.Equals(dataSrc.name, StringComparison.OrdinalIgnoreCase));
		FontExtensionData eData = null;
		if (num >= 0)
		{
			eData = extraData[num];
		}
		TextAsset textAsset = BinaryFonts.FirstOrDefault((TextAsset fnt) => fnt.name.Equals(dataSrc.name));
		value = LoadFontUncached(textAsset ? textAsset : dataSrc, textAsset, eData);
		cache[dataSrc.name] = value;
		return value;
	}

	public static FontData LoadFontUncached(TextAsset dataSrc, bool loadBinary, FontExtensionData eData = null)
	{
		if (loadBinary)
		{
			return FontLoader.FromBinary(dataSrc, eData);
		}
		return FontLoader.FromText(dataSrc, eData);
	}
}
