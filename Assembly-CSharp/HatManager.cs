using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200009B RID: 155
public class HatManager : DestroyableSingleton<HatManager>
{
	// Token: 0x0600033A RID: 826 RVA: 0x000041E4 File Offset: 0x000023E4
	public HatBehaviour GetHatById(uint hatId)
	{
		if ((ulong)hatId >= (ulong)((long)this.AllHats.Count))
		{
			return this.NoneHat;
		}
		return this.AllHats[(int)hatId];
	}

	// Token: 0x0600033B RID: 827 RVA: 0x000174C8 File Offset: 0x000156C8
	public HatBehaviour[] GetUnlockedHats()
	{
		return (from h in this.AllHats
		where h.LimitedMonth == 0 || SaveManager.GetPurchase(h.ProductId)
		select h into o
		orderby o.Order descending, o.name
		select o).ToArray<HatBehaviour>();
	}

	// Token: 0x0600033C RID: 828 RVA: 0x00004209 File Offset: 0x00002409
	public uint GetIdFromHat(HatBehaviour hat)
	{
		return (uint)this.AllHats.IndexOf(hat);
	}

	// Token: 0x0600033D RID: 829 RVA: 0x0001754C File Offset: 0x0001574C
	public SkinData[] GetUnlockedSkins()
	{
		return (from o in this.AllSkins
		orderby o.Order descending, o.name
		select o).ToArray<SkinData>();
	}

	// Token: 0x0600033E RID: 830 RVA: 0x00004217 File Offset: 0x00002417
	public uint GetIdFromSkin(SkinData skin)
	{
		return (uint)this.AllSkins.IndexOf(skin);
	}

	// Token: 0x0600033F RID: 831 RVA: 0x00004225 File Offset: 0x00002425
	internal SkinData GetSkinById(uint skinId)
	{
		if ((ulong)skinId >= (ulong)((long)this.AllSkins.Count))
		{
			return this.AllSkins[0];
		}
		return this.AllSkins[(int)skinId];
	}

	// Token: 0x06000340 RID: 832 RVA: 0x000175AC File Offset: 0x000157AC
	internal void SetSkin(SpriteRenderer skinRend, uint skinId)
	{
		SkinData skinById = this.GetSkinById(skinId);
		if (skinById)
		{
			skinRend.sprite = skinById.IdleFrame;
		}
	}

	// Token: 0x06000342 RID: 834 RVA: 0x0000426E File Offset: 0x0000246E
	public void Start()
	{
		this.AllHats.AddRange(HatLoader.LoadHats());
	}

	// Token: 0x0400033C RID: 828
	public HatBehaviour NoneHat;

	// Token: 0x0400033D RID: 829
	public List<HatBehaviour> AllHats = new List<HatBehaviour>();

	// Token: 0x0400033E RID: 830
	public List<SkinData> AllSkins = new List<SkinData>();
}
