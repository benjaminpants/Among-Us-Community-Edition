using System.Collections;
using PowerTools;
using UnityEngine;

public class OverlayKillAnimation : MonoBehaviour
{
	public KillAnimType KillType;

	public Renderer[] killerParts;

	public Renderer[] victimParts;

	private uint victimHat;

	public AudioClip Stinger;

	public AudioClip Sfx;

	public float StingerVolume = 0.6f;

	public void Begin(PlayerControl killer, GameData.PlayerInfo victim)
	{
		if (killerParts != null)
		{
			GameData.PlayerInfo data = killer.Data;
			for (int i = 0; i < killerParts.Length; i++)
			{
				Renderer renderer = killerParts[i];
				PlayerControl.SetPlayerMaterialColors(data.ColorId, renderer);
				if (renderer.name.StartsWith("HatSlot"))
				{
					PlayerControl.SetHatImage(data.HatId, (SpriteRenderer)renderer);
				}
				else if (renderer.name.StartsWith("Skin"))
				{
					switch (KillType)
					{
					case KillAnimType.Tongue:
					{
						SkinData skinById2 = DestroyableSingleton<HatManager>.Instance.GetSkinById(data.SkinId);
						renderer.GetComponent<SpriteAnim>().Play(skinById2.KillTongueImpostor);
						break;
					}
					case KillAnimType.Shoot:
					{
						SkinData skinById = DestroyableSingleton<HatManager>.Instance.GetSkinById(data.SkinId);
						renderer.GetComponent<SpriteAnim>().Play(skinById.KillShootImpostor);
						break;
					}
					case KillAnimType.Stab:
					case KillAnimType.Neck:
						PlayerControl.SetSkinImage(data.SkinId, (SpriteRenderer)renderer);
						break;
					}
				}
			}
		}
		if (victim == null || victimParts == null)
		{
			return;
		}
		victimHat = victim.HatId;
		for (int j = 0; j < victimParts.Length; j++)
		{
			Renderer renderer2 = victimParts[j];
			PlayerControl.SetPlayerMaterialColors(victim.ColorId, renderer2);
			if (renderer2.name.StartsWith("HatSlot"))
			{
				PlayerControl.SetHatImage(victimHat, (SpriteRenderer)renderer2);
			}
			else if (renderer2.name.StartsWith("Skin"))
			{
				SkinData skinById3 = DestroyableSingleton<HatManager>.Instance.GetSkinById(victim.SkinId);
				switch (KillType)
				{
				case KillAnimType.Tongue:
					renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillTongueVictim);
					break;
				case KillAnimType.Stab:
					renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillStabVictim);
					break;
				case KillAnimType.Neck:
					renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillNeckVictim);
					break;
				case KillAnimType.Shoot:
					renderer2.GetComponent<SpriteAnim>().Play(skinById3.KillShootVictim);
					break;
				}
			}
		}
	}

	public void SetHatFloor()
	{
		for (int i = 0; i < victimParts.Length; i++)
		{
			Renderer renderer = victimParts[i];
			if (renderer.name.StartsWith("HatSlot"))
			{
				((SpriteRenderer)renderer).sprite = DestroyableSingleton<HatManager>.Instance.GetHatById(victimHat).FloorImage;
			}
		}
	}

	public void PlayKillSound()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlaySound(Sfx, loop: false).volume = 0.8f;
		}
	}

	public IEnumerator WaitForFinish()
	{
		SpriteAnim[] anims = GetComponentsInChildren<SpriteAnim>();
		if (anims.Length == 0)
		{
			yield return new WaitForSeconds(1f);
			yield break;
		}
		while (true)
		{
			bool flag = false;
			for (int i = 0; i < anims.Length; i++)
			{
				if (anims[i].IsPlaying())
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				yield return null;
				continue;
			}
			break;
		}
	}
}
