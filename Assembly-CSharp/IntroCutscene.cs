using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000071 RID: 113
public class IntroCutscene : MonoBehaviour
{
	// Token: 0x06000263 RID: 611 RVA: 0x000037F3 File Offset: 0x000019F3
	public IEnumerator CoBegin(List<PlayerControl> yourTeam, bool isImpostor)
	{
		SoundManager.Instance.PlaySound(this.IntroStinger, false, 1f);
		if (PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.moveable = false;
		}
		if (!isImpostor)
		{
			this.BeginCrewmate(yourTeam);
		}
		else
		{
			this.BeginImpostor(yourTeam);
		}
		Color c = this.Title.Color;
		Color fade = Color.black;
		Color impColor = Color.white;
		Vector3 titlePos = this.Title.transform.localPosition;
		float timer = 0f;
		while (timer < 3f)
		{
			timer += Time.deltaTime;
			float num = Mathf.Min(1f, timer / 3f);
			this.Foreground.material.SetFloat("_Rad", this.ForegroundRadius.ExpOutLerp(num * 2f));
			fade.a = Mathf.Lerp(1f, 0f, num * 3f);
			this.FrontMost.color = fade;
			c.a = Mathf.Clamp(FloatRange.ExpOutLerp(num, 0f, 1f), 0f, 1f);
			this.Title.Color = c;
			impColor.a = Mathf.Lerp(0f, 1f, (num - 0.3f) * 3f);
			this.ImpostorText.Color = impColor;
			titlePos.y = 2.7f - num * 0.3f;
			this.Title.transform.localPosition = titlePos;
			yield return null;
		}
		timer = 0f;
		while (timer < 1f)
		{
			timer += Time.deltaTime;
			float num2 = timer / 1f;
			fade.a = Mathf.Lerp(0f, 1f, num2 * 3f);
			this.FrontMost.color = fade;
			yield return null;
		}
		if (PlayerControl.LocalPlayer)
		{
			PlayerControl.LocalPlayer.moveable = true;
		}
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x06000264 RID: 612 RVA: 0x0001324C File Offset: 0x0001144C
	private void BeginCrewmate(List<PlayerControl> yourTeam)
	{
		Vector3 position = this.BackgroundBar.transform.position;
		position.y -= 0.25f;
		this.BackgroundBar.transform.position = position;
		if (PlayerControl.GameOptions.NumImpostors == 1)
		{
			if (PlayerControl.GameOptions.Gamemode == 1)
			{
				this.ImpostorText.Text = string.Format("There is [62A74AFF]{0} Infected[] among us", PlayerControl.GameOptions.NumImpostors);
			}
			else
			{
				this.ImpostorText.Text = string.Format("There is [FF1919FF]{0} Impostor[] among us", PlayerControl.GameOptions.NumImpostors);
			}
		}
		else if (PlayerControl.GameOptions.Gamemode == 1)
		{
			this.ImpostorText.Text = string.Format("There are [62A74AFF]{0} Infected[] among us", PlayerControl.GameOptions.NumImpostors);
		}
		else
		{
			this.ImpostorText.Text = string.Format("There are [FF1919FF]{0} Impostors[] among us", PlayerControl.GameOptions.NumImpostors);
		}
		this.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
		if (PlayerControl.LocalPlayer.Data.role == GameData.PlayerInfo.Role.Sheriff)
		{
			this.Title.Text = "Sheriff";
			this.Title.Color = Palette.SheriffYellow;
			this.BackgroundBar.material.SetColor("_Color", Palette.SheriffYellow);
		}
		else
		{
			this.Title.Text = "Crewmate";
			this.Title.Color = Palette.CrewmateBlue;
			this.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
		}
		for (int i = 0; i < yourTeam.Count; i++)
		{
			PlayerControl playerControl = yourTeam[i];
			if (playerControl)
			{
				GameData.PlayerInfo data = playerControl.Data;
				if (data != null)
				{
					SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(this.PlayerPrefab, base.transform);
					spriteRenderer.name = data.PlayerName + "Dummy";
					spriteRenderer.flipX = (i % 2 == 0);
					float d = 1.5f;
					int num = (i % 2 == 0) ? -1 : 1;
					int num2 = (i + 1) / 2;
					float num3 = ((i == 0) ? 1.2f : 1f) - (float)num2 * 0.12f;
					float num4 = 1f - (float)num2 * 0.08f;
					float num5 = (float)((i == 0) ? -8 : -1);
					PlayerControl.SetPlayerMaterialColors((int)data.ColorId, spriteRenderer);
					spriteRenderer.transform.localPosition = new Vector3(0.8f * (float)num * (float)num2 * num4, this.BaseY - 0.25f + (float)num2 * 0.1f, num5 + (float)num2 * 0.01f) * d;
					Vector3 localScale = new Vector3(num3, num3, num3) * d;
					spriteRenderer.transform.localScale = localScale;
					spriteRenderer.GetComponentInChildren<TextRenderer>().gameObject.SetActive(false);
					SpriteRenderer component = spriteRenderer.transform.Find("SkinLayer").GetComponent<SpriteRenderer>();
					component.flipX = !spriteRenderer.flipX;
					DestroyableSingleton<HatManager>.Instance.SetSkin(component, data.SkinId);
					SpriteRenderer component2 = spriteRenderer.transform.Find("HatSlot").GetComponent<SpriteRenderer>();
					component2.flipX = !spriteRenderer.flipX;
					if (spriteRenderer.flipX)
					{
						Vector3 localPosition = component2.transform.localPosition;
						localPosition.x = -localPosition.x;
						component2.transform.localPosition = localPosition;
					}
					PlayerControl.SetHatImage(data.HatId, component2);
				}
			}
		}
	}

	// Token: 0x06000265 RID: 613 RVA: 0x000135D4 File Offset: 0x000117D4
	private void BeginImpostor(List<PlayerControl> yourTeam)
	{
		this.ImpostorText.gameObject.SetActive(false);
		if (PlayerControl.GameOptions.Gamemode == 1)
		{
			this.Title.Text = "Infected";
			this.BackgroundBar.material.SetColor("_Color", Palette.InfectedGreen);
			this.Title.Color = Palette.InfectedGreen;
		}
		else
		{
			this.Title.Text = "Impostor";
			this.Title.Color = Palette.ImpostorRed;
		}
		for (int i = 0; i < yourTeam.Count; i++)
		{
			PlayerControl playerControl = yourTeam[i];
			if (playerControl)
			{
				GameData.PlayerInfo data = playerControl.Data;
				if (data != null)
				{
					SpriteRenderer spriteRenderer = UnityEngine.Object.Instantiate<SpriteRenderer>(this.PlayerPrefab, base.transform);
					spriteRenderer.flipX = (i % 2 == 1);
					float d = 1.5f;
					int num = (i % 2 == 0) ? -1 : 1;
					int num2 = (i + 1) / 2;
					float num3 = 1f - (float)num2 * 0.075f;
					float num4 = 1f - (float)num2 * 0.035f;
					PlayerControl.SetPlayerMaterialColors((int)data.ColorId, spriteRenderer);
					float num5 = (float)((i == 0) ? -8 : -1);
					spriteRenderer.transform.localPosition = new Vector3((float)(num * num2) * num4, this.BaseY + (float)num2 * 0.15f, num5 + (float)num2 * 0.01f) * d;
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
						localPosition.x = -localPosition.x;
						component2.transform.localPosition = localPosition;
					}
					PlayerControl.SetHatImage(data.HatId, component2);
				}
			}
		}
	}

	// Token: 0x0400024A RID: 586
	public static IntroCutscene Instance;

	// Token: 0x0400024B RID: 587
	public TextRenderer Title;

	// Token: 0x0400024C RID: 588
	public TextRenderer ImpostorText;

	// Token: 0x0400024D RID: 589
	public SpriteRenderer PlayerPrefab;

	// Token: 0x0400024E RID: 590
	public MeshRenderer BackgroundBar;

	// Token: 0x0400024F RID: 591
	public MeshRenderer Foreground;

	// Token: 0x04000250 RID: 592
	public FloatRange ForegroundRadius;

	// Token: 0x04000251 RID: 593
	public SpriteRenderer FrontMost;

	// Token: 0x04000252 RID: 594
	public AudioClip IntroStinger;

	// Token: 0x04000253 RID: 595
	public float BaseY = -0.25f;
}
