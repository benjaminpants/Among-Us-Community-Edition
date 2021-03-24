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
		SkinData[] unlockedSkins = DestroyableSingleton<HatManager>.Instance.GetUnlockedSkins();
		for (int i = 0; i < unlockedSkins.Length; i++)
		{
			SkinData skin = unlockedSkins[i];
			if (!skin.IsHidden)
			{
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
		UpdateExtHats();
	}

	private void SelectHat(SkinData skin)
	{
		uint skinId = (SaveManager.LastSkin = DestroyableSingleton<HatManager>.Instance.GetIdFromSkin(skin));
		PlayerControl.SetSkinImage(SaveManager.LastSkin, SkinImage, (int)PlayerControl.LocalPlayer.Data.ColorId);
		if ((bool)PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.RpcSetSkin(skinId);
			SetHat();
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

	private void SetHatImage(uint hatId, SpriteRenderer target, int hatSlot = 0, int playerColor = 0)
    {
		if (DestroyableSingleton<HatManager>.InstanceExists)
		{
			CE_WardrobeManager.SetExtHatImage(DestroyableSingleton<HatManager>.Instance.GetHatById(hatId), target, hatSlot, playerColor, true);
		}
	}

	public void SetHat()
	{
		SetHatImage(SaveManager.LastHat, HatImage, 0, (int)SaveManager.BodyColor);
		SetHatImage(SaveManager.LastHat, HatImageExt, 1, (int)SaveManager.BodyColor);
		SetHatImage(SaveManager.LastHat, HatImageExt2, 2, (int)SaveManager.BodyColor);
		SetHatImage(SaveManager.LastHat, HatImageExt3, 3, (int)SaveManager.BodyColor);
		SetHatImage(SaveManager.LastHat, HatImageExt4, 4, (int)SaveManager.BodyColor);

		CE_WardrobeManager.MatchBaseHatRender(HatImageExt, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt2, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt3, HatImage);
		CE_WardrobeManager.MatchBaseHatRender(HatImageExt4, HatImage);
	}


}
