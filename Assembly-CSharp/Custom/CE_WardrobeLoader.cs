using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class CE_WardrobeLoader
{
	public static float TestPlaybackSpeed;

	public static int TestPlaybackMode;

	public const int TestPlaybackModeMax = 5;

	public static bool TestPlaybackResetAnimations;

	public static bool TestPlaybackPause;

	public static float TestPlaybackPausePosition;

	public static float TestPlaybackPausePositionSkin;

	public static bool AnimationTestingActive;

	public static float TestPlaybackCurrentPosition;

	public static float TestPlaybackCurrentPositionSkin;

	public static float HatPivot;

	public static float[] HatPivotPoints;

	public static bool AnimationDebugMode
	{
		get
		{
			if (SaveManager.EnableAnimationTestingMode)
			{
				return AnimationTestingActive;
			}
			return false;
		}
	}

	public static List<HatBehaviour> LoadHats()
	{
		List<HatBehaviour> list = new List<HatBehaviour>();
		string text = Path.Combine(Directory.GetCurrentDirectory(), "Hats");
		List<CE_CustomHatDefinition> list2 = new List<CE_CustomHatDefinition>();
		FileInfo[] files = new DirectoryInfo(text).GetFiles("*.json");
		for (int i = 0; i < files.Length; i++)
		{
			Debug.Log(files[i].FullName);
			using StreamReader reader = File.OpenText(files[i].FullName);
			try
			{
				CE_CustomHatDefinition item = (CE_CustomHatDefinition)new JsonSerializer().Deserialize(reader, typeof(CE_CustomHatDefinition));
				list2.Add(item);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}
		foreach (CE_CustomHatDefinition item2 in list2)
		{
			try
			{
				HatBehaviour hatBehaviour = new HatBehaviour();
				hatBehaviour.InFront = item2.inFront;
				hatBehaviour.ProductId = item2.ID;
				hatBehaviour.IsCustom = true;
				Texture2D texture2D = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(text, item2.NormalImg));
				Texture2D texture2D2 = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(text, item2.FloorImg));
				Texture2D texture2D3 = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(text, item2.PreviewImg));
				if (item2.UsePointFiltering)
				{
					texture2D.filterMode = FilterMode.Point;
					texture2D2.filterMode = FilterMode.Point;
					if ((bool)texture2D3)
					{
						texture2D3.filterMode = FilterMode.Point;
					}
				}
				hatBehaviour.MainImage = Sprite.Create(texture2D, new Rect(new Vector2(item2.NormalPosX, item2.NormalPosY), new Vector2(item2.NormalWidth, item2.NormalHeight)), new Vector2(item2.NormalPivotX, item2.NormalPivotY));
				hatBehaviour.FloorImage = Sprite.Create(texture2D2, new Rect(new Vector2(item2.FloorPosX, item2.FloorPosY), new Vector2(item2.FloorWidth, item2.FloorHeight)), new Vector2(item2.FloorPivotX, item2.FloorPivotY));
				if ((bool)texture2D3)
				{
					hatBehaviour.PreviewImage = Sprite.Create(texture2D3, new Rect(new Vector2(item2.PreviewPosX, item2.PreviewPosY), new Vector2(item2.PreviewWidth, item2.PreviewHeight)), new Vector2(item2.PreviewPivotX, item2.PreviewPivotY));
				}
				list.Add(hatBehaviour);
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.Message);
			}
		}
		return list;
	}

	public static List<SkinData> LoadSkins(SkinData BaseSkin)
	{
		List<SkinData> list = new List<SkinData>();
		string text = Path.Combine(Directory.GetCurrentDirectory(), "Skins");
		List<CE_CustomSkinDefinition> list2 = new List<CE_CustomSkinDefinition>();
		FileInfo[] files = new DirectoryInfo(text).GetFiles("*.json");
		for (int i = 0; i < files.Length; i++)
		{
			Debug.Log(files[i].FullName);
			using StreamReader reader = File.OpenText(files[i].FullName);
			try
			{
				CE_CustomSkinDefinition item = (CE_CustomSkinDefinition)new JsonSerializer().Deserialize(reader, typeof(CE_CustomSkinDefinition));
				list2.Add(item);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}
		int num = 8;
		foreach (CE_CustomSkinDefinition item2 in list2)
		{
			try
			{
				SkinData skinData = new SkinData();
				skinData.name = item2.ID;
				skinData.isCustom = true;
				skinData.StoreName = item2.ID;
				skinData.Free = true;
				skinData.Order = num;
				skinData.SpawnAnim = BaseSkin.SpawnAnim;
				skinData.IdleAnim = BaseSkin.IdleAnim;
				skinData.RunAnim = BaseSkin.RunAnim;
				skinData.EnterVentAnim = BaseSkin.EnterVentAnim;
				skinData.ExitVentAnim = BaseSkin.ExitVentAnim;
				skinData.EjectFrame = BaseSkin.EjectFrame;
				skinData.KillNeckVictim = BaseSkin.KillNeckVictim;
				skinData.KillShootImpostor = BaseSkin.KillShootImpostor;
				skinData.KillShootVictim = BaseSkin.KillShootVictim;
				skinData.KillStabVictim = BaseSkin.KillStabVictim;
				skinData.KillTongueImpostor = BaseSkin.KillTongueImpostor;
				skinData.KillTongueVictim = BaseSkin.KillTongueVictim;
				foreach (CE_CustomSkinDefinition.CustomSkinFrame frame in item2.FrameList)
				{
					frame.Texture = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(text, frame.SpritePath));
					skinData.FrameList.Add(frame.Name, frame);
				}
				if (skinData.FrameList.ContainsKey("Display"))
				{
					CE_CustomSkinDefinition.CustomSkinFrame customSkinFrame = skinData.FrameList["Display"];
					float x = customSkinFrame.Position.x;
					float y = customSkinFrame.Position.y;
					float x2 = customSkinFrame.Size.x;
					float y2 = customSkinFrame.Size.y;
					float x3 = customSkinFrame.Offset.x;
					float y3 = customSkinFrame.Offset.y;
					Texture2D texture = customSkinFrame.Texture;
					skinData.IdleFrame = Sprite.Create(texture, new Rect(x, y, x2, y2), new Vector2(x3, y3));
				}
				num++;
				list.Add(skinData);
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.Message);
			}
		}
		return list;
	}

	static CE_WardrobeLoader()
	{
		TestPlaybackSpeed = 0.05f;
		TestPlaybackMode = 0;
		TestPlaybackPause = false;
		TestPlaybackPausePosition = -1f;
		TestPlaybackPausePositionSkin = -1f;
		TestPlaybackCurrentPosition = -1f;
		TestPlaybackCurrentPositionSkin = -1f;
		HatPivotPoints = new float[14]
		{
			0.019f,
			0.05f,
			0.02f,
			-0.04f,
			-0.09f,
			-0.09f,
			0.059f,
			0.089f,
			0.06f,
			0f,
			-0.12f,
			-0.129f,
			0.3f,
			0.3f
		};
		TestPlaybackResetAnimations = false;
	}

	public static void SetHatBobingPhysics()
	{
	}
}
