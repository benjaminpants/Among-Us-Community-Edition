using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using System.Linq;
using UnityEngine.Purchasing;

public class CE_WardrobeManager
{
	#region Common Loader Functions

	public static Dictionary<string, Texture2D> LoadedTextures = new Dictionary<string, Texture2D>();
	public static FileInfo[] GetJSONFiles(string Path)
	{
		return new DirectoryInfo(Path).GetFiles("*.json");
	}
	public static SkinData GetSkinRefrence()
    {
		return DestroyableSingleton<HatManager>.Instance.AllSkins.Where((SkinData x) => x.name == "Police").FirstOrDefault();
	}
	public static void LinkSkinsAndHats()
    {
		
		foreach (var hat in DestroyableSingleton<HatManager>.Instance.AllHats)
        {
			foreach (var skin in DestroyableSingleton<HatManager>.Instance.AllSkins)
			{
				if (hat.IsCustom && skin.isCustom)
                {
					bool validHatPair = !string.IsNullOrEmpty(skin.RelatedHatName) && !string.IsNullOrEmpty(hat.StoreName);
					bool validSkinPair = !string.IsNullOrEmpty(hat.RelatedSkinName) && !string.IsNullOrEmpty(skin.StoreName);

					if (skin.RelatedHatName == hat.StoreName && validHatPair)
					{
						skin.RelatedHat = hat;
					}
					if (hat.RelatedSkinName == skin.StoreName && validSkinPair)
					{
						hat.RelatedSkin = skin;
					}
				}
			}
		}
	}
	public static void Reload()
    {
		uint hatId = PlayerControl.LocalPlayer.Data.HatId;
		uint skinId = PlayerControl.LocalPlayer.Data.SkinId;
		foreach (var loadedTexture in LoadedTextures)
		{
			UnityEngine.Object.Destroy(loadedTexture.Value);
		}
		LoadedTextures.Clear();
		DestroyableSingleton<HatManager>.Instance.AllHats.RemoveAll((HatBehaviour x) => x.IsCustom);
		DestroyableSingleton<HatManager>.Instance.AllSkins.RemoveAll((SkinData x) => x.isCustom);

		DestroyableSingleton<HatManager>.Instance.AllHats.AddRange(LoadHats());
		DestroyableSingleton<HatManager>.Instance.AllSkins.AddRange(LoadSkins(GetSkinRefrence()));
		PlayerControl.LocalPlayer.SetSkin(skinId);
		PlayerControl.LocalPlayer.SetHat(hatId);
		CE_WardrobeManager.AnimationEditor_Reset = true;
	}

	#endregion

	#region Custom Skin Loader

