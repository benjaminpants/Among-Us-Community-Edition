using System.Collections;
using Hazel;
using UnityEngine;
using System.Linq;

public class PlayerVoteArea : MonoBehaviour
{
	public sbyte TargetPlayerId;

	public const byte DeadBit = 128;

	public const byte VotedBit = 64;

	public const byte ReportedBit = 32;

	public const byte VoteMask = 31;

	public GameObject Buttons;

	public SpriteRenderer PlayerIcon;

	public SpriteRenderer Flag;

	public SpriteRenderer Megaphone;

	public SpriteRenderer Overlay;

	public TextRenderer NameText;

	public bool isDead;

	public bool didVote;

	public bool didReport;

	public sbyte votedFor;

	public bool voteComplete;

	public bool resultsShowing;

	public MeetingHud Parent
	{
		get;
		set;
	}

    public SpriteRenderer PlayerHat;

	public SpriteRenderer PlayerHatExt;

	public SpriteRenderer PlayerHatExt2;

	public SpriteRenderer PlayerHatExt3;

	public SpriteRenderer PlayerHatExt4;

	public SpriteRenderer PlayerSkin;

	public Sprite PlayerSprite;

	public Sprite LegacyPlayerSprite;

	public bool IconsLoaded = false;

	public void SetDead(bool isMe, bool didReport, bool isDead)
	{
		this.isDead = isDead;
		this.didReport = didReport;
		Megaphone.enabled = didReport;
		Overlay.gameObject.SetActive(value: false);
		Overlay.transform.GetChild(0).gameObject.SetActive(isDead);
	}

	public void SetDisabled()
	{
		if (!isDead)
		{
			if ((bool)Overlay)
			{
				Overlay.gameObject.SetActive(value: true);
				Overlay.transform.GetChild(0).gameObject.SetActive(value: false);
			}
			else
			{
				GetComponent<SpriteRenderer>().enabled = false;
			}
		}
	}

	public void SetEnabled()
	{
		if (!isDead)
		{
			if ((bool)Overlay)
			{
				Overlay.gameObject.SetActive(value: false);
			}
			else
			{
				GetComponent<SpriteRenderer>().enabled = true;
			}
		}
	}

	public IEnumerator CoAnimateOverlay()
	{
		Overlay.gameObject.SetActive(isDead);
		if (isDead)
		{
			Transform xMark = Overlay.transform.GetChild(0);
			Overlay.color = Palette.ClearWhite;
			xMark.localScale = Vector3.zero;
			float fadeDuration2 = 0.5f;
			for (float t2 = 0f; t2 < fadeDuration2; t2 += Time.deltaTime)
			{
				Overlay.color = Color.Lerp(Palette.ClearWhite, Color.white, t2 / fadeDuration2);
				yield return null;
			}
			Overlay.color = Color.white;
			float scaleDuration2 = 0.15f;
			for (float t2 = 0f; t2 < scaleDuration2; t2 += Time.deltaTime)
			{
				float num = Mathf.Lerp(3f, 1f, t2 / scaleDuration2);
				xMark.transform.localScale = new Vector3(num * 0.5f, num, num);
				yield return null;
			}
			xMark.transform.localScale = new Vector3(0.5f, 1f, 1f);
		}
		else if (didReport)
		{
			float scaleDuration2 = 1f;
			for (float fadeDuration2 = 0f; fadeDuration2 < scaleDuration2; fadeDuration2 += Time.deltaTime)
			{
				float num2 = fadeDuration2 / scaleDuration2;
				float num3 = TriangleWave(num2 * 3f) * 2f - 1f;
				Megaphone.transform.localEulerAngles = new Vector3(0f, 0f, num3 * 30f);
				num3 = Mathf.Lerp(0.7f, 1.2f, TriangleWave(num2 * 2f));
				Megaphone.transform.localScale = new Vector3(num3 * 0.5f, num3, num3);
				yield return null;
			}
			Megaphone.transform.localEulerAngles = Vector3.zero;
			Megaphone.transform.localScale = new Vector3(0.5f, 1f, 1f);
		}
	}

	private static float TriangleWave(float t)
	{
		t -= (float)(int)t;
		if (t < 0.5f)
		{
			return t * 2f;
		}
		return 1f - (t - 0.5f) * 2f;
	}

	internal void SetVote(sbyte suspectIdx)
	{
		didVote = true;
		votedFor = suspectIdx;
		Flag.enabled = true;
		Flag.transform.localScale = new Vector3(0f,0f,0f);
		StartCoroutine(Effects.BloopHalf(0f, Flag.transform, 2f));
	}

	public void UnsetVote()
	{
		Flag.enabled = false;
		votedFor = 0;
		didVote = false;
	}

	public void ClearButtons()
	{
		Buttons.SetActive(value: false);
	}

	public void ClearForResults()
	{
		resultsShowing = true;
		Flag.enabled = false;
	}

	public void VoteForMe()
	{
		if (!voteComplete)
		{
			Parent.Confirm(TargetPlayerId);
		}
	}

	public void Select()
	{
		if (!PlayerControl.LocalPlayer.Data.IsDead && !isDead && !voteComplete && Parent.Select(TargetPlayerId))
		{
			Buttons.SetActive(value: true);
		}
	}

	public void Cancel()
	{
		Buttons.SetActive(value: false);
	}

	public void Serialize(MessageWriter writer)
	{
		byte state = GetState();
		writer.Write(state);
	}

