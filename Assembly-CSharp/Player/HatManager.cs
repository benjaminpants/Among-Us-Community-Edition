using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HatManager : DestroyableSingleton<HatManager>
{
	public HatBehaviour NoneHat;

	public List<HatBehaviour> AllHats = new List<HatBehaviour>();

	public List<SkinData> AllSkins = new List<SkinData>();

	public HatBehaviour GetHatById(uint hatId)
	{
		if ((ulong)hatId >= (ulong)AllHats.Count)
		{
			return NoneHat;
		}
		return AllHats[(int)hatId];
	}

	public HatBehaviour[] GetUnlockedHats()
	{
		return (from o in AllHats 
			where !o.IsHidden
			orderby o.LimitedMonth ascending
			orderby !o.IsCustom descending, 
			o.name orderby o.Order descending
				select o).ToArray();
	}

	public uint GetIdFromHat(HatBehaviour hat)
	{
		return (uint)AllHats.IndexOf(hat);
	}

	public SkinData[] GetUnlockedSkins()
	{
		return (from o in AllSkins
			where !o.IsHidden
			orderby !o.isCustom descending,
			o.name orderby o.Order descending
			select o).ToArray();
	}

	public uint GetIdFromSkin(SkinData skin)
	{
		return (uint)AllSkins.IndexOf(skin);
	}

	internal SkinData GetSkinById(uint skinId)
	{
		if ((ulong)skinId >= (ulong)AllSkins.Count)
		{
			return AllSkins[0];
		}
		return AllSkins[(int)skinId];
	}

	internal void SetSkin(SpriteRenderer skinRend, uint skinId, int colorid)
	{
		SkinData skinById = GetSkinById(skinId);
		if ((bool)skinById)
		{
			skinRend.sprite = skinById.IdleFrame;
			CE_WardrobeManager.SetHatRenderColors(skinRend, colorid, skinById.IsPlayerOverride);
		}
	}

    public void AddHats(string path)
    {
        AllHats.AddRange(CE_WardrobeManager.LoadHats(path));
		CE_WardrobeManager.LinkSkinsAndHats();
	}
	public void AddSkins(string path)
	{
        AllSkins.AddRange(CE_WardrobeManager.LoadSkins(CE_WardrobeManager.GetSkinRefrence(), path));
		CE_WardrobeManager.LinkSkinsAndHats();
	}

	public void Start()
	{
		//AllHats.AddRange(CE_WardrobeManager.LoadHats());
        //AllSkins.AddRange(CE_WardrobeManager.LoadSkins(CE_WardrobeManager.GetSkinRefrence()));
		CE_WardrobeManager.HatHash = VersionShower.GetDeterministicHashCode(CE_WardrobeManager.HatString);
		CE_WardrobeManager.HatString = "bob";
	}

	public void ReloadCustomHatsAndSkins()
	{
		Debug.Log("Reloading has been temporarily disabled!");
		//CE_WardrobeManager.Reload();
	}
}
