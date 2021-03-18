using System.Collections.Generic;
using UnityEngine;

public class PlayerTab : MonoBehaviour
{
	public ColorChip ColorTabPrefab;

	public SpriteRenderer DemoImage;

	public SpriteRenderer HatImage;

	private int CurrentPage = 0;

	public SpriteRenderer SkinImage;

	public FloatRange XRange = new FloatRange(1.5f, 3f);

	public FloatRange YRange = new FloatRange(-1f, -3f);

	private HashSet<int> AvailableColors = new HashSet<int>();

	private List<ColorChip> ColorChips = new List<ColorChip>();

    private const int Columns = 3;

    public SpriteRenderer HatImageExt;

    public SpriteRenderer HatImageExt2;

    public SpriteRenderer HatImageExt3;

	public SpriteRenderer HatImageExt4;

	public void Start()
	{
        var DobLeft = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "double_left.png"));
        var Left = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "left.png"));
		var DobRight = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "double_right.png"));
		var Right = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "right.png"));
		SetHat();
		ColorChip colorChip = Object.Instantiate(ColorTabPrefab);
		colorChip.transform.SetParent(base.transform);
		colorChip.transform.localPosition = new Vector3(1.65f, -3.33f, -1f);
        colorChip.transform.localScale *= 0.75f;
		colorChip.Inner.sprite = CE_TextureNSpriteExtensions.ConvertToSpriteAutoPivot(DobLeft);
        colorChip.Button.OnClick.AddListener(delegate
        {
            CurrentPage = 0;
            CreateColorPage(CurrentPage);
        });
		ColorChip colorChip2 = Object.Instantiate(ColorTabPrefab);
		colorChip2.transform.SetParent(base.transform);
		colorChip2.transform.localPosition = new Vector3(1.65f + 0.48f, -3.33f, -1f);
		colorChip2.transform.localScale *= 0.75f;
		colorChip2.Inner.sprite = CE_TextureNSpriteExtensions.ConvertToSpriteAutoPivot(Left);
        colorChip2.Button.OnClick.AddListener(delegate
        {
            CurrentPage = Mathf.Clamp(CurrentPage - 1, 0, Mathf.CeilToInt(Palette.PLColors.Count / 24));
            CreateColorPage(CurrentPage);
        });
		ColorChip colorChip3 = Object.Instantiate(ColorTabPrefab);
		colorChip3.transform.SetParent(base.transform);
		colorChip3.transform.localPosition = new Vector3(1.65f + (0.48f * 2), -3.33f, -1f);
		colorChip3.transform.localScale *= 0.75f;
		colorChip3.Inner.sprite = CE_TextureNSpriteExtensions.ConvertToSpriteAutoPivot(Right);
        colorChip3.Button.OnClick.AddListener(delegate
        {
            CurrentPage = Mathf.Clamp(CurrentPage + 1, 0, Mathf.CeilToInt(Palette.PLColors.Count / 24));
            CreateColorPage(CurrentPage);
        });
		ColorChip colorChip4 = Object.Instantiate(ColorTabPrefab);
		colorChip4.transform.SetParent(base.transform);
		colorChip4.transform.localPosition = new Vector3(1.65f + (0.48f * 3), -3.33f, -1f);
		colorChip4.transform.localScale *= 0.75f;
		colorChip4.Inner.sprite = CE_TextureNSpriteExtensions.ConvertToSpriteAutoPivot(DobRight);
		colorChip4.Button.OnClick.AddListener(delegate
		{
			CurrentPage = Mathf.CeilToInt(Palette.PLColors.Count / 24);
			CreateColorPage(CurrentPage);
		});
		CreateColorPage(0);
	}

	public void CreateColorPage(int page)
    {
		foreach (ColorChip ch in ColorChips)
		{
			Destroy(ch.gameObject);
		}
		ColorChips = new List<ColorChip>();
		float num = 1.65f;
		float num2 = -0.33f;
		float num3 = 0f;
		int num4 = 0;
		var SpecialClrChip = CE_TextureNSpriteExtensions.LoadPNG(System.IO.Path.Combine(Application.dataPath, "CE_Assets", "Textures", "colorchip_special.png"));
		Sprite specsprite = Sprite.Create(SpecialClrChip, new Rect(0f, 0f, SpecialClrChip.width, SpecialClrChip.height), new Vector2(0.5f, 0.5f));
		while (num4 < Mathf.Clamp(Palette.PLColors.Count, 0, 24))
		{
			for (float num5 = 0f; num5 < 1.5f; num5 += 0.48f)
			{
				if (num4 < Palette.PLColors.Count)
				{
					int i = num4 + (24 * page);
					if (!(i >= Palette.PLColors.Count))
					{
						ColorChip colorChip = Object.Instantiate(ColorTabPrefab);
						colorChip.transform.SetParent(base.transform);
						colorChip.transform.localPosition = new Vector3(num + num5, num2 + num3, -1f);
						colorChip.transform.localScale *= 0.75f;
						colorChip.Button.OnClick.AddListener(delegate
						{
							SelectColor((uint)i);
						});
						colorChip.Inner.color = Palette.PLColors[i].Base;
						if (Palette.PLColors[i].IsSpecial)
						{
							colorChip.Inner.sprite = specsprite;
						}
						if (Palette.PLColors[i].IsFunnyRainbowColor)
						{
							colorChip.gameObject.AddComponent<CE_RainbowColorNPS>();
						}
						ColorChips.Add(colorChip);
					}
					num4++;
				}
			}
			num3 -= 0.5f;
		}
	}

	public void OnEnable()
	{
		PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.Data.ColorId, DemoImage);
		SetHat();
		PlayerControl.SetSkinImage(SaveManager.LastSkin, SkinImage);
	}

	public void Update()
	{
		UpdateAvailableColors();
		for (int i = 0; i < ColorChips.Count; i++)
		{
			if (ColorChips[i])
			{
				ColorChips[i].InUseForeground.SetActive(!AvailableColors.Contains(i + (CurrentPage * 24)));
			}
		}
		UpdateExtHats();
	}

	private void Freeplay_SwapColor(uint colorId, uint lastColorId)
    {
		if ((bool)GameData.Instance)
		{
			List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
			for (int j = 0; j < allPlayers.Count; j++)
			{
				GameData.PlayerInfo playerInfo = allPlayers[j];
				if (playerInfo.ColorId == colorId)
                {
                    playerInfo.Object.RpcSetColor(lastColorId);
					PlayerControl.LocalPlayer.RpcSetColor(colorId);
					return;
				}
			}
			PlayerControl.LocalPlayer.RpcSetColor(colorId);
		}
	}

	private void SelectColor(uint colorId)
	{
		UpdateAvailableColors();
		if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
        {
			Freeplay_SwapColor(colorId, SaveManager.BodyColor);
		}
		if (AvailableColors.Remove((int)colorId))
		{
			SaveManager.BodyColor = colorId;
			if ((bool)PlayerControl.LocalPlayer)
			{
				PlayerControl.LocalPlayer.CmdCheckColor(colorId);
				PlayerControl.LocalPlayer.RpcSetHat(SaveManager.LastHat);
			}
		}
		SetHat();
	}

	public void UpdateAvailableColors()
	{
		PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.Data.ColorId, DemoImage);
		for (int i = 0; i < Palette.PLColors.Count; i++)
		{
			AvailableColors.Add(i);
		}

		if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
		{
			if ((bool)GameData.Instance)
			{
				AvailableColors.Remove((int)SaveManager.BodyColor);
				return;
			}

		}
		else
		{
			if ((bool)GameData.Instance)
			{
				List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
				for (int j = 0; j < allPlayers.Count; j++)
				{
					GameData.PlayerInfo playerInfo = allPlayers[j];
					AvailableColors.Remove((int)playerInfo.ColorId);
				}
			}
		}


	}

	public void UpdateExtHats()
	{
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
}
