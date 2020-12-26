using System.Collections.Generic;
using UnityEngine;

public class SkinsTab : MonoBehaviour
{
	public ColorChip ColorTabPrefab;

	public SpriteRenderer DemoImage;

	public SpriteRenderer HatImage;

	public SpriteRenderer SkinImage;

	public FloatRange XRange = new FloatRange(1.5f, 3f);

	public float YStart = 0.8f;

	public float YOffset = 0.8f;

	public int NumPerRow = 4;

	public Scroller scroller;

	private List<ColorChip> ColorChips = new List<ColorChip>();

	public void OnEnable()
	{
		PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.Data.ColorId, DemoImage);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImage);
		PlayerControl.SetSkinImage(SaveManager.LastSkin, SkinImage);
		SkinData[] unlockedSkins = DestroyableSingleton<HatManager>.Instance.GetUnlockedSkins();
		for (int i = 0; i < unlockedSkins.Length; i++)
		{
			SkinData skin = unlockedSkins[i];
			float x = XRange.Lerp((float)(i % NumPerRow) / ((float)NumPerRow - 1f));
			float y = YStart - (float)(i / NumPerRow) * YOffset;
			ColorChip colorChip = Object.Instantiate(ColorTabPrefab, scroller.Inner);
			colorChip.transform.localPosition = new Vector3(x, y, -1f);
			colorChip.Button.OnClick.AddListener(delegate
			{
				SelectHat(skin);
			});
			colorChip.Inner.sprite = skin.IdleFrame;
			ColorChips.Add(colorChip);
		}
		scroller.YBounds.max = 0f - (YStart - (float)(unlockedSkins.Length / NumPerRow) * YOffset) - 3f;
	}

	public void OnDisable()
	{
		for (int i = 0; i < ColorChips.Count; i++)
		{
			Object.Destroy(ColorChips[i].gameObject);
		}
		ColorChips.Clear();
	}

	public void Update()
	{
		PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.Data.ColorId, DemoImage);
		SkinData skinById = DestroyableSingleton<HatManager>.Instance.GetSkinById(SaveManager.LastSkin);
		for (int i = 0; i < ColorChips.Count; i++)
		{
			ColorChip colorChip = ColorChips[i];
			colorChip.InUseForeground.SetActive(skinById.IdleFrame == colorChip.Inner.sprite);
		}
	}

	private void SelectHat(SkinData skin)
	{
		uint skinId = (SaveManager.LastSkin = DestroyableSingleton<HatManager>.Instance.GetIdFromSkin(skin));
		PlayerControl.SetSkinImage(SaveManager.LastSkin, SkinImage);
		if ((bool)PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.RpcSetSkin(skinId);
		}
	}
}
