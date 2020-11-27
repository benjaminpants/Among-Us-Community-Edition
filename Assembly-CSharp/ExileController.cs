using System;
using System.Collections;
using System.Linq;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class ExileController : MonoBehaviour
{
	// Token: 0x06000252 RID: 594 RVA: 0x00012C84 File Offset: 0x00010E84
	public void Begin(GameData.PlayerInfo exiled, bool tie)
	{
		this.exiled = exiled;
		this.Text.gameObject.SetActive(false);
		this.Text.Text = string.Empty;
		int num = GameData.Instance.AllPlayers.Count((GameData.PlayerInfo p) => p.IsImpostor && !p.IsDead && !p.Disconnected);
		if (exiled != null)
		{
			GameData.PlayerInfo playerById = GameData.Instance.GetPlayerById(exiled.PlayerId);
			int num2 = GameData.Instance.AllPlayers.Count((GameData.PlayerInfo p) => p.IsImpostor);
			string text = exiled.IsImpostor ? string.Empty : "not ";
			string text2 = (PlayerControl.GameOptions.Gamemode == 1) ? " Infected" : " Impostor";
			string text3 = (num2 > 1) ? "An" : "The";
			if (PlayerControl.GameOptions.ConfirmEject)
			{
				this.completeString = string.Concat(new string[]
				{
					exiled.PlayerName,
					" was ",
					text,
					text3,
					text2
				});
			}
			else
			{
				this.completeString = string.Concat(new string[]
				{
					exiled.PlayerName,
					" was ejected."
				});
			}
			PlayerControl.SetPlayerMaterialColors((int)playerById.ColorId, this.Player);
			PlayerControl.SetHatImage(exiled.HatId, this.PlayerHat);
			this.PlayerSkin.sprite = DestroyableSingleton<HatManager>.Instance.GetSkinById(playerById.SkinId).EjectFrame;
			if (exiled.IsImpostor)
			{
				num--;
			}
		}
		else
		{
			this.completeString = string.Format("No one was ejected ({0})", tie ? "Tie" : "Skipped");
			this.Player.gameObject.SetActive(false);
		}
		if (PlayerControl.GameOptions.ConfirmEject)
		{
			if (PlayerControl.GameOptions.Gamemode == 1)
			{
				this.ImpostorText.Text = num + " infected remain.";
			}
			else
			{
				this.ImpostorText.Text = num + ((num != 1) ? " impostors remain" : " impostor remains");
			}
		}
		else
		{
			this.ImpostorText.Text = "";
		}
		base.StartCoroutine(this.Animate());
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00003772 File Offset: 0x00001972
	private IEnumerator Animate()
	{
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear, 0.2f);
		yield return new WaitForSeconds(1f);
		float d = Camera.main.orthographicSize * Camera.main.aspect + 1f;
		Vector2 left = Vector2.left * d;
		Vector2 right = Vector2.right * d;
		for (float t = 0f; t <= this.Duration; t += Time.deltaTime)
		{
			float num = t / this.Duration;
			this.Player.transform.localPosition = Vector2.Lerp(left, right, this.LerpCurve.Evaluate(num));
			float z = (t + 0.75f) * 25f / Mathf.Exp(t * 0.75f + 1f);
			this.Player.transform.Rotate(new Vector3(0f, 0f, z));
			if (num >= 0.3f)
			{
				int num2 = (int)(Mathf.Min(1f, (num - 0.3f) / 0.3f) * (float)this.completeString.Length);
				if (num2 > this.Text.Text.Length)
				{
					this.Text.Text = this.completeString.Substring(0, num2);
					this.Text.gameObject.SetActive(true);
					if (this.completeString[num2 - 1] != ' ')
					{
						SoundManager.Instance.PlaySoundImmediate(this.TextSound, false, 0.8f, 1f);
					}
				}
			}
			yield return null;
		}
		this.Text.Text = this.completeString;
		this.ImpostorText.gameObject.SetActive(true);
		yield return Effects.Bloop(0f, this.ImpostorText.transform, 0.5f);
		yield return new WaitForSeconds(0.5f);
		yield return DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.clear, Color.black, 0.2f);
		if (this.exiled != null)
		{
			PlayerControl @object = this.exiled.Object;
			if (@object != null)
			{
				@object.Exiled();
			}
		}
		if (DestroyableSingleton<TutorialManager>.InstanceExists || !ShipStatus.Instance.IsGameOverDueToDeath())
		{
			DestroyableSingleton<HudManager>.Instance.StartCoroutine(DestroyableSingleton<HudManager>.Instance.CoFadeFullScreen(Color.black, Color.clear, 0.2f));
			PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
			Camera.main.GetComponent<FollowerCamera>().Locked = false;
			DestroyableSingleton<HudManager>.Instance.SetHudActive(true);
		}
		UnityEngine.Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x04000234 RID: 564
	public TextRenderer ImpostorText;

	// Token: 0x04000235 RID: 565
	public TextRenderer Text;

	// Token: 0x04000236 RID: 566
	public SpriteRenderer Player;

	// Token: 0x04000237 RID: 567
	public SpriteRenderer PlayerHat;

	// Token: 0x04000238 RID: 568
	public SpriteRenderer PlayerSkin;

	// Token: 0x04000239 RID: 569
	public AnimationCurve LerpCurve;

	// Token: 0x0400023A RID: 570
	public float Duration = 7f;

	// Token: 0x0400023B RID: 571
	public AudioClip TextSound;

	// Token: 0x0400023C RID: 572
	private string completeString = string.Empty;

	// Token: 0x0400023D RID: 573
	private GameData.PlayerInfo exiled;
}
