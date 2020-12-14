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
		return (from h in AllHats
			where h.LimitedMonth == 0 || SaveManager.GetPurchase(h.ProductId)
			select h into o
			orderby o.Order descending, o.name
			select o).ToArray();
	}

	public uint GetIdFromHat(HatBehaviour hat)
	{
		return (uint)AllHats.IndexOf(hat);
	}

	public SkinData[] GetUnlockedSkins()
	{
		return (from o in AllSkins
			orderby o.Order descending, o.name
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

	internal void SetSkin(SpriteRenderer skinRend, uint skinId)
	{
		SkinData skinById = GetSkinById(skinId);
		if ((bool)skinById)
		{
			skinRend.sprite = skinById.IdleFrame;
		}
	}

	public void Start()
	{
		AllHats.AddRange(CE_WardrobeLoader.LoadHats());
		AllSkins.AddRange(CE_WardrobeLoader.LoadSkins(GetCustomRefrence()));
	}

	public void ReloadCustomHatsAndSkins()
	{
		uint hatId = PlayerControl.LocalPlayer.Data.HatId;
		uint skinId = PlayerControl.LocalPlayer.Data.SkinId;
		AllHats.RemoveAll((HatBehaviour x) => x.IsCustom);
		AllSkins.RemoveAll((SkinData x) => x.isCustom);
		AllHats.AddRange(CE_WardrobeLoader.LoadHats());
		AllSkins.AddRange(CE_WardrobeLoader.LoadSkins(GetCustomRefrence()));
		PlayerControl.LocalPlayer.SetSkin(skinId);
		PlayerControl.LocalPlayer.SetHat(hatId);
		_ = CE_WardrobeLoader.TestPlaybackMode;
		CE_WardrobeLoader.TestPlaybackResetAnimations = true;
	}

	private SkinData GetCustomRefrence()
	{
		return AllSkins.Where((SkinData x) => x.name == "Police").FirstOrDefault();
	}
}