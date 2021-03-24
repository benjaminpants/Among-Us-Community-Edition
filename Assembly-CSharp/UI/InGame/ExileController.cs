using System.Collections;
using System.Linq;
using UnityEngine;

public class ExileController : MonoBehaviour
{
	public TextRenderer ImpostorText;

	public TextRenderer Text;

	public SpriteRenderer Player;

	public SpriteRenderer PlayerHat;

	public SpriteRenderer PlayerSkin;

	public AnimationCurve LerpCurve;

	public float Duration = 7f;

	public AudioClip TextSound;

	private string completeString = string.Empty;

	private GameData.PlayerInfo exiled;

	public SpriteRenderer PlayerHatExt;

	public SpriteRenderer PlayerHatExt2;

	public SpriteRenderer PlayerHatExt3;

	public SpriteRenderer PlayerHatExt4;

	public bool IsDebugging = false;

	public void Awake()
    {
		PlayerHatExt = CE_WardrobeManager.CreateExtSpriteRender(PlayerHat);
		PlayerHatExt2 = CE_WardrobeManager.CreateExtSpriteRender(PlayerHat);
		PlayerHatExt3 = CE_WardrobeManager.CreateExtSpriteRender(PlayerHat);
		PlayerHatExt4 = CE_WardrobeManager.CreateExtSpriteRender(PlayerHat);
	}

	public void Begin(GameData.PlayerInfo exiled, bool tie)
	{
		this.exiled = exiled;
		Text.gameObject.SetActive(value: false);
		Text.Text = string.Empty;
		int num = GameData.Instance.AllPlayers.Count((GameData.PlayerInfo p) => p.IsImpostor && !p.IsDead && !p.Disconnected);
		if (exiled != null)
		{
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(exiled.PlayerId);
			int num2 = GameData.Instance.AllPlayers.Count((GameData.PlayerInfo p) => p.IsImpostor);
			string text = (exiled.IsImpostor ? string.Empty : "not ");
			string text2 = " Impostor";
			string text3 = ((num2 > 1) ? "An" : "The");
			if (exiled.role != 0) 
			{
				text2 = " " + CE_RoleManager.GetRoleFromID(exiled.role).RoleName;
				text3 = ((CE_RoleManager.GetRoleCount(exiled.role) > 1) ? CE_RoleManager.GetRoleFromID(exiled.role).RoleName.AOrAn(true) : "The");
				text = string.Empty;
			}
			if (PlayerControl.GameOptions.ConfirmEject)
			{
				if (exiled.IsImpostor || exiled.role != 0)
				{
					completeString = exiled.PlayerName + " was " + text + text3 + text2;
				}
				else
                {
					completeString = CE_LanguageManager.GetGMLanguage(PlayerControl.GameOptions.Gamemode).GetFormatted("Game_WasNotImp", exiled.PlayerName, text3);
				}
			}
			else
			{
				completeString = string.Concat(new string[2]
				{
					exiled.PlayerName,
					" was ejected."
				});
			}
			PlayerControl.SetPlayerMaterialColors(playerById.ColorId, Player);
			SetPlayerHat(exiled.HatId, (int)exiled.ColorId);
			PlayerSkin.sprite = DestroyableSingleton<HatManager>.Instance.GetSkinById(playerById.SkinId).EjectFrame;
			CE_WardrobeManager.SetHatRenderColors(PlayerSkin,(int)playerById.ColorId, DestroyableSingleton<HatManager>.Instance.GetSkinById(playerById.SkinId).IsPlayerOverride);
			if (exiled.IsImpostor)
			{
				num--;
			}
		}
		else
		{
			completeString = string.Format("No one was ejected ({0})", tie ? "Tie" : "Skipped");
			Player.gameObject.SetActive(value: false);
		}
		if (PlayerControl.GameOptions.ConfirmEject)
		{
			string thing = CE_LanguageManager.GetGMLanguage(PlayerControl.GameOptions.Gamemode).GetFormatted("Game_ImpsRemain",num, ((num != 1) ? "s" : ""));
			//ImpostorText.Text = num + ((num != 1) ? " impostors remain" : " impostor remains");
			ImpostorText.Text = thing;
		}
		else
		{
			ImpostorText.Text = "";
		}
		StartCoroutine(Animate());
	}