	public void Deserialize(MessageReader reader)
	{
		byte b = reader.ReadByte();
		votedFor = (sbyte)((b & 0x1F) - 1);
		isDead = (b & 0x80) > 0;
		didVote = (b & 0x40) > 0;
		didReport = (b & 0x20) > 0;
        Flag.enabled = didVote && !resultsShowing;
		Flag.transform.localScale = new Vector3(0.5f, 1f, 1f);
		Overlay.gameObject.SetActive(isDead);
		Megaphone.enabled = didReport;
	}

	public byte GetState()
	{
		return (byte)(((uint)(votedFor + 1) & 0x1Fu) | (isDead ? 128u : 0u) | (didVote ? 64u : 0u) | (didReport ? 32u : 0u));
	}

	public void Update()
    {
		if (PlayerIcon != null)
        {
			PlayerControl playerControl = PlayerControl.AllPlayerControls.FirstOrDefault((PlayerControl p) => p.PlayerId == TargetPlayerId);
			if (playerControl == null) return;
			var hatID = playerControl.Data.HatId;
			HatBehaviour Hat = DestroyableSingleton<HatManager>.Instance.GetHatById(playerControl.Data.HatId);

			if (!IconsLoaded)
            {
				PlayerHat = CreateNewSprite(PlayerIcon);
				PlayerHatExt = CreateNewSprite(PlayerIcon);
				PlayerHatExt2 = CreateNewSprite(PlayerIcon);
				PlayerHatExt3 = CreateNewSprite(PlayerIcon);
				PlayerHatExt4 = CreateNewSprite(PlayerIcon);
				PlayerSkin = CreateNewSprite(PlayerIcon);


				PlayerControl.SetHatImage(hatID, PlayerHat, 0, (int)playerControl.Data.ColorId);
				PlayerControl.SetHatImage(hatID, PlayerHatExt, 1, (int)playerControl.Data.ColorId);
				PlayerControl.SetHatImage(hatID, PlayerHatExt2, 2, (int)playerControl.Data.ColorId);
				PlayerControl.SetHatImage(hatID, PlayerHatExt3, 3, (int)playerControl.Data.ColorId);
				PlayerControl.SetHatImage(hatID, PlayerHatExt4, 4, (int)playerControl.Data.ColorId);
				PlayerControl.SetSkinImage(playerControl.Data.SkinId, PlayerSkin, (int)playerControl.Data.ColorId);
				IconsLoaded = true;
			}

			if (!PlayerSprite)
            {
				string PlayerSpritePath = System.IO.Path.Combine(CE_Extensions.GetTexturesDirectory("UI"), "VotePlayerIcon.png");
				PlayerSprite = CE_TextureNSpriteExtensions.ConvertToSprite(CE_TextureNSpriteExtensions.LoadPNG(PlayerSpritePath), new Vector2(0.5f, 0.5f));				
			}

			if (!LegacyPlayerSprite)
			{
				LegacyPlayerSprite = PlayerIcon.sprite;
			}

			if (SaveManager.UseLegacyVoteIcons)
			{
				PlayerIcon.sprite = LegacyPlayerSprite;
			}
			else
            {
				PlayerIcon.sprite = PlayerSprite;
			}

			UpdateHatLayer(PlayerHat, PlayerIcon, Hat.InFront, 1f);
			UpdateHatLayer(PlayerHatExt, PlayerIcon, Hat.InFrontExt, 2f);
			UpdateHatLayer(PlayerHatExt2, PlayerIcon, Hat.InFrontExt2, 3f);
			UpdateHatLayer(PlayerHatExt3, PlayerIcon, Hat.InFrontExt3, 4f);
			UpdateHatLayer(PlayerHatExt4, PlayerIcon, Hat.InFrontExt4, 5f);
			UpdateSpriteLayer(PlayerSkin, PlayerIcon);
		}
	}

	public void UpdateHatLayer(SpriteRenderer dest, SpriteRenderer source, bool inFront, float offset)
	{
		if (!SaveManager.UseLegacyVoteIcons)
        {
			dest.enabled = true;
			dest.flipX = source.flipX;
			dest.flipY = source.flipY;
			dest.transform.parent = source.transform.parent;
			dest.transform.localRotation = source.transform.localRotation;
			dest.transform.localScale = source.transform.localScale;
			var localPos = source.transform.localPosition;
			localPos.y += 0.4f;
			if (inFront) localPos.z -= offset;
			dest.transform.localPosition = localPos;
		}
		else
        {
			dest.enabled = false;
		}

	}

	public void UpdateSpriteLayer(SpriteRenderer dest, SpriteRenderer source)
	{
		if (!SaveManager.UseLegacyVoteIcons)
		{
			dest.enabled = true;
			dest.flipX = source.flipX;
			dest.flipY = source.flipY;
			dest.transform.parent = source.transform.parent;
			dest.transform.localRotation = source.transform.localRotation;
			dest.transform.localScale = source.transform.localScale;
			var localPos = source.transform.localPosition;
			localPos.z -= 1f;
			dest.transform.localPosition = localPos;
		}
		else
        {
			dest.enabled = false;
		}
	}

	public SpriteRenderer CreateNewSprite(SpriteRenderer spriteRenderer)
	{
		GameObject gameObject = new GameObject();
		SpriteRenderer newSprite = gameObject.AddComponent<SpriteRenderer>();
		gameObject.layer = spriteRenderer.gameObject.layer;
		newSprite.transform.SetParent(spriteRenderer.transform);
		return newSprite;
	}
}
