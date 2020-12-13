using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class FontLoader
{
	public static FontData FromBinary(TextAsset dataSrc, FontExtensionData eData)
	{
		FontData fontData = new FontData();
		if (eData != null)
		{
			eData.Prepare(fontData.kernings);
		}
		using MemoryStream memoryStream = new MemoryStream(dataSrc.bytes);
		using BinaryReader binaryReader = new BinaryReader(memoryStream);
		memoryStream.Position += 4L;
		while (memoryStream.Position < memoryStream.Length)
		{
			byte b = binaryReader.ReadByte();
			int num = binaryReader.ReadInt32();
			long position = memoryStream.Position;
			switch (b)
			{
			default:
				memoryStream.Position += num;
				continue;
			case 2:
				fontData.LineHeight = (int)binaryReader.ReadUInt16();
				memoryStream.Position += 2L;
				fontData.TextureSize = new Vector2((int)binaryReader.ReadUInt16(), (int)binaryReader.ReadUInt16());
				memoryStream.Position = position + num;
				continue;
			case 4:
			{
				int num2 = num / 20;
				fontData.charMap = new Dictionary<int, int>(num2);
				fontData.bounds.Capacity = num2;
				fontData.offsets.Capacity = num2;
				fontData.Channels.Capacity = num2;
				fontData.kernings = new Dictionary<int, Dictionary<int, float>>(256);
				for (int i = 0; i < num2; i++)
				{
					int key = binaryReader.ReadInt32();
					int num3 = binaryReader.ReadUInt16();
					int num4 = binaryReader.ReadUInt16();
					int num5 = binaryReader.ReadUInt16();
					int num6 = binaryReader.ReadUInt16();
					int num7 = binaryReader.ReadInt16();
					int num8 = binaryReader.ReadInt16();
					int num9 = binaryReader.ReadInt16();
					binaryReader.ReadByte();
					int input = binaryReader.ReadByte();
					fontData.charMap.Add(key, fontData.bounds.Count);
					fontData.bounds.Add(new Vector4(num3, num4, num5, num6));
					fontData.offsets.Add(new Vector3(num7, num8, num9));
					fontData.Channels.Add(IntToChannels(input));
				}
				continue;
			}
			case 5:
				break;
			}
			while (memoryStream.Position < position + num)
			{
				int key2 = binaryReader.ReadInt32();
				int key3 = binaryReader.ReadInt32();
				int num10 = binaryReader.ReadInt16();
				if (eData != null && eData.fastKern.TryGetValue(key2, out var value) && value.TryGetValue(key3, out var value2))
				{
					num10 += value2;
				}
				if (!fontData.kernings.TryGetValue(key2, out var value3))
				{
					fontData.kernings.Add(key2, value3 = new Dictionary<int, float>(256));
				}
				value3.Add(key3, num10);
			}
		}
		return fontData;
	}

	public static FontData FromText(TextAsset dataSrc, FontExtensionData eData)
	{
		FontData fontData = new FontData();
		if (eData != null)
		{
			eData.Prepare(fontData.kernings);
		}
		List<SubString> list = new List<SubString>(25);
		SubStringReader subStringReader = new SubStringReader(dataSrc.text);
		SubString subString = subStringReader.ReadLine();
		while (subString.Source != null)
		{
			if (subString.StartsWith("common "))
			{
				subString.SafeSplit(list, ' ');
				fontData.LineHeight = list[1].GetKvpValue();
				int kvpValue = list[3].GetKvpValue();
				int kvpValue2 = list[4].GetKvpValue();
				fontData.TextureSize = new Vector2(kvpValue, kvpValue2);
			}
			else if (subString.StartsWith("chars "))
			{
				subString.SafeSplit(list, ' ');
				int capacity = list[1].GetKvpValue() + 1;
				fontData.charMap = new Dictionary<int, int>(capacity);
				fontData.kernings = new Dictionary<int, Dictionary<int, float>>(256);
				fontData.bounds.Capacity = capacity;
				fontData.offsets.Capacity = capacity;
				fontData.Channels.Capacity = capacity;
			}
			else if (subString.StartsWith("char "))
			{
				subString.SafeSplit(list, ' ');
				int kvpValue3 = list[1].GetKvpValue();
				int kvpValue4 = list[2].GetKvpValue();
				int kvpValue5 = list[3].GetKvpValue();
				int kvpValue6 = list[4].GetKvpValue();
				int kvpValue7 = list[5].GetKvpValue();
				int kvpValue8 = list[6].GetKvpValue();
				int kvpValue9 = list[7].GetKvpValue();
				int kvpValue10 = list[8].GetKvpValue();
				int kvpValue11 = list[10].GetKvpValue();
				fontData.charMap.Add(kvpValue3, fontData.bounds.Count);
				fontData.bounds.Add(new Vector4(kvpValue4, kvpValue5, kvpValue6, kvpValue7));
				fontData.offsets.Add(new Vector3(kvpValue8, kvpValue9, kvpValue10));
				fontData.Channels.Add(IntToChannels(kvpValue11));
			}
			else if (subString.StartsWith("kerning "))
			{
				subString.SafeSplit(list, ' ');
				int kvpValue12 = list[1].GetKvpValue();
				int kvpValue13 = list[2].GetKvpValue();
				int num = list[3].GetKvpValue();
				if (eData != null && eData.fastKern.TryGetValue(kvpValue12, out var value) && value.TryGetValue(kvpValue13, out var value2))
				{
					num += value2;
				}
				if (!fontData.kernings.TryGetValue(kvpValue12, out var value3))
				{
					fontData.kernings.Add(kvpValue12, value3 = new Dictionary<int, float>(256));
				}
				value3.Add(kvpValue13, num);
			}
			subString = subStringReader.ReadLine();
		}
		return fontData;
	}

	private static Vector4 IntToChannels(int input)
	{
		Vector4 result = default(Vector4);
		for (int i = 0; i < 4; i++)
		{
			if (((input >> i) & 1) == 1)
			{
				result[i] = 1f;
			}
		}
		return result;
	}
}
