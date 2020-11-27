using System;
using System.Collections;
using PowerTools;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class OverlayKillAnimation : MonoBehaviour
{
	// Token: 0x060007DA RID: 2010 RVA: 0x0002C51C File Offset: 0x0002A71C
	public void Begin(PlayerControl killer, GameData.PlayerInfo victim)
	{
		if (this.killerParts != null)
		{
			GameData.PlayerInfo data = killer.Data;
			for (int i = 0; i < this.killerParts.Length; i++)
			{
				Renderer renderer = this.killerParts[i];
				PlayerControl.SetPlayerMaterialColors((int)data.ColorId, renderer);
				if (renderer.name.StartsWith("HatSlot"))
				{
					PlayerControl.SetHatImage(data.HatId, (SpriteRenderer)renderer);
				}
				else if (renderer.name.StartsWith("Skin"))
				{
					switch (this.KillType)
					{
					case KillAnimType.Stab:
					case KillAnimType.Neck:
						PlayerControl.SetSkinImage(data.SkinId, (SpriteRenderer)renderer);
						break;
					case KillAnimType.Tongue:
					{
						SkinData skinById = DestroyableSingleton<HatManager>.Instance.GetSkinById(data.SkinId);
						renderer.GetComponent<SpriteAnim>().Play(skinById.KillTongueImpostor, 1f);
						break;
					}
					case KillAnimType.Shoot:
					{
						SkinData skinById2 = DestroyableSingleton<HatManager>.Instance.GetSkinById(data.SkinId);
						renderer.GetComponent<SpriteAnim>().Play(skinById2.KillShootImpostor, 1f);
						break;
					}
					}
				}
			}
		}
		if (victim != null && this.victimParts != null)
		{
			this.victimHat = victim.HatId;
			for (int j = 0; j < this.victimParts.Length; j++)
			{
				Renderer renderer2 = this.victimParts[j];
				PlayerControl.SetPlayerMaterialColors((int)victim.ColorId, renderer2);
				if (renderer2.name.StartsWith("HatSlot"))
				{
					PlayerControl.SetHatImage(this.victimHat, (SpriteRenderer)renderer2);
				}
				else if (renderer2.name.StartsWith("Skin"))
				{
					SkinData skinById3 = DestroyableSingleton<HatManager>.Instance.GetSkinById(victim.SkinId);
					switch (this.KillType)
					{
					case KillAnimType.Stab:
						renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillStabVictim, 1f);
						break;
					case KillAnimType.Tongue:
						renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillTongueVictim, 1f);
						break;
					case KillAnimType.Shoot:
						renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillShootVictim, 1f);
						break;
					case KillAnimType.Neck:
						renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillNeckVictim, 1f);
						break;
					}
				}
			}
		}
	}

	// Token: 0x060007DB RID: 2011 RVA: 0x0002C758 File Offset: 0x0002A958
	public void SetHatFloor()
	{
		for (int i = 0; i < this.victimParts.Length; i++)
		{
			Renderer renderer = this.victimParts[i];
			if (renderer.name.StartsWith("HatSlot"))
			{
				((SpriteRenderer)renderer).sprite = DestroyableSingleton<HatManager>.Instance.GetHatById(this.victimHat).FloorImage;
			}
		}
	}

	// Token: 0x060007DC RID: 2012 RVA: 0x00006D64 File Offset: 0x00004F64
	public void PlayKillSound()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(this.Sfx, false, 1f).volume = 0.8f;
		}
	}

	// Token: 0x060007DD RID: 2013 RVA: 0x00006D8D File Offset: 0x00004F8D
	public IEnumerator WaitForFinish()
	{
		SpriteAnim[] anims = base.GetComponentsInChildren<SpriteAnim>();
		if (anims.Length == 0)
		{
			yield return new WaitForSeconds(1f);
		}
		else
		{
			for (;;)
			{
				bool flag = false;
				for (int i = 0; i < anims.Length; i++)
				{
					if (anims[i].IsPlaying((AnimationClip)null))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
				yield return null;
			}
		}
		yield break;
	}

	// Token: 0x040007BE RID: 1982
	public KillAnimType KillType;

	// Token: 0x040007BF RID: 1983
	public Renderer[] killerParts;

	// Token: 0x040007C0 RID: 1984
	public Renderer[] victimParts;

	// Token: 0x040007C1 RID: 1985
	private uint victimHat;

	// Token: 0x040007C2 RID: 1986
	public AudioClip Stinger;

	// Token: 0x040007C3 RID: 1987
	public AudioClip Sfx;

	// Token: 0x040007C4 RID: 1988
	public float StingerVolume = 0.6f;
}
