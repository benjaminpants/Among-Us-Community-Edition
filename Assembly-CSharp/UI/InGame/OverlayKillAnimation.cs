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

	private SkinData LastSkinData_Victim;

	private SkinData LastSkinData_Killer;

	public void Begin(PlayerControl killer, GameData.PlayerInfo victim)
	{
		UpdateCustomVisual(killer, victim);
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
						case KillAnimType.Stab:
						{
							PlayerControl.SetSkinImage(data.SkinId, (SpriteRenderer)renderer);
							break;
						}
						case KillAnimType.Neck:
						{
							PlayerControl.SetSkinImage(data.SkinId, (SpriteRenderer)renderer);
							break;
						}
						case KillAnimType.Tongue:
						{
							SkinData skinById2 = DestroyableSingleton<HatManager>.Instance.GetSkinById(data.SkinId);
							renderer.GetComponent<SpriteAnim>().Play(skinById2.KillTongueImpostor, 1f);
							break;
						}
						case KillAnimType.Shoot:
						{
							SkinData skinById = DestroyableSingleton<HatManager>.Instance.GetSkinById(data.SkinId);
							renderer.GetComponent<SpriteAnim>().Play(skinById.KillShootImpostor, 1f);
							break;
						}
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

	private void UpdateCustomVisual(PlayerControl killer, GameData.PlayerInfo victim)
	{
        if (killer != null)
        {
            LastSkinData_Killer = DestroyableSingleton<HatManager>.Instance.GetSkinById(killer.Data.SkinId);
        }
		if (victim != null)
		{
			LastSkinData_Victim = DestroyableSingleton<HatManager>.Instance.GetSkinById(victim.SkinId);
		}
	}

	private void LateUpdate()
	{
		SpriteAnim[] anims = GetComponentsInChildren<SpriteAnim>();
		for (int i = 0; i < anims.Length; i++)
		{
			anims[i].Speed = CE_WardrobeLoader.AnimationEditor_CurrentSpeed;
			anims[i].Paused = CE_WardrobeLoader.AnimationEditor_Paused;
		}

		if (!LastSkinData_Killer || !LastSkinData_Victim) return;
		if (LastSkinData_Killer.isCustom)
		{
			for (int i = 0; i < killerParts.Length; i++)
			{
				Renderer renderer = killerParts[i];
				if (renderer.name.StartsWith("Skin"))
				{
					CE_WardrobeLoader.LogPivot(renderer);
					var sprite = CE_WardrobeLoader.GetSkin(renderer.GetComponent<SpriteRenderer>().sprite.name, LastSkinData_Killer);
					if (sprite) renderer.GetComponent<SpriteRenderer>().sprite = sprite;
				}
			}
		}
		if (LastSkinData_Victim.isCustom)
		{
			for (int j = 0; j < victimParts.Length; j++)
			{
				Renderer renderer = victimParts[j];
				if (renderer.name.StartsWith("Skin"))
				{
					CE_WardrobeLoader.LogPivot(renderer);
					var sprite = CE_WardrobeLoader.GetSkin(renderer.GetComponent<SpriteRenderer>().sprite.name, LastSkinData_Victim);
					if (sprite) renderer.GetComponent<SpriteRenderer>().sprite = sprite;
				}
			}

		}
	}
}
