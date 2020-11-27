using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200014A RID: 330
public class PlayerTab : MonoBehaviour
{
	// Token: 0x060006E6 RID: 1766 RVA: 0x00028760 File Offset: 0x00026960
	public void Start()
	{
		float num = (float)Palette.PlayerColors.Length / 3f;
		for (int i = 0; i < Palette.PlayerColors.Length; i++)
		{
			float x = this.XRange.Lerp((float)(i % 3) / 2f);
			float y = this.YRange.Lerp(1f - (float)(i / 3) / num);
			ColorChip colorChip = UnityEngine.Object.Instantiate<ColorChip>(this.ColorTabPrefab);
			colorChip.transform.SetParent(base.transform);
			colorChip.transform.localPosition = new Vector3(x, y, -1f);
			int j = i;
			colorChip.Button.OnClick.AddListener(delegate()
			{
				this.SelectColor(j);
			});
			colorChip.Inner.color = Palette.PlayerColors[i];
			this.ColorChips.Add(colorChip);
		}
	}

	// Token: 0x060006E7 RID: 1767 RVA: 0x00006416 File Offset: 0x00004616
	public void OnEnable()
	{
		PlayerControl.SetPlayerMaterialColors((int)PlayerControl.LocalPlayer.Data.ColorId, this.DemoImage);
		PlayerControl.SetHatImage(SaveManager.LastHat, this.HatImage);
		PlayerControl.SetSkinImage(SaveManager.LastSkin, this.SkinImage);
	}

	// Token: 0x060006E8 RID: 1768 RVA: 0x00028858 File Offset: 0x00026A58
	public void Update()
	{
		this.UpdateAvailableColors();
		for (int i = 0; i < this.ColorChips.Count; i++)
		{
			this.ColorChips[i].InUseForeground.SetActive(!this.AvailableColors.Contains(i));
		}
	}

	// Token: 0x060006E9 RID: 1769 RVA: 0x00006452 File Offset: 0x00004652
	private void SelectColor(int colorId)
	{
		this.UpdateAvailableColors();
		if (this.AvailableColors.Remove(colorId))
		{
			SaveManager.BodyColor = (byte)colorId;
			if (PlayerControl.LocalPlayer)
			{
				PlayerControl.LocalPlayer.CmdCheckColor((byte)colorId);
			}
		}
	}

	// Token: 0x060006EA RID: 1770 RVA: 0x000288A8 File Offset: 0x00026AA8
	public void UpdateAvailableColors()
	{
		PlayerControl.SetPlayerMaterialColors((int)PlayerControl.LocalPlayer.Data.ColorId, this.DemoImage);
		for (int i = 0; i < Palette.PlayerColors.Length; i++)
		{
			this.AvailableColors.Add(i);
		}
		if (GameData.Instance)
		{
			List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
			for (int j = 0; j < allPlayers.Count; j++)
			{
				GameData.PlayerInfo playerInfo = allPlayers[j];
				this.AvailableColors.Remove((int)playerInfo.ColorId);
			}
		}
	}

	// Token: 0x040006A8 RID: 1704
	public ColorChip ColorTabPrefab;

	// Token: 0x040006A9 RID: 1705
	public SpriteRenderer DemoImage;

	// Token: 0x040006AA RID: 1706
	public SpriteRenderer HatImage;

	// Token: 0x040006AB RID: 1707
	public SpriteRenderer SkinImage;

	// Token: 0x040006AC RID: 1708
	public FloatRange XRange = new FloatRange(1.5f, 3f);

	// Token: 0x040006AD RID: 1709
	public FloatRange YRange = new FloatRange(-1f, -3f);

	// Token: 0x040006AE RID: 1710
	private HashSet<int> AvailableColors = new HashSet<int>();

	// Token: 0x040006AF RID: 1711
	private List<ColorChip> ColorChips = new List<ColorChip>();

	// Token: 0x040006B0 RID: 1712
	private const int Columns = 3;
}
