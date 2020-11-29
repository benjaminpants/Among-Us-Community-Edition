using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014F RID: 335
public class HatsTab : MonoBehaviour
{
	// Token: 0x06000703 RID: 1795
	public void OnEnable()
	{
		PlayerControl.SetPlayerMaterialColors((int)PlayerControl.LocalPlayer.Data.ColorId, this.DemoImage);
		PlayerControl.SetHatImage(SaveManager.LastHat, this.HatImage);
		PlayerControl.SetSkinImage(SaveManager.LastSkin, this.SkinImage);
		HatBehaviour[] unlockedHats = DestroyableSingleton<HatManager>.Instance.GetUnlockedHats();
		for (int i = 0; i < unlockedHats.Length; i++)
		{
			HatBehaviour hat = unlockedHats[i];
			float x = this.XRange.Lerp((float)(i % this.NumPerRow) / ((float)this.NumPerRow - 1f));
			float y = this.YStart - (float)(i / this.NumPerRow) * this.YOffset;
			ColorChip colorChip = UnityEngine.Object.Instantiate<ColorChip>(this.ColorTabPrefab, this.scroller.Inner);
			colorChip.transform.localPosition = new Vector3(x, y, -1f);
			colorChip.Button.OnClick.AddListener(delegate()
			{
				this.SelectHat(hat);
			});
			colorChip.Inner.sprite = (hat.PreviewImage ? hat.PreviewImage : hat.MainImage);
			this.ColorChips.Add(colorChip);
		}
		this.scroller.YBounds.max = -(this.YStart - (float)(unlockedHats.Length / this.NumPerRow) * this.YOffset) - 3f;
	}

	// Token: 0x06000704 RID: 1796
	public void OnDisable()
	{
		for (int i = 0; i < this.ColorChips.Count; i++)
		{
			UnityEngine.Object.Destroy(this.ColorChips[i].gameObject);
		}
		this.ColorChips.Clear();
	}

	// Token: 0x06000705 RID: 1797
	public void Update()
	{
		PlayerControl.SetPlayerMaterialColors((int)PlayerControl.LocalPlayer.Data.ColorId, this.DemoImage);
		HatBehaviour hatById = DestroyableSingleton<HatManager>.Instance.GetHatById(SaveManager.LastHat);
		for (int i = 0; i < this.ColorChips.Count; i++)
		{
			ColorChip colorChip = this.ColorChips[i];
			colorChip.InUseForeground.SetActive((hatById.PreviewImage ? hatById.PreviewImage : hatById.MainImage) == colorChip.Inner.sprite);
		}
	}

	// Token: 0x06000706 RID: 1798
	private void SelectHat(HatBehaviour hat)
	{
		uint idFromHat = DestroyableSingleton<HatManager>.Instance.GetIdFromHat(hat);
		SaveManager.LastHat = idFromHat;
		PlayerControl.SetHatImage(idFromHat, this.HatImage);
		bool flag = PlayerControl.LocalPlayer;
		if (flag)
		{
			PlayerControl.LocalPlayer.RpcSetHat(idFromHat);
		}
	}

	// Token: 0x040006BC RID: 1724
	public ColorChip ColorTabPrefab;

	// Token: 0x040006BD RID: 1725
	public SpriteRenderer DemoImage;

	// Token: 0x040006BE RID: 1726
	public SpriteRenderer HatImage;

	// Token: 0x040006BF RID: 1727
	public SpriteRenderer SkinImage;

	// Token: 0x040006C0 RID: 1728
	public FloatRange XRange = new FloatRange(1.5f, 3f);

	// Token: 0x040006C1 RID: 1729
	public float YStart = 0.8f;

	// Token: 0x040006C2 RID: 1730
	public float YOffset = 0.8f;

	// Token: 0x040006C3 RID: 1731
	public int NumPerRow = 4;

	// Token: 0x040006C4 RID: 1732
	public Scroller scroller;

	// Token: 0x040006C5 RID: 1733
	private List<ColorChip> ColorChips = new List<ColorChip>();
}
