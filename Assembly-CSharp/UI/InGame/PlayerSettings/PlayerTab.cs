using System.Collections.Generic;
using UnityEngine;

public class PlayerTab : MonoBehaviour
{
	public ColorChip ColorTabPrefab;

	public SpriteRenderer DemoImage;

	public SpriteRenderer HatImage;

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
		float num = 1.65f;
		float num2 = -0.33f;
		_ = (float)Palette.PlayerColors.Length / 3f;
		float num3 = 0f;
		int num4 = 0;
		Sprite specsprite = Sprite.Create(CE_CommonUI.SpecialClrChip,new Rect(0f,0f, CE_CommonUI.SpecialClrChip.width, CE_CommonUI.SpecialClrChip.height),new Vector2(0.5f,0.5f));
		while (num4 < Palette.PlayerColors.Length)
		{
			for (float num5 = 0f; num5 < 1.5f; num5 += 0.48f)
			{
				if (num4 < Palette.PlayerColors.Length)
				{
					ColorChip colorChip = Object.Instantiate(ColorTabPrefab);
					colorChip.transform.SetParent(base.transform);
					colorChip.transform.localPosition = new Vector3(num + num5, num2 + num3, -1f);
					int i = num4;
                    colorChip.transform.localScale *= 0.75f;
					colorChip.Button.OnClick.AddListener(delegate
					{
						SelectColor(i);
					});
					colorChip.Inner.color = Palette.PlayerColors[num4];
					switch (i) //below code is a mess
					{
						case 21:
							colorChip.Inner.sprite = specsprite;
							break;
						case 22:
							colorChip.Inner.sprite = specsprite;
							break;
						case 23:
							colorChip.Inner.sprite = specsprite;
							break;
						case 25:
							colorChip.Inner.sprite = specsprite;
							break;
						case 26:
							colorChip.Inner.sprite = specsprite;
                            break;
						case 27:
							colorChip.Inner.sprite = specsprite;
							break;
						case 28:
							colorChip.Inner.sprite = specsprite;
							break;
						default:
							//do nothing
							break;
					}
					ColorChips.Add(colorChip);
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
			ColorChips[i].InUseForeground.SetActive(!AvailableColors.Contains(i));
		}
		UpdateExtHats();
	}

	private void Freeplay_SwapColor(byte colorId, byte lastColorId)
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

	private void SelectColor(int colorId)
	{
		UpdateAvailableColors();
		if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
        {
			if (AmongUsClient.Instance.GameMode == GameModes.FreePlay) Freeplay_SwapColor((byte)colorId, SaveManager.BodyColor);
		}
		if (AvailableColors.Remove(colorId))
		{
			SaveManager.BodyColor = (byte)colorId;
			if ((bool)PlayerControl.LocalPlayer)
			{
				PlayerControl.LocalPlayer.CmdCheckColor((byte)colorId);
			}
		}
	}

	public void UpdateAvailableColors()
	{
		PlayerControl.SetPlayerMaterialColors(PlayerControl.LocalPlayer.Data.ColorId, DemoImage);
		for (int i = 0; i < Palette.PlayerColors.Length; i++)
		{
			AvailableColors.Add(i);
		}

		if (AmongUsClient.Instance.GameMode == GameModes.FreePlay)
		{
			if ((bool)GameData.Instance)
			{
				AvailableColors.Remove(SaveManager.BodyColor);
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
					AvailableColors.Remove(playerInfo.ColorId);
				}
			}
		}


	}

	public void UpdateExtHats()
	{
		CE_WardrobeManager.UpdateSpriteRenderer(HatImageExt, HatImage);
		CE_WardrobeManager.UpdateSpriteRenderer(HatImageExt2, HatImage);
		CE_WardrobeManager.UpdateSpriteRenderer(HatImageExt3, HatImage);
		CE_WardrobeManager.UpdateSpriteRenderer(HatImageExt4, HatImage);
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
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImage);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt, 1);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt2, 2);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt3, 3);
		PlayerControl.SetHatImage(SaveManager.LastHat, HatImageExt4, 4);
	}
}
