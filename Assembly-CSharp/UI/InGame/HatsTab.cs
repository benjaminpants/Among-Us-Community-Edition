using System.Collections.Generic;
using UnityEngine;

public class HatsTab : MonoBehaviour
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
		HatBehaviour[] unlockedHats = DestroyableSingleton<HatManager>.Instance.GetUnlockedHats();
		for (int i = 0; i < unlockedHats.Length; i++)
		{
			HatBehaviour hat = unlockedHats[i];
			float x = XRange.Lerp((float)(i % NumPerRow) / ((float)NumPerRow - 1f));
			float y = YStart - (float)(i / NumPerRow) * YOffset;
			ColorChip colorChip = Object.Instantiate(ColorTabPrefab, scroller.Inner);
			colorChip.transform.localPosition = new Vector3(x, y, -1f);
			colorChip.Button.OnClick.AddListener(delegate
			{
				SelectHat(hat);
			});
			colorChip.Inner.sprite = (hat.PreviewImage ? hat.PreviewImage : hat.MainImage);
			ColorChips.Add(colorChip);
		}
		scroller.YBounds.max = 0f - (YStart - (float)(unlockedHats.Length / NumPerRow) * YOffset) - 3f;
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
		HatBehaviour hatById = DestroyableSingleton<HatManager>.Instance.GetHatById(SaveManager.LastHat);
		for (int i = 0; i < ColorChips.Count; i++)
		{
			ColorChip colorChip = ColorChips[i];
			colorChip.InUseForeground.SetActive((hatById.PreviewImage ? hatById.PreviewImage : hatById.MainImage) == colorChip.Inner.sprite);
		}
	}

	private void SelectHat(HatBehaviour hat)
	{
		uint hatId = (SaveManager.LastHat = DestroyableSingleton<HatManager>.Instance.GetIdFromHat(hat));
		PlayerControl.SetHatImage(hatId, HatImage);
		if ((bool)PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.RpcSetHat(hatId);
		}
	}
}