	private void SetPlayerHat(uint hatId, int ColorId)
    {
		CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt, PlayerHat);
		CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt2, PlayerHat);
		CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt3, PlayerHat);
		CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt4, PlayerHat);

		PlayerControl.SetHatImage(hatId, PlayerHat, 0, ColorId);
		PlayerControl.SetHatImage(hatId, PlayerHatExt, 1,  ColorId);
		PlayerControl.SetHatImage(hatId, PlayerHatExt2, 2, ColorId);
		PlayerControl.SetHatImage(hatId, PlayerHatExt3, 3, ColorId);
		PlayerControl.SetHatImage(hatId, PlayerHatExt4, 4, ColorId);
	}

	private IEnumerator Animate()
	{
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear);
		yield return new WaitForSeconds(1f);
		float d = Camera.main.orthographicSize * Camera.main.aspect + 1f;
		Vector2 left = Vector2.left * d;
		Vector2 right = Vector2.right * d;
		for (float t = 0f; t <= Duration; t += Time.deltaTime)
		{
			float num = t / Duration;
			Player.transform.localPosition = Vector2.Lerp(left, right, LerpCurve.Evaluate(num));
			float z = (t + 0.75f) * 25f / Mathf.Exp(t * 0.75f + 1f);
			Player.transform.Rotate(new Vector3(0f, 0f, z));
			if (num >= 0.3f)
			{
				int num2 = (int)(Mathf.Min(1f, (num - 0.3f) / 0.3f) * (float)completeString.Length);
				if (num2 > Text.Text.Length)
				{
					Text.Text = completeString.Substring(0, num2);
					Text.gameObject.SetActive(value: true);
					if (completeString[num2 - 1] != ' ')
					{
						SoundManager.Instance.PlaySoundImmediate(TextSound, loop: false, 0.8f);
					}
				}
			}
			yield return null;
		}
		Text.Text = completeString;
		ImpostorText.gameObject.SetActive(value: true);
		yield return Effects.Bloop(0f, ImpostorText.transform);
		yield return new WaitForSeconds(0.5f);
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black);
		if (exiled != null)
		{
			PlayerControl @object = exiled.Object;
			if (@object != null && !IsDebugging)
			{
				@object.Exiled();
			}
		}
        if (DestroyableSingleton<TutorialManager>.InstanceExists || !ShipStatus.Instance.IsGameOverDueToDeath())
        {
            DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear));
            PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
            Camera.main.GetComponent<FollowerCamera>().Locked = false;
            DestroyableSingleton<HudManager>.Instance.SetHudActive(isActive: true);
        }
		if (CE_LuaLoader.CurrentGMLua)
		{
			if (exiled != null)
			{
				CE_LuaLoader.GetGamemodeResult("OnExile", (CE_PlayerInfoLua)exiled);
			}
			else
			{
				CE_LuaLoader.GetGamemodeResult("OnExileSkip", null);
			}
		}
		ShipStatus.Instance.TimeSinceLastRound = 0f;
		IsDebugging = false;
		Object.Destroy(base.gameObject);
	}

	private void Update()
    {
		if (exiled != null)
		{
			CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt, PlayerHat);
			CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt2, PlayerHat);
			CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt3, PlayerHat);
			CE_WardrobeManager.MatchBaseHatRender(PlayerHatExt4, PlayerHat);
		}
	}


	private void LateUpdate()
	{
		if (exiled != null)
        {
			SkinData skinById = DestroyableSingleton<HatManager>.Instance.GetSkinById(exiled.SkinId);
			if (skinById)
			{
				bool isCustom = skinById.isCustom;
				if (isCustom)
				{
					PlayerSkin.sprite = CE_WardrobeManager.GetSkin("ejected", skinById);
				}
			}
		}

	}
}
