using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x0200005A RID: 90
public static class FontLoader
{
	// Token: 0x060001EF RID: 495 RVA: 0x00010BCC File Offset: 0x0000EDCC
	public static FontData FromBinary(TextAsset dataSrc, FontExtensionData eData)
	{
		FontData fontData = new FontData();
		if (eData != null)
		{
			eData.Prepare(fontData.kernings);
		}
		using (MemoryStream memoryStream = new MemoryStream(dataSrc.bytes))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				memoryStream.Position += 4L;
				while (memoryStream.Position < memoryStream.Length)
				{
					byte b = binaryReader.ReadByte();
					int num = binaryReader.ReadInt32();
					long position = memoryStream.Position;
					switch (b)
					{
					default:
						memoryStream.Position += (long)num;
						break;
					case 2:
						fontData.LineHeight = (float)binaryReader.ReadUInt16();
						memoryStream.Position += 2L;
						fontData.TextureSize = new Vector2((float)binaryReader.ReadUInt16(), (float)binaryReader.ReadUInt16());
						memoryStream.Position = position + (long)num;
						break;
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
							int num3 = (int)binaryReader.ReadUInt16();
							int num4 = (int)binaryReader.ReadUInt16();
							int num5 = (int)binaryReader.ReadUInt16();
							int num6 = (int)binaryReader.ReadUInt16();
							int num7 = (int)binaryReader.ReadInt16();
							int num8 = (int)binaryReader.ReadInt16();
							int num9 = (int)binaryReader.ReadInt16();
							binaryReader.ReadByte();
							int input = (int)binaryReader.ReadByte();
							fontData.charMap.Add(key, fontData.bounds.Count);
							fontData.bounds.Add(new Vector4((float)num3, (float)num4, (float)num5, (float)num6));
							fontData.offsets.Add(new Vector3((float)num7, (float)num8, (float)num9));
							fontData.Channels.Add(FontLoader.IntToChannels(input));
						}
						break;
					}
					case 5:
						while (memoryStream.Position < position + (long)num)
						{
							int key2 = binaryReader.ReadInt32();
							int key3 = binaryReader.ReadInt32();
							int num10 = (int)binaryReader.ReadInt16();
							Dictionary<int, int> dictionary;
							int num11;
							if (eData != null && eData.fastKern.TryGetValue(key2, out dictionary) && dictionary.TryGetValue(key3, out num11))
							{
								num10 += num11;
							}
							Dictionary<int, float> dictionary2;
							if (!fontData.kernings.TryGetValue(key2, out dictionary2))
							{
								fontData.kernings.Add(key2, dictionary2 = new Dictionary<int, float>(256));
							}
							dictionary2.Add(key3, (float)num10);
						}
						break;
					}
				}
			}
		}
		return fontData;
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x00010EA8 File Offset: 0x0000F0A8
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
				fontData.LineHeight = (float)list[1].GetKvpValue();
				int kvpValue = list[3].GetKvpValue();
				int kvpValue2 = list[4].GetKvpValue();
				fontData.TextureSize = new Vector2((float)kvpValue, (float)kvpValue2);
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
				fontData.bounds.Add(new Vector4((float)kvpValue4, (float)kvpValue5, (float)kvpValue6, (float)kvpValue7));
				fontData.offsets.Add(new Vector3((float)kvpValue8, (float)kvpValue9, (float)kvpValue10));
				fontData.Channels.Add(FontLoader.IntToChannels(kvpValue11));
			}
			else if (subString.StartsWith("kerning "))
			{
				subString.SafeSplit(list, ' ');
				int kvpValue12 = list[1].GetKvpValue();
				int kvpValue13 = list[2].GetKvpValue();
				int num = list[3].GetKvpValue();
				Dictionary<int, int> dictionary;
				int num2;
				if (eData != null && eData.fastKern.TryGetValue(kvpValue12, out dictionary) && dictionary.TryGetValue(kvpValue13, out num2))
				{
					num += num2;
				}
				Dictionary<int, float> dictionary2;
				if (!fontData.kernings.TryGetValue(kvpValue12, out dictionary2))
				{
					fontData.kernings.Add(kvpValue12, dictionary2 = new Dictionary<int, float>(256));
				}
				dictionary2.Add(kvpValue13, (float)num);
			}
			subString = subStringReader.ReadLine();
		}
		return fontData;
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x000111B8 File Offset: 0x0000F3B8
	private static Vector4 IntToChannels(int input)
	{
		Vector4 result = default(Vector4);
		for (int i = 0; i < 4; i++)
		{
			if ((input >> i & 1) == 1)
			{
				result[i] = 1f;
			}
		}
		return result;
	}
}
