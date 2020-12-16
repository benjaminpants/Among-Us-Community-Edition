using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class CE_WardrobeLoader
{
	#region Common Loader Functions
	public static FileInfo[] GetJSONFiles(string Path)
	{
		return new DirectoryInfo(Path).GetFiles("*.json");
	}

	#endregion

	#region Custom Skin Loader

	public static CE_CustomSkinDefinition UpdateSkin(string fileName, CE_CustomSkinDefinition skinDefinition)
	{
		//For Updating Skins Only
		foreach (var entry in skinDefinition.FrameList)
		{
			var offset = GetPixelPivot(entry.Size.x, entry.Size.y, entry.Offset.x, entry.Offset.y);
			entry.Offset = new Point(offset.x, offset.y);
		}
		File.WriteAllText(fileName + ".new", JsonConvert.SerializeObject(skinDefinition, Formatting.Indented));
		return skinDefinition;
	}
	public static List<CE_CustomSkinDefinition> GetSkinDefinitions(FileInfo[] files)
	{
		List<CE_CustomSkinDefinition> DefinitionsList = new List<CE_CustomSkinDefinition>();
		for (int i = 0; i < files.Length; i++)
		{
			Debug.Log(files[i].FullName);
			using StreamReader reader = File.OpenText(files[i].FullName);
			try
			{
				CE_CustomSkinDefinition item = (CE_CustomSkinDefinition)new JsonSerializer().Deserialize(reader, typeof(CE_CustomSkinDefinition));
				DefinitionsList.Add(item);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}
		return DefinitionsList;
	}
	public static List<SkinData> GetSkinData(List<CE_CustomSkinDefinition> Definitions, string RootPath, SkinData BaseSkin)
	{
		List<SkinData> DataList = new List<SkinData>();
		int num = 8;
		foreach (CE_CustomSkinDefinition item2 in Definitions)
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
				foreach (CE_SpriteFrame frame in item2.FrameList)
				{
					frame.Texture = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(RootPath, frame.SpritePath));
					skinData.FrameList.Add(frame.Name, frame);
				}
				if (skinData.FrameList.ContainsKey("Display"))
				{
					CE_SpriteFrame customSkinFrame = skinData.FrameList["Display"];
					float x = customSkinFrame.Position.x;
					float y = customSkinFrame.Position.y;
					float width = customSkinFrame.Size.x;
					float height = customSkinFrame.Size.y;
					float offset_x = customSkinFrame.Offset.x;
					float offset_y = customSkinFrame.Offset.y;
					Texture2D texture = customSkinFrame.Texture;

					var offset = GetPrecentagePivot(width, height, new Vector2(offset_x, offset_y));
					var pivot = new Vector2(offset.x, offset_y);

					skinData.IdleFrame = Sprite.Create(texture, new Rect(x, y, width, height), pivot);
				}
				num++;
				DataList.Add(skinData);
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.Message);
			}
		}
		return DataList;
	}
	public static List<SkinData> LoadSkins(SkinData BaseSkin)
	{
		string Directory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Skins");
		FileInfo[] SkinFiles = GetJSONFiles(Directory);
		List<CE_CustomSkinDefinition> DefinitionsList = GetSkinDefinitions(SkinFiles);
		return GetSkinData(DefinitionsList, Directory, BaseSkin);
	}

	#endregion

	#region Custom Hat Loader

	public static List<CE_CustomHatDefinition> GetHatDefinitions(FileInfo[] files)
	{
		List<CE_CustomHatDefinition> DefinitionsList = new List<CE_CustomHatDefinition>();
		for (int i = 0; i < files.Length; i++)
		{
			Debug.Log(files[i].FullName);
			using StreamReader reader = File.OpenText(files[i].FullName);
			try
			{
				CE_CustomHatDefinition item = (CE_CustomHatDefinition)new JsonSerializer().Deserialize(reader, typeof(CE_CustomHatDefinition));
				DefinitionsList.Add(item);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}
		return DefinitionsList;
	}
	public static List<HatBehaviour> GetHatBehaviours(List<CE_CustomHatDefinition> Definitions, string RootPath)
	{
		List<HatBehaviour> BehaviorsList = new List<HatBehaviour>();
		foreach (CE_CustomHatDefinition item2 in Definitions)
		{
			try
			{
				HatBehaviour hatBehaviour = new HatBehaviour();
				hatBehaviour.InFront = item2.inFront;
				hatBehaviour.ProductId = item2.ID;
				hatBehaviour.IsCustom = true;
				hatBehaviour.NoBobbing = item2.NoBobbing;
				Texture2D texture2D = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(RootPath, item2.NormalImg));
				Texture2D texture2D2 = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(RootPath, item2.FloorImg));
				Texture2D texture2D3;
				try
                {
					 texture2D3 = CE_TextureNSpriteExtensions.LoadPNG(Path.Combine(RootPath, item2.PreviewImg));
				}
				catch
                {
					texture2D3 = null;
                }

				if (item2.UsePointFiltering)
				{
					texture2D.filterMode = FilterMode.Point;
					texture2D2.filterMode = FilterMode.Point;
					if (texture2D3 != null)
					{
						texture2D3.filterMode = FilterMode.Point;
					}
				}
				hatBehaviour.MainImage = Sprite.Create(texture2D, new Rect(new Vector2(item2.NormalPosX, item2.NormalPosY), new Vector2(item2.NormalWidth, item2.NormalHeight)), new Vector2(item2.NormalPivotX, item2.NormalPivotY));
				hatBehaviour.FloorImage = Sprite.Create(texture2D2, new Rect(new Vector2(item2.FloorPosX, item2.FloorPosY), new Vector2(item2.FloorWidth, item2.FloorHeight)), new Vector2(item2.FloorPivotX, item2.FloorPivotY));
				if (texture2D3 != null)
				{
					hatBehaviour.PreviewImage = Sprite.Create(texture2D3, new Rect(new Vector2(item2.PreviewPosX, item2.PreviewPosY), new Vector2(item2.PreviewWidth, item2.PreviewHeight)), new Vector2(item2.PreviewPivotX, item2.PreviewPivotY));
				}
				BehaviorsList.Add(hatBehaviour);
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.Message);
			}
		}
		return BehaviorsList;
	}
	public static List<HatBehaviour> LoadHats()
	{
		string Directory = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Hats");
		FileInfo[] HatFiles = GetJSONFiles(Directory);
		List<CE_CustomHatDefinition> DefinitionsList = GetHatDefinitions(HatFiles);
		return GetHatBehaviours(DefinitionsList, Directory);
	}

	#endregion

	#region Animation Debug Stuff

	public static void SetCurrentFramePivotX(float x)
    {
		SkinData skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(PlayerControl.LocalPlayer.Data.SkinId);
		int index = DestroyableSingleton<HatManager>.Instance.AllSkins.IndexOf(skin);
		if (skin.FrameList.ContainsKey(AnimationEditor_LastFrame))
		{
			float new_x = x;
			float new_y = skin.FrameList[AnimationEditor_LastFrame].Offset.y;
			skin.FrameList[AnimationEditor_LastFrame].Offset = new Point(new_x, new_y);
		}
		PlayerControl.LocalPlayer.SetSkin(PlayerControl.LocalPlayer.Data.SkinId);
	}

	public static void SetCurrentFramePivotY(float y)
	{
		SkinData skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(PlayerControl.LocalPlayer.Data.SkinId);
		int index = DestroyableSingleton<HatManager>.Instance.AllSkins.IndexOf(skin);
		if (skin.FrameList.ContainsKey(AnimationEditor_LastFrame))
		{
			float new_x = skin.FrameList[AnimationEditor_LastFrame].Offset.x;
			float new_y = y;
			skin.FrameList[AnimationEditor_LastFrame].Offset = new Point(new_x, new_y);
		}
		PlayerControl.LocalPlayer.SetSkin(PlayerControl.LocalPlayer.Data.SkinId);
	}

	public static void NudgeCurrentFramePivot(float x, float y)
	{
		SkinData skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(PlayerControl.LocalPlayer.Data.SkinId);
		int index = DestroyableSingleton<HatManager>.Instance.AllSkins.IndexOf(skin);
		if (skin.FrameList.ContainsKey(AnimationEditor_LastFrame))
		{
			float new_x = skin.FrameList[AnimationEditor_LastFrame].Offset.x + x;
			float new_y = skin.FrameList[AnimationEditor_LastFrame].Offset.y + y;
			skin.FrameList[AnimationEditor_LastFrame].Offset = new Point(new_x, new_y);
		}
		PlayerControl.LocalPlayer.SetSkin(PlayerControl.LocalPlayer.Data.SkinId);
	}

	public const int AnimationEditor_ModeMax = 5;
	public static float AnimationEditor_CurrentSpeed
    {
		get
        {
			return (AnimationEditor_Enabled ? AnimationEditor_Speed : 1f);
        }
    }
	public static float AnimationEditor_LastPivotX { get; set; } = 0;
	public static float AnimationEditor_LastPivotY { get; set; } = 0;
	public static bool AnimationEditor_Active { get; set; } = false;
	public static float AnimationEditor_Speed { get; set; } = 1;
	public static int AnimationEditor_Mode { get; set; } = 0;
	public static bool AnimationEditor_Paused
	{
		get
        {
			return (AnimationEditor_Enabled ? AnimationEditor_IsPaused : false);
		}
	}
	public static bool AnimationEditor_IsPaused { get; set; } = false;
	public static bool AnimationEditor_Reset { get; set; } = false;
	public static string AnimationEditor_PauseAt { get; set; } = string.Empty;
	public static bool AnimationEditor_NextFrame { get; set; } = false;
	public static bool AnimationEditor_Enabled
	{
		get
		{
			if (SaveManager.EnableAnimationTestingMode)
			{
				return AnimationEditor_Active;
			}
			return false;
		}
	}
	public static string AnimationEditor_LastFrame { get; set; }
	public static void LogPivot(Renderer renderer)
	{
		Sprite sprite = null;
		if (((SpriteRenderer)renderer) != null) sprite = ((SpriteRenderer)renderer).sprite;
		else if (renderer.GetComponent<SpriteRenderer>() != null) sprite = renderer.GetComponent<SpriteRenderer>().sprite;
		if (sprite != null)
		{
			var pivot = GetPixelOffsetFromCenter(sprite);
			string contents = string.Format("{2}: {0},{1}", pivot.x, pivot.y, sprite.name);
			Debug.Log(contents);
			System.IO.File.AppendAllText(System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Log.txt"), contents + "\r\n");
		}
	}

	#endregion

	#region Skin Rendering Methods

	public static Vector2 GetPixelOffsetFromCenter(float width, float height, float x, float y)
	{
		Vector2 pixel_pivot = new Vector2(x * width - (width / 2), y * height - (height / 2));
		return pixel_pivot;
	}
	public static Vector2 GetPixelOffsetFromCenter(Sprite sprite)
	{
		float x = -sprite.bounds.center.x / sprite.bounds.extents.x / 2 + 0.5f;
		float y = -sprite.bounds.center.y / sprite.bounds.extents.y / 2 + 0.5f;
		Vector2 pixel_pivot = new Vector2(x * sprite.textureRect.width - (sprite.textureRect.width / 2), y * sprite.textureRect.height - (sprite.textureRect.height / 2));
		return pixel_pivot;
	}
	public static Vector2 GetPixelPivot(float width, float height, float x, float y)
	{
		Vector2 pixel_pivot = new Vector2(x * width, y * height);
		return pixel_pivot;
	}
	public static Vector2 GetPixelPivot(Sprite sprite)
	{
		float x = -sprite.bounds.center.x / sprite.bounds.extents.x / 2 + 0.5f;
		float y = -sprite.bounds.center.y / sprite.bounds.extents.y / 2 + 0.5f;
		Vector2 pixel_pivot = new Vector2(x * sprite.textureRect.width, y * sprite.textureRect.height);
		return pixel_pivot;
	}
	public static Vector2 GetPrecentagePivot(float width, float height, Vector2 pixelCoords)
	{
		float x = pixelCoords.x / width;
		float y = pixelCoords.y / height;
		Vector2 frame_pivot = new Vector2(x, y);
		return frame_pivot;
	}
	public static Vector2 GetPrecentagePivot(Sprite sprite, Vector2 pixelCoords)
	{
		float x = pixelCoords.x / sprite.textureRect.width;
		float y = pixelCoords.y / sprite.textureRect.height;
		Vector2 frame_pivot = new Vector2(x, y);
		return frame_pivot;
	}
    public static Sprite GetSkin(string name, SkinData skin)
    {
		string key = name.Substring(name.IndexOf("_") + 1);
		if (!skin.FrameList.ContainsKey(key))
        {
			key = name.Substring(name.IndexOf("-") + 1);
		}

		if (skin.FrameList.ContainsKey(key))
        {
            CE_SpriteFrame customSkinFrame = skin.FrameList[key];
            float x = customSkinFrame.Position.x;
            float y = customSkinFrame.Position.y;
            float width = customSkinFrame.Size.x;
            float height = customSkinFrame.Size.y;
            float offset_x = customSkinFrame.Offset.x;
            float offset_y = customSkinFrame.Offset.y;
            Texture2D texture = customSkinFrame.Texture;

			AnimationEditor_LastPivotX = offset_x;
			AnimationEditor_LastPivotY = offset_y;

			bool NewFrame = AnimationEditor_LastFrame != key;
			if (AnimationEditor_Enabled && NewFrame)
			{
				if (key == AnimationEditor_PauseAt && AnimationEditor_PauseAt != string.Empty)
                {
					AnimationEditor_IsPaused = true;
				}
				if (AnimationEditor_NextFrame)
				{
					AnimationEditor_IsPaused = true;
					AnimationEditor_NextFrame = false;
				}
			}

			AnimationEditor_LastFrame = key;


			var pivot = GetPrecentagePivot(width, height, new Vector2(offset_x, offset_y));
            return Sprite.Create(texture, new Rect(x, y, width, height), pivot);

		}
        else return null;
    }

	public static Vector3 SetHatBobingPhysics(string name, Vector3 position)
	{
		float num = 0.65f;
		if (name == "walkcolor0001") num += 0.019f;
		if (name == "walkcolor0002") num += 0.05f;
		if (name == "walkcolor0003") num += 0.02f;
		if (name == "walkcolor0004") num += -0.04f;
		if (name == "walkcolor0005") num += -0.09f;
		if (name == "walkcolor0006") num += -0.09f;
		if (name == "walkcolor0007") num += 0.059f;
		if (name == "walkcolor0008") num += 0.089f;
		if (name == "walkcolor0009") num += 0.06f;
		if (name == "walkcolor0010") num += 0f;
		if (name == "walkcolor0011") num += -0.12f;
		if (name == "walkcolor0012") num += -0.129f;
		float x = position.x;
		float z = position.z;
		position = new Vector3(x, num, z);
		return position;
	}

	#endregion

	static CE_WardrobeLoader()
	{

	}

}
