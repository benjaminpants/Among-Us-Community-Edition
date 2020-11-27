using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000059 RID: 89
[CreateAssetMenu]
public class FontExtensionData : ScriptableObject
{
	// Token: 0x060001ED RID: 493 RVA: 0x00010B04 File Offset: 0x0000ED04
	public void Prepare(Dictionary<int, Dictionary<int, float>> outside)
	{
		if (this.fastKern != null)
		{
			return;
		}
		this.fastKern = new Dictionary<int, Dictionary<int, int>>();
		for (int i = 0; i < this.kernings.Count; i++)
		{
			KerningPair kerningPair = this.kernings[i];
			Dictionary<int, int> dictionary;
			if (!this.fastKern.TryGetValue((int)kerningPair.First, out dictionary))
			{
				dictionary = (this.fastKern[(int)kerningPair.First] = new Dictionary<int, int>());
			}
			dictionary[(int)kerningPair.Second] = kerningPair.Pixels;
			Dictionary<int, float> dictionary2;
			if (!outside.TryGetValue((int)kerningPair.First, out dictionary2))
			{
				dictionary2 = (outside[(int)kerningPair.First] = new Dictionary<int, float>());
			}
			dictionary2[(int)kerningPair.Second] = (float)kerningPair.Pixels;
		}
	}

	// Token: 0x040001DD RID: 477
	public string FontName;

	// Token: 0x040001DE RID: 478
	public List<KerningPair> kernings = new List<KerningPair>();

	// Token: 0x040001DF RID: 479
	public Dictionary<int, Dictionary<int, int>> fastKern;
}
