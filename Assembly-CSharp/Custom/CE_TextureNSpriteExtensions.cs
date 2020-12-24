using System.IO;
using UnityEngine;
using System.Collections.Generic;

public class CE_TextureNSpriteExtensions
{

	public static Dictionary<string, Texture2D> GlobalLoadedTextures = new Dictionary<string, Texture2D>();


	public static Texture2D LoadPNG(string filePath)
	{
		if (CE_TextureNSpriteExtensions.GlobalLoadedTextures.ContainsKey(filePath)) return CE_TextureNSpriteExtensions.GlobalLoadedTextures[filePath];
		else
		{
			Texture2D texture2D = null;
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(data);
			}
			CE_TextureNSpriteExtensions.GlobalLoadedTextures.Add(filePath, texture2D);
			return texture2D;
		}
	}

	public static Texture2D LoadPNG_HatManager(string filePath)
	{
		if (DestroyableSingleton<HatManager>.Instance.LoadedTextures.ContainsKey(filePath)) return DestroyableSingleton<HatManager>.Instance.LoadedTextures[filePath];
		else
        {
			Texture2D texture2D = null;
			if (File.Exists(filePath))
			{
				byte[] data = File.ReadAllBytes(filePath);
				texture2D = new Texture2D(2, 2);
				texture2D.LoadImage(data);
			}
			DestroyableSingleton<HatManager>.Instance.LoadedTextures.Add(filePath, texture2D);
			return texture2D;
		}
	}

	public static Sprite ConvertToSprite(Texture2D texture, Vector2 pivot)
	{
		return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), pivot);
	}

	public static Sprite ConvertToSprite(Texture2D texture)
	{
		return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), Vector2.zero);
	}

	public static Texture2D MakeTex(int width, int height, Color col)
	{
		Color[] array = new Color[width * height];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = col;
		}
		Texture2D texture2D = new Texture2D(width, height);
		texture2D.SetPixels(array);
		texture2D.Apply();
		return texture2D;
	}

	public static Sprite ConvertToSpriteAutoPivot(Texture2D texture)
	{
		return Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));
	}
}
