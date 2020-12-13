using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FontData
{
	public Vector2 TextureSize = new Vector2(256f, 256f);

	public List<Vector4> bounds = new List<Vector4>();

	public List<Vector3> offsets = new List<Vector3>();

	public List<Vector4> Channels = new List<Vector4>();

	public Dictionary<int, int> charMap;

	public float LineHeight;

	public Dictionary<int, Dictionary<int, float>> kernings;

	public float GetKerning(int last, int cur)
	{
		if (kernings.TryGetValue(last, out var value) && value.TryGetValue(cur, out var value2))
		{
			return value2;
		}
		return 0f;
	}
}
