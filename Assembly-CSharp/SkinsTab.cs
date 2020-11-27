using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014C RID: 332
public class SkinsTab : MonoBehaviour
{
	// Token: 0x060006EE RID: 1774 RVA: 0x00028984 File Offset: 0x00026B84
	public void OnEnable()
	{
		PlayerControl.SetPlayerMaterialColors((int)PlayerControl.LocalPlayer.Data.ColorId, this.DemoImage);
		PlayerControl.SetHatImage(SaveManager.LastHat, this.HatImage);
		PlayerControl.SetSkinImage(SaveManager.LastSkin, this.SkinImage);
		SkinData[] unlockedSkins = DestroyableSingleton<HatManager>.Instance.GetUnlockedSkins();
		for (int i = 0; i < unlockedSkins.Length; i++)
		{
			SkinData skin = unlockedSkins[i];
			float x = this.XRange.Lerp((float)(i % this.NumPerRow) / ((float)this.NumPerRow - 1f));
			float y = this.YStart - (float)(i / this.NumPerRow) * this.YOffset;
			ColorChip colorChip = UnityEngine.Object.Instantiate<ColorChip>(this.ColorTabPrefab, this.scroller.Inner);
			colorChip.transform.localPosition = new Vector3(x, y, -1f);
			colorChip.Button.OnClick.AddListener(delegate()
			{
				this.SelectHat(skin);
			});
			colorChip.Inner.sprite = skin.IdleFrame;
			this.ColorChips.Add(colorChip);
		}
		this.scroller.YBounds.max = -(this.YStart - (float)(unlockedSkins.Length / this.NumPerRow) * this.YOffset) - 3f;
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x00028AE0 File Offset: 0x00026CE0
	public void OnDisable()
	{
		for (int i = 0; i < this.ColorChips.Count; i++)
		{
			UnityEngine.Object.Destroy(this.ColorChips[i].gameObject);
		}
		this.ColorChips.Clear();
	}

	// Token: 0x060006F0 RID: 1776 RVA: 0x00028B24 File Offset: 0x00026D24
	public void Update()
	{
		PlayerControl.SetPlayerMaterialColors((int)PlayerControl.LocalPlayer.Data.ColorId, this.DemoImage);
		SkinData skinById = DestroyableSingleton<HatManager>.Instance.GetSkinById(SaveManager.LastSkin);
		for (int i = 0; i < this.ColorChips.Count; i++)
		{
			ColorChip colorChip = this.ColorChips[i];
			colorChip.InUseForeground.SetActive(skinById.IdleFrame == colorChip.Inner.sprite);
		}
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x00028BA0 File Offset: 0x00026DA0
	private void SelectHat(SkinData skin)
	{
		uint idFromSkin = DestroyableSingleton<HatManager>.Instance.GetIdFromSkin(skin);
		SaveManager.LastSkin = idFromSkin;
		PlayerControl.SetSkinImage(SaveManager.LastSkin, this.SkinImage);
		if (PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.RpcSetSkin(idFromSkin);
		}
	}

	// Token: 0x040006B3 RID: 1715
	public ColorChip ColorTabPrefab;

	// Token: 0x040006B4 RID: 1716
	public SpriteRenderer DemoImage;

	// Token: 0x040006B5 RID: 1717
	public SpriteRenderer HatImage;

	// Token: 0x040006B6 RID: 1718
	public SpriteRenderer SkinImage;

	// Token: 0x040006B7 RID: 1719
	public FloatRange XRange = new FloatRange(1.5f, 3f);

	// Token: 0x040006B8 RID: 1720
	public float YStart = 0.8f;

	// Token: 0x040006B9 RID: 1721
	public float YOffset = 0.8f;

	// Token: 0x040006BA RID: 1722
	public int NumPerRow = 4;

	// Token: 0x040006BB RID: 1723
	public Scroller scroller;

	// Token: 0x040006BC RID: 1724
	private List<ColorChip> ColorChips = new List<ColorChip>();
}
