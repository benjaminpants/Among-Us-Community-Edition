using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

// Token: 0x020002D0 RID: 720
public class HatLoader
{
	// Token: 0x06000EF8 RID: 3832 RVA: 0x0004121C File Offset: 0x0003F41C
	public static Texture2D LoadPNG(string filePath)
	{
		Texture2D texture2D = null;
		if (File.Exists(filePath))
		{
			byte[] data = File.ReadAllBytes(filePath);
			texture2D = new Texture2D(2, 2);
			texture2D.LoadImage(data);
		}
		return texture2D;
	}

	// Token: 0x06000EF9 RID: 3833 RVA: 0x0004124C File Offset: 0x0003F44C
	public static List<HatBehaviour> LoadHats()
	{
		List<HatBehaviour> list = new List<HatBehaviour>();
		string text = Path.Combine(Directory.GetCurrentDirectory(), "Hats");
		List<CustomHatDefinition> list2 = new List<CustomHatDefinition>();
		FileInfo[] files = new DirectoryInfo(text).GetFiles("*.json");
		for (int i = 0; i < files.Length; i++)
		{
			Debug.Log(files[i].FullName);
			using (StreamReader streamReader = File.OpenText(files[i].FullName))
			{
				try
				{
					CustomHatDefinition item = (CustomHatDefinition)new JsonSerializer().Deserialize(streamReader, typeof(CustomHatDefinition));
					list2.Add(item);
				}
				catch (Exception ex)
				{
					Debug.Log(ex.Message);
				}
			}
		}
		foreach (CustomHatDefinition customHatDefinition in list2)
		{
			try
			{
				HatBehaviour hatBehaviour = new HatBehaviour();
				hatBehaviour.InFront = customHatDefinition.inFront;
				hatBehaviour.ProductId = customHatDefinition.ID;
				Texture2D texture = HatLoader.LoadPNG(Path.Combine(text, customHatDefinition.NormalImg));
				Texture2D texture2 = HatLoader.LoadPNG(Path.Combine(text, customHatDefinition.FloorImg));
				if (customHatDefinition.UsePointFiltering)
				{
					texture.filterMode = FilterMode.Point;
				}
				hatBehaviour.MainImage = Sprite.Create(texture, new Rect(new Vector2(customHatDefinition.NormalPosX, customHatDefinition.NormalPosY), new Vector2(customHatDefinition.NormalWidth, customHatDefinition.NormalHeight)), new Vector2(customHatDefinition.NormalPivotX, customHatDefinition.NormalPivotY));
				hatBehaviour.FloorImage = Sprite.Create(texture2, new Rect(new Vector2(customHatDefinition.FloorPosX, customHatDefinition.FloorPosY), new Vector2(customHatDefinition.FloorWidth, customHatDefinition.FloorHeight)), new Vector2(customHatDefinition.FloorPivotX, customHatDefinition.FloorPivotY));
				list.Add(hatBehaviour);
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.Message);
			}
		}
		return list;
	}
}
