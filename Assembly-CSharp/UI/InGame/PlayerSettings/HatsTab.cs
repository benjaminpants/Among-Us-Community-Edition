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

	public SpriteRenderer HatImageExt;

	public SpriteRenderer HatImageExt2;

	public SpriteRenderer HatImageExt3;

	public SpriteRenderer HatImageExt4;

	public void OnStart()
	{
		SetHat();
	}

	public void OnEnable()
	{
		PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.Data.ColorId, DemoImage);
		SetHat();
		PlayerControl.SetSkinImage(SaveManager.LastSkin, SkinImage, (int)PlayerControl.LocalPlayer.Data.ColorId);
		HatBehaviour[] unlockedHats = DestroyableSingleton<HatManager>.Instance.GetUnlockedHats();
		for (int i = 0; i < unlockedHats.Length; i++)
		{
			HatBehaviour hat = unlockedHats[i];
			if (!hat.IsHidden)
            {
				float x = XRange.Lerp((float)(i % NumPerRow) / ((float)NumPerRow - 1f));
				float y = YStart - (float)(i / NumPerRow) * YOffset;
				ColorChip colorChip = Object.Instantiate(ColorTabPrefab, scroller.Inner);
				colorChip.Button.OnClick.AddListener(delegate
				{
					SelectHat(hat);
				});
				SetColorChipImages(colorChip, hat);
				colorChip.hatId = DestroyableSingleton<HatManager>.Instance.GetIdFromHat(hat);
				colorChip.transform.localPosition = new Vector3(x, y, -1f);
				ColorChips.Add(colorChip);
			}
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
			colorChip.UpdateHat();
		}
		UpdateExtHats();
	}

	private void SelectHat(HatBehaviour hat)
	{
		uint hatId = (SaveManager.LastHat = DestroyableSingleton<HatManager>.Instance.GetIdFromHat(hat));
		SetHatImage(hatId);
		if ((bool)PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.RpcSetHat(hatId);
		}
	}

	public void UpdateExtHats()
    {
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImage, 0, (int)SaveManager.BodyColor);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt, 1, (int)SaveManager.BodyColor);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt2, 2, (int)SaveManager.BodyColor);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt3, 3, (int)SaveManager.BodyColor);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt4, 4, (int)SaveManager.BodyColor);

		CE_WardrobeManager.MatchBaseHatRender(HatImageExt, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt2, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt3, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt4, HatImage);
	}

	public void Awake()
	{
		HatImageExt = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
		HatImageExt2 = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
		HatImageExt3 = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
		HatImageExt4 = CE_WardrobeManager.CreateExtSpriteRender(HatImage);
	}

	public void SetHatImage(uint hatId)
	{
		PlayerControl.SetHatImage(hatId, HatImage, 0, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(hatId, HatImageExt, 1, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(hatId, HatImageExt2, 2, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(hatId, HatImageExt3, 3, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(hatId, HatImageExt4, 4, (int)PlayerControl.LocalPlayer.Data.ColorId);

		CE_WardrobeManager.MatchBaseHatRender(HatImageExt, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt2, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt3, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt4, HatImage);
	}

	public void SetHat()
	{
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImage, 0, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt, 1, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt2, 2, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt3, 3, (int)PlayerControl.LocalPlayer.Data.ColorId);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt4, 4, (int)PlayerControl.LocalPlayer.Data.ColorId);

		CE_WardrobeManager.MatchBaseHatRender(HatImageExt, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt2, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt3, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt4, HatImage);
	}

	private void SetColorChipImages(ColorChip colorChip, HatBehaviour hat)
    {
		colorChip.Inner.sprite = (hat.PreviewImage ? hat.PreviewImage : hat.MainImage);
		colorChip.InnerA.sprite = (hat.PreviewImageExt ? hat.PreviewImageExt : hat.MainImageExt);
		colorChip.InnerB.sprite = (hat.PreviewImageExt2 ? hat.PreviewImageExt2 : hat.MainImageExt2);
		colorChip.InnerC.sprite = (hat.PreviewImageExt3 ? hat.PreviewImageExt3 : hat.MainImageExt3);
		colorChip.InnerD.sprite = (hat.PreviewImageExt4 ? hat.PreviewImageExt4 : hat.MainImageExt4);
	}
}