	public static List<CE_FrameSet> GetSkinDefinitions(FileInfo[] files)
	{
		List<CE_FrameSet> DefinitionsList = new List<CE_FrameSet>();
		for (int i = 0; i < files.Length; i++)
		{
			Debug.Log(files[i].FullName);
			using StreamReader reader = File.OpenText(files[i].FullName);
			try
			{
				CE_FrameSet item = (CE_FrameSet)new JsonSerializer().Deserialize(reader, typeof(CE_FrameSet));
				item.FilePath = files[i].FullName;
				DefinitionsList.Add(item);
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
			reader.Close();
		}
		return DefinitionsList;
	}
	public static List<SkinData> GetSkinData(List<CE_FrameSet> Definitions, string RootPath, SkinData BaseSkin)
	{
		List<SkinData> DataList = new List<SkinData>();
		int num = 8;
		foreach (CE_FrameSet item2 in Definitions)
		{
			try
			{
				SkinData skinData = ScriptableObject.CreateInstance<SkinData>();
				skinData.name = item2.Name;
				skinData.isCustom = true;
				skinData.StoreName = item2.StoreName;
				skinData.Free = true;
				skinData.ID = item2.ID;
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
				skinData.RelatedHatName = item2.RelatedHat;
				skinData.IsHidden = item2.IsHidden;
				foreach (CE_SpriteFrame frame in item2.FrameList)
				{
					frame.Texture = CE_TextureNSpriteExtensions.LoadPNG_HatManager(Path.Combine(RootPath, frame.SpritePath));
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
					var pivot = new Vector2(offset.x, offset.y);

					skinData.IdleFrame = Sprite.Create(texture, GetSpriteRect(texture, x, y, width, height), GetSpritePivot(pivot));
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
		string Directory = System.IO.Path.Combine(CE_Extensions.GetGameDirectory(), "Skins");
		FileInfo[] SkinFiles = GetJSONFiles(Directory);
		List<CE_FrameSet> DefinitionsList = GetSkinDefinitions(SkinFiles);
		return GetSkinData(DefinitionsList, Directory, BaseSkin);
	}

	#endregion

	#region Custom Hat Loader
	public static List<CE_FrameSet> GetHatDefinitions(FileInfo[] files)
	{
		List<CE_FrameSet> DefinitionsList = new List<CE_FrameSet>();
		for (int i = 0; i < files.Length; i++)
		{
            Debug.Log(files[i].FullName);
			string json = File.ReadAllText(files[i].FullName);
			try
			{
				CE_FrameSet Result = (CE_FrameSet)new JsonSerializer().Deserialize(new JsonTextReader(new StringReader(json)), typeof(CE_FrameSet));
				if (Result != null)
				{
					DefinitionsList.Add(Result);
				}
				else throw new Exception("\"" + files[i].FullName + "\"" + " is Not a Valid Hat");
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}

		}
		return DefinitionsList;
	}
	public static List<HatBehaviour> GetHatBehaviours(List<CE_FrameSet> Definitions, string RootPath)
	{
		List<HatBehaviour> BehaviorsList = new List<HatBehaviour>();
		foreach (CE_FrameSet item2 in Definitions)
		{
			try
			{
				HatBehaviour hatBehaviour = ScriptableObject.CreateInstance<HatBehaviour>();
				hatBehaviour.ProductId = item2.ID;
				hatBehaviour.StoreName = item2.Name;
				hatBehaviour.RelatedSkinName = item2.RelatedSkin;
				hatBehaviour.IsCustom = true;
				hatBehaviour.IsHidden = item2.IsHidden;
				CE_LoadHats_SetHatNoBobbingState(ref hatBehaviour, item2);
				CE_LoadHats_SetHatInFrontState(ref hatBehaviour, item2);
				CE_LoadHats_SetColorFilterState(ref hatBehaviour, item2);


				foreach (CE_SpriteFrame frame in item2.FrameList)
				{
					frame.Texture = CE_TextureNSpriteExtensions.LoadPNG_HatManager(Path.Combine(RootPath, frame.SpritePath));
					float x = frame.Position.x;
					float y = frame.Position.y;
					float width = frame.Size.x;
					float height = frame.Size.y;
					float offset_x = frame.Offset.x;
					float offset_y = frame.Offset.y;

					Vector2 pivot;
					if (!item2.UsePercentageBasedPivot)
					{
						Vector2 offset = GetPrecentagePivot(width, height, new Vector2(offset_x, offset_y));
						pivot = GetSpritePivot(new Vector2(offset.x, offset.y));
					}
					else
					{
						pivot = new Vector2(offset_x, offset_y);
					}



					if (frame.UsePointFiltering || item2.UsePointFilteringGlobally)
					{
						frame.Texture.filterMode = FilterMode.Point;
					}

					Debug.Log("Creating Sprites for:" + item2.ID);

					switch (frame.Name)
					{
						case "Normal":
							hatBehaviour.MainImage = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							if (item2.HatInFront == false) CE_LoadHats_SetHatInFrontState(ref hatBehaviour, frame, 0);
							if (item2.NoHatBobbing == false) CE_LoadHats_SetHatNoBobbingState(ref hatBehaviour, frame, 0);
							if (item2.UseColorFilteringGlobally == false) CE_LoadHats_SetColorFilterState(ref hatBehaviour, frame, 0);
							break;
						case "Floor":
							hatBehaviour.FloorImage = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "Preview":
							hatBehaviour.PreviewImage = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "NormalExt":
							hatBehaviour.MainImageExt = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							if (item2.HatInFront == false) CE_LoadHats_SetHatInFrontState(ref hatBehaviour, frame, 1);
							if (item2.NoHatBobbing == false) CE_LoadHats_SetHatNoBobbingState(ref hatBehaviour, frame, 1);
							if (item2.UseColorFilteringGlobally == false) CE_LoadHats_SetColorFilterState(ref hatBehaviour, frame, 1);
							break;
						case "FloorExt":
							hatBehaviour.FloorImageExt = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "PreviewExt":
							hatBehaviour.PreviewImageExt = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "NormalExt2":
							hatBehaviour.MainImageExt2 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							if (item2.HatInFront == false) CE_LoadHats_SetHatInFrontState(ref hatBehaviour, frame, 2);
							if (item2.NoHatBobbing == false) CE_LoadHats_SetHatNoBobbingState(ref hatBehaviour, frame, 2);
							if (item2.UseColorFilteringGlobally == false) CE_LoadHats_SetColorFilterState(ref hatBehaviour, frame, 2);
							break;
						case "FloorExt2":
							hatBehaviour.FloorImageExt2 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "PreviewExt2":
							hatBehaviour.PreviewImageExt2 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "NormalExt3":
							hatBehaviour.MainImageExt3 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							if (item2.HatInFront == false) CE_LoadHats_SetHatInFrontState(ref hatBehaviour, frame, 3);
							if (item2.NoHatBobbing == false) CE_LoadHats_SetHatNoBobbingState(ref hatBehaviour, frame, 3);
							if (item2.UseColorFilteringGlobally == false) CE_LoadHats_SetColorFilterState(ref hatBehaviour, frame, 3);
							break;
						case "FloorExt3":
							hatBehaviour.FloorImageExt3 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "PreviewExt3":
							hatBehaviour.PreviewImageExt3 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "NormalExt4":
							hatBehaviour.MainImageExt4 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							if (item2.HatInFront == false) CE_LoadHats_SetHatInFrontState(ref hatBehaviour, frame, 4);
							if (item2.NoHatBobbing == false) CE_LoadHats_SetHatNoBobbingState(ref hatBehaviour, frame, 4);
							if (item2.UseColorFilteringGlobally == false) CE_LoadHats_SetColorFilterState(ref hatBehaviour, frame, 4);
							break;
						case "FloorExt4":
							hatBehaviour.FloorImageExt4 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
						case "PreviewExt4":
							hatBehaviour.PreviewImageExt4 = Sprite.Create(frame.Texture, GetSpriteRect(frame.Texture, x, y, width, height), pivot);
							break;
					}
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
		string Directory = System.IO.Path.Combine(CE_Extensions.GetGameDirectory(), "Hats");
		FileInfo[] HatFiles = GetJSONFiles(Directory);
		List<CE_FrameSet> DefinitionsList = GetHatDefinitions(HatFiles);
		return GetHatBehaviours(DefinitionsList, Directory);
	}

	public static void CE_LoadHats_SetColorFilterState(ref HatBehaviour hatBehaviour, CE_SpriteFrame item2, int index)
	{
		switch (index)
		{
			case 0:
				hatBehaviour.IsColorFiltered = item2.UseColorFiltering;
				break;
			case 1:
				hatBehaviour.IsColorFilteredExt = item2.UseColorFiltering;
				break;
			case 2:
				hatBehaviour.IsColorFilteredExt2 = item2.UseColorFiltering;
				break;
			case 3:
				hatBehaviour.IsColorFilteredExt3 = item2.UseColorFiltering;
				break;
			case 4:
				hatBehaviour.IsColorFilteredExt4 = item2.UseColorFiltering;
				break;
		}
	}
	public static void CE_LoadHats_SetColorFilterState(ref HatBehaviour hatBehaviour, CE_FrameSet item2)
	{
		hatBehaviour.IsColorFiltered = item2.UseColorFilteringGlobally;
		hatBehaviour.IsColorFilteredExt = item2.UseColorFilteringGlobally;
		hatBehaviour.IsColorFilteredExt2 = item2.UseColorFilteringGlobally;
		hatBehaviour.IsColorFilteredExt3 = item2.UseColorFilteringGlobally;
		hatBehaviour.IsColorFilteredExt4 = item2.UseColorFilteringGlobally;
	}
	public static void CE_LoadHats_SetHatNoBobbingState(ref HatBehaviour hatBehaviour, CE_SpriteFrame item2, int index)
	{
		switch (index)
		{
			case 0:
				hatBehaviour.NoBobbing = item2.Hat_NoBobbing;
				break;
			case 1:
				hatBehaviour.NoBobbingExt = item2.Hat_NoBobbing;
				break;
			case 2:
				hatBehaviour.NoBobbingExt2 = item2.Hat_NoBobbing;
				break;
			case 3:
				hatBehaviour.NoBobbingExt3 = item2.Hat_NoBobbing;
				break;
			case 4:
				hatBehaviour.NoBobbingExt4 = item2.Hat_NoBobbing;
				break;
		}
	}
	public static void CE_LoadHats_SetHatNoBobbingState(ref HatBehaviour hatBehaviour, CE_FrameSet item2)
	{
		hatBehaviour.NoBobbing = item2.NoHatBobbing;
		hatBehaviour.NoBobbingExt = item2.NoHatBobbing;
		hatBehaviour.NoBobbingExt2 = item2.NoHatBobbing;
		hatBehaviour.NoBobbingExt3 = item2.NoHatBobbing;
		hatBehaviour.NoBobbingExt4 = item2.NoHatBobbing;
	}
	public static void CE_LoadHats_SetHatInFrontState(ref HatBehaviour hatBehaviour, CE_SpriteFrame item2, int index)
	{
		switch (index)
		{
			case 0:
				hatBehaviour.InFront = item2.InFront;
				break;
			case 1:
				hatBehaviour.InFrontExt = item2.InFront;
				break;
			case 2:
				hatBehaviour.InFrontExt2 = item2.InFront;
				break;
			case 3:
				hatBehaviour.InFrontExt3 = item2.InFront;
				break;
			case 4:
				hatBehaviour.InFrontExt4 = item2.InFront;
				break;
		}
	}
	public static void CE_LoadHats_SetHatInFrontState(ref HatBehaviour hatBehaviour, CE_FrameSet item2)
	{
		hatBehaviour.InFront = item2.HatInFront;
		hatBehaviour.InFrontExt = item2.HatInFront;
		hatBehaviour.InFrontExt2 = item2.HatInFront;
		hatBehaviour.InFrontExt3 = item2.HatInFront;
		hatBehaviour.InFrontExt4 = item2.HatInFront;
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
			skin.FrameList[AnimationEditor_LastFrame].Offset = new CE_Point(new_x, new_y);
			skin.FrameList[AnimationEditor_LastFrame].Sprite = null;
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
			skin.FrameList[AnimationEditor_LastFrame].Offset = new CE_Point(new_x, new_y);
			skin.FrameList[AnimationEditor_LastFrame].Sprite = null;
		}
		PlayerControl.LocalPlayer.SetSkin(PlayerControl.LocalPlayer.Data.SkinId);
	}
	public static void SaveCurrentSkin()
    {
		SkinData skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(PlayerControl.LocalPlayer.Data.SkinId);
		if (skin.isCustom)
		{ 		
			CE_FrameSet skinToSave = new CE_FrameSet();
			skinToSave.FrameList = skin.FrameList.Select(frame => frame.Value).ToList();
			File.WriteAllText(skinToSave.FilePath, JsonConvert.SerializeObject(skinToSave, Formatting.Indented));
		}

	}
	public static void NudgeCurrentFramePivot(float x, float y)
	{
		SkinData skin = DestroyableSingleton<HatManager>.Instance.GetSkinById(PlayerControl.LocalPlayer.Data.SkinId);
		int index = DestroyableSingleton<HatManager>.Instance.AllSkins.IndexOf(skin);
		if (skin.FrameList.ContainsKey(AnimationEditor_LastFrame))
		{
			float new_x = skin.FrameList[AnimationEditor_LastFrame].Offset.x + x;
			float new_y = skin.FrameList[AnimationEditor_LastFrame].Offset.y + y;
			skin.FrameList[AnimationEditor_LastFrame].Offset = new CE_Point(new_x, new_y);
			skin.FrameList[AnimationEditor_LastFrame].Sprite = null;
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

	#region Skin Calculating Methods

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
	public static Vector2 GetSpritePivot(Vector2 input)
	{
		return new Vector2(input.x, input.y);
	}
	public static Rect GetSpriteRect(Texture texture, Sprite sprite)
	{
		float real_y = sprite.rect.y + texture.height;
		float _finalX = sprite.rect.x;
		float _finalY = real_y + sprite.rect.height;
		float _finalWidth = sprite.rect.width;
		float _finalHeight = sprite.rect.height;

		return new Rect(_finalX, _finalY, _finalWidth, _finalHeight);
	}
	public static Rect GetSpriteRect(Texture texture, float x, float y, float width, float height)
	{
		float real_y = texture.height - y;
		float _finalX = x;
		float _finalY = real_y - height;
		float _finalWidth = width;
		float _finalHeight = height;

		return new Rect(_finalX, _finalY, _finalWidth, _finalHeight);
	}

	#endregion

	#region Skin/Hat Rendering Methods

	private static Material HatShaderMaterial;

	private static Material HatShaderlessMaterial;

	public static CE_SpriteFrame ConvertToFrame(string name, Sprite sprite)
    {
		CE_SpriteFrame frame = new CE_SpriteFrame();
		frame.Name = name;
		frame.Texture = sprite.texture;
		frame.Sprite = sprite;
		var rect = GetSpriteRect(sprite.texture, sprite);
		frame.Size = new CE_Point(rect.width, rect.height);
		frame.Position = new CE_Point(rect.x, rect.y);
		var pivot = GetPixelPivot(sprite);
		frame.Offset = new CE_Point(pivot.x, pivot.y);
		return frame;
    }
	public static Sprite GetSkin(string name, SkinData skin)
	{
		bool AllowNormalSkin = false;
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
			if (skin.FrameList[key].Sprite == null) skin.FrameList[key].Sprite = Sprite.Create(texture, GetSpriteRect(texture, x, y, width, height), GetSpritePivot(pivot));
			return skin.FrameList[key].Sprite;
		}
		else
        {
			/*if (!skin.isCustom && AllowNormalSkin)
            {
				if (key == "Main")
				{
					var frame = ConvertToFrame(key, skin.IdleFrame);
					skin.FrameList.Add(key, frame);
					return skin.FrameList[key].Sprite;
				}
				else
				{
					var frame = ConvertToFrame(key, sprite);
					skin.FrameList.Add(key, frame);
					return skin.FrameList[key].Sprite;
				}
			}*/
			return null;
		}
	}
	public static void UpdateActiveHatRender(PlayerControl player, string name, ref SpriteRenderer spriteRenderer, int hatSlot)
    {
		if (player == null || spriteRenderer == null) return;
		if (!DestroyableSingleton<HatManager>.InstanceExists) return;

		var hat = DestroyableSingleton<HatManager>.Instance.GetHatById(player.Data.HatId);
		if (hat == null) return;
		bool noBobbing;
		switch (hatSlot)
		{
			case 0:
				noBobbing = hat.NoBobbing;
				break;
			case 1:
				noBobbing = hat.NoBobbingExt;
				break;
			case 2:
				noBobbing = hat.NoBobbingExt2;
				break;
			case 3:
				noBobbing = hat.NoBobbingExt3;
				break;
			case 4:
				noBobbing = hat.NoBobbingExt4;
				break;
			default:
				noBobbing = hat.NoBobbing;
				break;
		}

		if (noBobbing)
		{
			var position = spriteRenderer.transform.localPosition;

			float x = position.x;
			float y = position.y;
			float z = position.z;

			if (player.MyPhysics.IsWalking())
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
				spriteRenderer.transform.localPosition = new Vector3(x, num, z);
			}
			else
			{
				spriteRenderer.transform.localPosition = new Vector3(x, y, z);
			}
		}
	}
	public static void MatchBaseHatRender(SpriteRenderer ExtraHatRenderer, SpriteRenderer HatRenderer)
	{
		ExtraHatRenderer.flipX = HatRenderer.flipX;
		ExtraHatRenderer.flipY = HatRenderer.flipY;
		ExtraHatRenderer.transform.parent = HatRenderer.transform.parent;
		ExtraHatRenderer.transform.localRotation = HatRenderer.transform.localRotation;
		ExtraHatRenderer.transform.localScale = HatRenderer.transform.localScale;

		var LocalPos = ExtraHatRenderer.transform.localPosition;
		LocalPos.x = HatRenderer.transform.localPosition.x;
		LocalPos.y = HatRenderer.transform.localPosition.y;
		ExtraHatRenderer.transform.localPosition = LocalPos;

		ExtraHatRenderer.realtimeLightmapIndex = HatRenderer.realtimeLightmapIndex;
		ExtraHatRenderer.enabled = HatRenderer.enabled;
		ExtraHatRenderer.color = HatRenderer.color;
		ExtraHatRenderer.sortingLayerID = HatRenderer.sortingLayerID;
		ExtraHatRenderer.drawMode = HatRenderer.drawMode;
		ExtraHatRenderer.maskInteraction = HatRenderer.maskInteraction;
		ExtraHatRenderer.renderingLayerMask = HatRenderer.renderingLayerMask;
	}
	public static void MatchSpriteRenderer(SpriteRenderer Target, SpriteRenderer Source)
	{
		Target.flipX = Source.flipX;
		Target.flipY = Source.flipY;
		Target.transform.parent = Source.transform.parent;
		Target.transform.localRotation = Source.transform.localRotation;
		Target.transform.localScale = Source.transform.localScale;
		Target.transform.localPosition = Source.transform.localPosition;
		Target.sortingLayerID = Source.sortingLayerID;
		Target.drawMode = Source.drawMode;
		Target.maskInteraction = Source.maskInteraction;
		Target.renderingLayerMask = Source.renderingLayerMask;
	}
	public static SpriteRenderer CreateExtHatCutscenes(SpriteRenderer _ref, int index)
	{
		string realIndex = string.Format("{0}", index + 1);
		GameObject gameObject = new GameObject("ExtHatSlot" + realIndex);
		gameObject.layer = _ref.gameObject.layer;
		SpriteRenderer HatRendererExt = gameObject.AddComponent<SpriteRenderer>();
		HatRendererExt.transform.SetParent(_ref.transform);
		return HatRendererExt;
	}
	public static SpriteRenderer CreateExtHat(PlayerControl playerControl)
	{	
		GameObject gameObject = new GameObject("ExtHatSlot");
		SpriteRenderer HatRendererExt = gameObject.AddComponent<SpriteRenderer>();
		HatRendererExt.transform.SetParent(playerControl.transform);
		return HatRendererExt;
	}
	public static SpriteRenderer CreateExtSpriteRender(SpriteRenderer _ref, string name = "ExtHatSlot", bool inFront = false)
    {
		GameObject gameObject = new GameObject(name);
		gameObject.layer = _ref.gameObject.layer + (inFront ? 1 : 0);
		SpriteRenderer HatRendererExt = gameObject.AddComponent<SpriteRenderer>();
		HatRendererExt.transform.SetParent(_ref.transform);
		return HatRendererExt;
	}
	public static void SetExtHatImage(HatBehaviour hat, SpriteRenderer target, int hatSlot, int playerColor, bool PreviewMode = false)
	{
		if ((bool)target && (bool)hat)
		{
			Sprite MainImage;
			bool InFront;
			bool Colored;
			switch (hatSlot)
			{
				case 0:
					if (PreviewMode) MainImage = (hat.PreviewImage ? hat.PreviewImage : hat.MainImage);
					else MainImage = hat.MainImage;
					InFront = hat.InFront;
					Colored = hat.IsColorFiltered;
					break;
				case 1:
					if (PreviewMode) MainImage = (hat.PreviewImageExt ? hat.PreviewImageExt : hat.MainImageExt);
					else MainImage = hat.MainImageExt;
					InFront = hat.InFrontExt;
					Colored = hat.IsColorFilteredExt;
					break;
				case 2:
					if (PreviewMode) MainImage = (hat.PreviewImageExt2 ? hat.PreviewImageExt2 : hat.MainImageExt2);
					else MainImage = hat.MainImageExt2;
					InFront = hat.InFrontExt2;
					Colored = hat.IsColorFilteredExt2;
					break;
				case 3:
					if (PreviewMode) MainImage = (hat.PreviewImageExt3 ? hat.PreviewImageExt3 : hat.MainImageExt3);
					else MainImage = hat.MainImageExt3;
					InFront = hat.InFrontExt3;
					Colored = hat.IsColorFilteredExt3;
					break;
				case 4:
					if (PreviewMode) MainImage = (hat.PreviewImageExt4 ? hat.PreviewImageExt4 : hat.MainImageExt4);
					else MainImage = hat.MainImageExt4;
					InFront = hat.InFrontExt4;
					Colored = hat.IsColorFilteredExt4;
					break;
				default:
					if (PreviewMode) MainImage = (hat.PreviewImage ? hat.PreviewImage : hat.MainImage);
					else MainImage = hat.MainImage;
					InFront = hat.InFront;
					Colored = hat.IsColorFiltered;
					break;
			}

			
			target.sprite = MainImage;
			if (MainImage)
            {
				Vector3 localPosition = target.transform.localPosition;
				localPosition.z = (InFront ? (-0.0001f) : 0.0001f);
				target.transform.localPosition = localPosition;
				SetHatRenderColors(target, (int)playerColor, Colored);
			}
		}
		else
		{
			string str = ((!target) ? "null" : target.name);
			string str2 = ((!hat) ? "null" : hat.name);
			Debug.LogError("Player: " + str + "\tHat: " + str2);
		}
	}
	public static void SetHatRenderColors(SpriteRenderer spriteRenderer, int colorId, bool useShader)
    {
		string obj_Name = "PlayerMaterial";
		if (!HatShaderMaterial)
		{
			var resources = UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Material));
			if (resources != null)
			{
				foreach (var item in resources)
				{
					if (item.name == obj_Name)
					{
                        HatShaderMaterial = UnityEngine.Object.Instantiate(item as Material);
						HatShaderlessMaterial = UnityEngine.Object.Instantiate(spriteRenderer.material);
					}
				}
			}
		}
		if (HatShaderMaterial)
		{
			if (useShader)
            {
				spriteRenderer.material = HatShaderMaterial;
				PlayerControl.SetPlayerMaterialColors(colorId, spriteRenderer);
			}
			else
            {
				spriteRenderer.material = HatShaderlessMaterial;
			}

		}
	}

	public static void CE_SetHatAlpha(ref SpriteRenderer HatRenderer, float a)
    {
		Color white = Color.white;
		white.a = a;
		HatRenderer.color = white;

		var _Color = HatRenderer.material.color;
		_Color = new Color(_Color.r, _Color.g, _Color.b, a);
		HatRenderer.material.color = _Color;
	}

	#endregion

	static CE_WardrobeManager()
	{

	}

}
