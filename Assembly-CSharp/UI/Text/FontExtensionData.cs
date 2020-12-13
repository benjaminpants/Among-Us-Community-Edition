using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FontExtensionData : ScriptableObject
{
	public string FontName;

	public List<KerningPair> kernings = new List<KerningPair>();

	public Dictionary<int, Dictionary<int, int>> fastKern;

	public void Prepare(Dictionary<int, Dictionary<int, float>> outside)
	{
		if (fastKern != null)
		{
			return;
		}
		fastKern = new Dictionary<int, Dictionary<int, int>>();
		for (int i = 0; i < kernings.Count; i++)
		{
			KerningPair kerningPair = kernings[i];
			if (!fastKern.TryGetValue(kerningPair.First, out var value))
			{
				Dictionary<int, int> dictionary2 = (fastKern[kerningPair.First] = new Dictionary<int, int>());
				value = dictionary2;
			}
			value[kerningPair.Second] = kerningPair.Pixels;
			if (!outside.TryGetValue(kerningPair.First, out var value2))
			{
				Dictionary<int, float> dictionary4 = (outside[kerningPair.First] = new Dictionary<int, float>());
				value2 = dictionary4;
			}
			value2[kerningPair.Second] = kerningPair.Pixels;
		}
	}
}
