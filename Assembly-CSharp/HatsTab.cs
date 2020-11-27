using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000146 RID: 326
public class HatsTab : MonoBehaviour
{
	// Token: 0x060006D2 RID: 1746 RVA: 0x00028228 File Offset: 0x00026428
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
			colorChip.Inner.sprite = hat.MainImage;
			this.ColorChips.Add(colorChip);
		}
		this.scroller.YBounds.max = -(this.YStart - (float)(unlockedHats.Length / this.NumPerRow) * this.YOffset) - 3f;
	}

	// Token: 0x060006D3 RID: 1747 RVA: 0x00028384 File Offset: 0x00026584
	public void OnDisable()
	{
		for (int i = 0; i < this.ColorChips.Count; i++)
		{
			UnityEngine.Object.Destroy(this.ColorChips[i].gameObject);
		}
		this.ColorChips.Clear();
	}

	// Token: 0x060006D4 RID: 1748 RVA: 0x000283C8 File Offset: 0x000265C8
	public void Update()
	{
		PlayerControl.SetPlayerMaterialColors((int)PlayerControl.LocalPlayer.Data.ColorId, this.DemoImage);
		HatBehaviour hatById = DestroyableSingleton<HatManager>.Instance.GetHatById(SaveManager.LastHat);
		for (int i = 0; i < this.ColorChips.Count; i++)
		{
			ColorChip colorChip = this.ColorChips[i];
			colorChip.InUseForeground.SetActive(hatById.MainImage == colorChip.Inner.sprite);
		}
	}

	// Token: 0x060006D5 RID: 1749 RVA: 0x00028444 File Offset: 0x00026644
	private void SelectHat(HatBehaviour hat)
	{
		uint idFromHat = DestroyableSingleton<HatManager>.Instance.GetIdFromHat(hat);
		SaveManager.LastHat = idFromHat;
		PlayerControl.SetHatImage(idFromHat, this.HatImage);
		if (PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.RpcSetHat(idFromHat);
		}
	}

	// Token: 0x04000693 RID: 1683
	public ColorChip ColorTabPrefab;

	// Token: 0x04000694 RID: 1684
	public SpriteRenderer DemoImage;

	// Token: 0x04000695 RID: 1685
	public SpriteRenderer HatImage;

	// Token: 0x04000696 RID: 1686
	public SpriteRenderer SkinImage;

	// Token: 0x04000697 RID: 1687
	public FloatRange XRange = new FloatRange(1.5f, 3f);

	// Token: 0x04000698 RID: 1688
	public float YStart = 0.8f;

	// Token: 0x04000699 RID: 1689
	public float YOffset = 0.8f;

	// Token: 0x0400069A RID: 1690
	public int NumPerRow = 4;

	// Token: 0x0400069B RID: 1691
	public Scroller scroller;

	// Token: 0x0400069C RID: 1692
	private List<ColorChip> ColorChips = new List<ColorChip>();
}
