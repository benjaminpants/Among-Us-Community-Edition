using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutscene : MonoBehaviour
{
	public static IntroCutscene Instance;

	public TextRenderer Title;

	public TextRenderer ImpostorText;

	public SpriteRenderer PlayerPrefab;

	public MeshRenderer BackgroundBar;

	public MeshRenderer Foreground;

	public FloatRange ForegroundRadius;

	public SpriteRenderer FrontMost;

	public AudioClip IntroStinger;

	public float BaseY = -0.25f;

	public IEnumerator CoBegin(List<PlayerControl> yourTeam, bool isImpostor)
	{
		SoundManager.Instance.PlaySound(IntroStinger, loop: false);
		if ((bool)PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.moveable = false;
		}
		if (!isImpostor)
		{
			BeginCrewmate(yourTeam);
		}
		else
		{
			BeginImpostor(yourTeam);
		}
		Color c = Title.Color;
		Color fade = Color.black;
		Color impColor = Color.white;
		Vector3 titlePos = Title.transform.localPosition;
		float timer2 = 0f;
		while (timer2 < 3f)
		{
			timer2 += Time.deltaTime;
			float num = Mathf.Min(1f, timer2 / 3f);
			Foreground.material.SetFloat("_Rad", ForegroundRadius.ExpOutLerp(num * 2f));
			fade.a = Mathf.Lerp(1f, 0f, num * 3f);
			FrontMost.color = fade;
			c.a = Mathf.Clamp(FloatRange.ExpOutLerp(num, 0f, 1f), 0f, 1f);
			Title.Color = c;
			impColor.a = Mathf.Lerp(0f, 1f, (num - 0.3f) * 3f);
			ImpostorText.Color = impColor;
			titlePos.y = 2.7f - num * 0.3f;
			Title.transform.localPosition = titlePos;
			yield return null;
		}
		timer2 = 0f;
		while (timer2 < 1f)
		{
			timer2 += Time.deltaTime;
			float num2 = timer2 / 1f;
			fade.a = Mathf.Lerp(0f, 1f, num2 * 3f);
			FrontMost.color = fade;
			yield return null;
		}
		if ((bool)PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.moveable = true;
		}
		Object.Destroy(base.gameObject);
	}

	private void BeginCrewmate(List<PlayerControl> yourTeam)
	{
		CE_Role UserRole = CE_RoleManager.GetRoleFromID(PlayerControl.LocalPlayer.Data.role);
		Vector3 position = BackgroundBar.transform.position;
		position.y -= 0.25f;
		BackgroundBar.transform.position = position;
		if (UserRole.RoleText == "default_text" || PlayerControl.LocalPlayer.Data.role == 0)
		{
			if (PlayerControl.GameOptions.NumImpostors == 1)
			{
				if (PlayerControl.GameOptions.Gamemode == 1)
				{
					ImpostorText.Text = $"There is [62A74AFF]{PlayerControl.GameOptions.NumImpostors} Infected[] among us";
				}
				else
				{
					ImpostorText.Text = $"There is [FF1919FF]{PlayerControl.GameOptions.NumImpostors} Impostor[] among us";
				}
			}
			else if (PlayerControl.GameOptions.Gamemode == 1)
			{
				ImpostorText.Text = $"There are [62A74AFF]{PlayerControl.GameOptions.NumImpostors} Infected[] among us";
			}
			else
			{
				ImpostorText.Text = $"There are [FF1919FF]{PlayerControl.GameOptions.NumImpostors} Impostors[] among us";
			}
		}
		else
        {
			ImpostorText.Text = UserRole.RoleText;

		}
		if (PlayerControl.LocalPlayer.Data.role != 0)
		{
			Title.Text = UserRole.RoleName;
			Title.Color = UserRole.RoleColor;
			BackgroundBar.material.SetColor("_Color", UserRole.RoleColor);
		}
		else
		{
			Title.Text = "Crewmate";
			Title.Color = Palette.CrewmateBlue;
			BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
		}
		for (int i = 0; i < yourTeam.Count; i++)
		{
			PlayerControl playerControl = yourTeam[i];
			if (!playerControl)
			{
				continue;
			}
			GameData.PlayerInfo data = playerControl.Data;
			if (data != null)
			{
				SpriteRenderer spriteRenderer = Object.Instantiate(PlayerPrefab, base.transform);
				spriteRenderer.name = data.PlayerName + "Dummy";
				spriteRenderer.flipX = i % 2 == 0;
				float d = 1.5f;
				int num = ((i % 2 != 0) ? 1 : (-1));
				int num2 = (i + 1) / 2;
				float num3 = ((i == 0) ? 1.2f : 1f) - (float)num2 * 0.12f;
				float num4 = 1f - (float)num2 * 0.08f;
				float num5 = ((i == 0) ? (-8) : (-1));
				PlayerControl.SetPlayerMaterialColors(data.ColorId, spriteRenderer);
				spriteRenderer.transform.localPosition = new Vector3(0.8f * (float)num * (float)num2 * num4, BaseY - 0.25f + (float)num2 * 0.1f, num5 + (float)num2 * 0.01f) * d;
				Vector3 localScale = new Vector3(num3, num3, num3) * d;
				spriteRenderer.transform.localScale = localScale;
				spriteRenderer.GetComponentInChildren<TextRenderer>().gameObject.SetActive(value: false);
				SpriteRenderer component = spriteRenderer.transform.Find("SkinLayer").GetComponent<SpriteRenderer>();
				component.flipX = !spriteRenderer.flipX;
				DestroyableSingleton<HatManager>.Instance.SetSkin(component, data.SkinId);
				SpriteRenderer component2 = spriteRenderer.transform.Find("HatSlot").GetComponent<SpriteRenderer>();
				component2.flipX = !spriteRenderer.flipX;
				if (spriteRenderer.flipX)
				{
					Vector3 localPosition = component2.transform.localPosition;
					localPosition.x = 0f - localPosition.x;
					component2.transform.localPosition = localPosition;
				}
				PlayerControl.SetHatImage(data.HatId, component2);
			}
		}
	}

	private void BeginImpostor(List<PlayerControl> yourTeam)
	{
		ImpostorText.gameObject.SetActive(value: false);
		if (PlayerControl.GameOptions.Gamemode == 1)
		{
			Title.Text = "Infected";
			BackgroundBar.material.SetColor("_Color", Palette.InfectedGreen);
			Title.Color = Palette.InfectedGreen;
		}
		else
		{
			Title.Text = "Impostor";
			Title.Color = Palette.ImpostorRed;
		}
		for (int i = 0; i < yourTeam.Count; i++)
		{
			PlayerControl playerControl = yourTeam[i];
			if (!playerControl)
			{
				continue;
			}
			GameData.PlayerInfo data = playerControl.Data;
			if (data != null)
			{
				SpriteRenderer spriteRenderer = Object.Instantiate(PlayerPrefab, base.transform);
				spriteRenderer.flipX = i % 2 == 1;
				float d = 1.5f;
				int num = ((i % 2 != 0) ? 1 : (-1));
				int num2 = (i + 1) / 2;
				float num3 = 1f - (float)num2 * 0.075f;
				float num4 = 1f - (float)num2 * 0.035f;
				PlayerControl.SetPlayerMaterialColors(data.ColorId, spriteRenderer);
				float num5 = ((i == 0) ? (-8) : (-1));
				spriteRenderer.transform.localPosition = new Vector3((float)(num * num2) * num4, BaseY + (float)num2 * 0.15f, num5 + (float)num2 * 0.01f) * d;
				Vector3 vector = new Vector3(num3, num3, num3) * d;
				spriteRenderer.transform.localScale = vector;
				TextRenderer componentInChildren = spriteRenderer.GetComponentInChildren<TextRenderer>();
				componentInChildren.Text = data.PlayerName;
				componentInChildren.transform.localScale = vector.Inv();
				SpriteRenderer component = spriteRenderer.transform.Find("SkinLayer").GetComponent<SpriteRenderer>();
				component.flipX = !spriteRenderer.flipX;
				DestroyableSingleton<HatManager>.Instance.SetSkin(component, data.SkinId);
				SpriteRenderer component2 = spriteRenderer.transform.Find("HatSlot").GetComponent<SpriteRenderer>();
				component2.flipX = !spriteRenderer.flipX;
				if (spriteRenderer.flipX)
				{
					Vector3 localPosition = component2.transform.localPosition;
					localPosition.x = 0f - localPosition.x;
					component2.transform.localPosition = localPosition;
				}
				PlayerControl.SetHatImage(data.HatId, component2);
			}
		}
	}
}
