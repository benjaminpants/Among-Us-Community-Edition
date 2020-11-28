using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;
using UnityEditor;

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

	//public static SkinData CreateTestSkinData()	
    //{
		/*
		var myLoadedAssetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.dataPath, "sharedassets0"));

		if (myLoadedAssetBundle == null)
		{
			Debug.Log("Failed to load AssetBundle!");
			return null;
		}

		var IdleSprite = myLoadedAssetBundle.LoadAsset<Sprite>("military_stand");
		var SpawnClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("MilitarySpawn");
        var RunClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("MilitaryRun");
        var IdleClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("MilitaryIdle");
        var ExitVentClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("MilitaryExitVent");
		var EnterVentClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("MilitaryEnterVent");

		var EjectSprite = myLoadedAssetBundle.LoadAsset<Sprite>("military_ejected");
		var TongueVictimClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("mili_tongue_victim");
		var TongueImpostorClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("mili_tongue_impostor");
		var StabVictimClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("mili_stab_victim");
		var NeckVictimClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("mili_neck_victim");
		var GunVictimClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("mili_gun_victim");
		var GunImpostorClip = myLoadedAssetBundle.LoadAsset<AnimationClip>("mili_gun_impostor");

		myLoadedAssetBundle.Unload(false);
		*/

		/*
		string dir = "Animations/";

		var IdleSprite = Resources.Load<Sprite>(dir + "IdleSprite.dat");
		var SpawnClip = Resources.Load<AnimationClip>(dir + "SpawnClip.dat");
		var RunClip = Resources.Load<AnimationClip>(dir + "RunClip.dat");
		var IdleClip = Resources.Load<AnimationClip>(dir + "IdleClip.dat");
		var ExitVentClip = Resources.Load<AnimationClip>(dir + "ExitVentClip.dat");
		var EnterVentClip = Resources.Load<AnimationClip>(dir + "EnterVentClip.dat");

		var EjectSprite = Resources.Load<Sprite>(dir + "EjectSprite.dat");
		var TongueVictimClip = Resources.Load<AnimationClip>(dir + "TongueVictimClip.dat");
		var TongueImpostorClip = Resources.Load<AnimationClip>(dir + "TongueImpostorClip.dat");
		var StabVictimClip = Resources.Load<AnimationClip>(dir + "StabVictimClip.dat");
		var NeckVictimClip = Resources.Load<AnimationClip>(dir + "NeckVictimClip.dat");
		var GunVictimClip = Resources.Load<AnimationClip>(dir + "GunVictimClip.dat");
		var GunImpostorClip = Resources.Load<AnimationClip>(dir + "GunImpostorClip.dat");

		if (IdleSprite == null)
		{
			Debug.Log("Failed to load TestFiles!");
			return null;
		}

		SkinData TestSkin = new SkinData();		
		TestSkin.IdleFrame = IdleSprite;
		TestSkin.IdleAnim = IdleClip;
		TestSkin.RunAnim = RunClip;
		TestSkin.EnterVentAnim = EnterVentClip;
		TestSkin.ExitVentAnim = ExitVentClip;
		TestSkin.KillTongueImpostor = TongueImpostorClip;
		TestSkin.KillTongueVictim = TongueVictimClip;
		TestSkin.KillShootImpostor = GunImpostorClip;
		TestSkin.KillShootVictim = GunVictimClip;
		TestSkin.KillStabVictim = StabVictimClip;
		TestSkin.KillNeckVictim = NeckVictimClip;
		TestSkin.EjectFrame = EjectSprite;
		TestSkin.SpawnAnim = SpawnClip;
		TestSkin.Free = true;
		TestSkin.RelatedHat = null;
		TestSkin.StoreName = "TestSuit";
		TestSkin.Order = 0;
		return TestSkin;
		*/
//}

	public static List<SkinData> LoadSkins()
	{
		List<SkinData> list = new List<SkinData>();
		//list.Add(CreateTestSkinData());
		return list;
	}
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
