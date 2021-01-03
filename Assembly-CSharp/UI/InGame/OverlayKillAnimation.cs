using System.Collections;
using PowerTools;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
	
	public void Awake()
    {
		InitCustomHatLayers();
	}
	public void Begin(PlayerControl killer, GameData.PlayerInfo victim)
	{
		if (killerParts != null)
		{
			GameData.PlayerInfo data = killer.Data;
			for (int i = 0; i < killerParts.Length; i++)
			{
				Renderer renderer = killerParts[i];
				PlayerControl.SetPlayerMaterialColors(data.ColorId, renderer);
				if (SetHatNormal(ref renderer, data.HatId)) continue;
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
		UpdateCustomVisual(killer, victim);
		victimHat = victim.HatId;
		for (int j = 0; j < victimParts.Length; j++)
		{
			Renderer renderer2 = victimParts[j];
			PlayerControl.SetPlayerMaterialColors(victim.ColorId, renderer2);
			if (SetHatNormal(ref renderer2, victimHat)) continue;
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
			else if (renderer.name.StartsWith("ExtHatSlot1"))
            {
				((SpriteRenderer)renderer).sprite = DestroyableSingleton<HatManager>.Instance.GetHatById(victimHat).FloorImageExt;
			}
			else if (renderer.name.StartsWith("ExtHatSlot2"))
			{
				((SpriteRenderer)renderer).sprite = DestroyableSingleton<HatManager>.Instance.GetHatById(victimHat).FloorImageExt2;
			}
			else if (renderer.name.StartsWith("ExtHatSlot3"))
			{
				((SpriteRenderer)renderer).sprite = DestroyableSingleton<HatManager>.Instance.GetHatById(victimHat).FloorImageExt3;
			}
			else if (renderer.name.StartsWith("ExtHatSlot4"))
			{
				((SpriteRenderer)renderer).sprite = DestroyableSingleton<HatManager>.Instance.GetHatById(victimHat).FloorImageExt4;
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
	public bool SetHatNormal(ref Renderer renderer, uint hatID)
	{
		if (renderer.name.StartsWith("HatSlot"))
		{
			PlayerControl.SetHatImage(hatID, (SpriteRenderer)renderer);
			return true;
		}
		else if (renderer.name.StartsWith("ExtHatSlot1"))
		{
			PlayerControl.SetHatImage(hatID, (SpriteRenderer)renderer, 1);
			return true;
		}
		else if (renderer.name.StartsWith("ExtHatSlot2"))
		{
			PlayerControl.SetHatImage(hatID, (SpriteRenderer)renderer, 2);
			return true;
		}
		else if (renderer.name.StartsWith("ExtHatSlot3"))
		{
			PlayerControl.SetHatImage(hatID, (SpriteRenderer)renderer, 3);
			return true;
		}
		else if (renderer.name.StartsWith("ExtHatSlot4"))
		{
			PlayerControl.SetHatImage(hatID, (SpriteRenderer)renderer, 4);
			return true;
		}
		else return false;
	}
	public void UpdateMultiHat()
    {
		SpriteRenderer victimSource = null;
		SpriteRenderer killerSource = null;
		for (int j = 0; j < victimParts.Length; j++)
		{
			Renderer renderer = victimParts[j];
			if (renderer.name.StartsWith("HatSlot"))
			{
				victimSource = (SpriteRenderer)renderer;
			}
			else if (renderer.name.StartsWith("ExtHatSlot1") && victimSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), victimSource);
			}
			else if (renderer.name.StartsWith("ExtHatSlot2") && victimSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), victimSource);
			}
			else if (renderer.name.StartsWith("ExtHatSlot3") && victimSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), victimSource);
			}
			else if (renderer.name.StartsWith("ExtHatSlot4") && victimSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), victimSource);
			}
		}
		for (int i = 0; i < killerParts.Length; i++)
		{
			Renderer renderer = killerParts[i];
			if (renderer.name.StartsWith("HatSlot"))
			{
				killerSource = (SpriteRenderer)renderer;
			}
			else if (renderer.name.StartsWith("ExtHatSlot1") && killerSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), killerSource);
			}
			else if (renderer.name.StartsWith("ExtHatSlot2") && killerSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), killerSource);
			}
			else if (renderer.name.StartsWith("ExtHatSlot3") && killerSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), killerSource);
			}
			else if (renderer.name.StartsWith("ExtHatSlot4") && killerSource)
			{
				CE_WardrobeManager.UpdateMultiHat(((SpriteRenderer)renderer), killerSource);
			}
		}
	}
	private void UpdateCustomVisual(PlayerControl killer, GameData.PlayerInfo victim)
	{
		if (!killer || victim == null) return;

		LastSkinData_Victim = DestroyableSingleton<HatManager>.Instance.GetSkinById(victim.SkinId);
		LastSkinData_Killer = DestroyableSingleton<HatManager>.Instance.GetSkinById(killer.Data.SkinId);
	}
	private void InitCustomHatLayers()
    {
		SpriteRenderer VictimRef = null;
		for (int i = 0; i < victimParts.Length; i++)
		{
			Renderer renderer = victimParts[i];
			if (renderer.name.StartsWith("HatSlot"))
			{
				VictimRef = ((SpriteRenderer)renderer);
			}
		}
		if (VictimRef != null)
		{
			var list = victimParts.ToList();
			for (int i = 0; i < 4; i++)
			{
				list.Add(CE_WardrobeManager.CreateExtraHatOverlayKill(VictimRef, i));
			}
			victimParts = list.ToArray();
		}

		SpriteRenderer KillerRef = null;
		for (int i = 0; i < killerParts.Length; i++)
		{
			Renderer renderer = killerParts[i];
			if (renderer.name.StartsWith("HatSlot"))
			{
				KillerRef = ((SpriteRenderer)renderer);
			}
		}
		if (KillerRef != null)
		{
			var list = killerParts.ToList();
			for (int i = 0; i < 4; i++)
			{
				list.Add(CE_WardrobeManager.CreateExtraHatOverlayKill(KillerRef, i));
			}
			killerParts = list.ToArray();
		}
	}
	private void LateUpdate()
	{
		if (!LastSkinData_Killer || !LastSkinData_Victim) return;

		SpriteAnim[] anims = GetComponentsInChildren<SpriteAnim>();
		for (int i = 0; i < anims.Length; i++)
		{
			anims[i].Speed = CE_WardrobeManager.AnimationEditor_CurrentSpeed;
			anims[i].Paused = CE_WardrobeManager.AnimationEditor_Paused;
		}

		UpdateMultiHat();


		if (LastSkinData_Killer.isCustom)
		{
			for (int i = 0; i < killerParts.Length; i++)
			{
				Renderer renderer = killerParts[i];
				if (renderer.name.StartsWith("Skin"))
				{
					CE_WardrobeManager.LogPivot(renderer);
					var sprite = CE_WardrobeManager.GetSkin(renderer.GetComponent<SpriteRenderer>().sprite.name, LastSkinData_Killer);
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
					CE_WardrobeManager.LogPivot(renderer);
					var sprite = CE_WardrobeManager.GetSkin(renderer.GetComponent<SpriteRenderer>().sprite.name, LastSkinData_Victim);
					if (sprite) renderer.GetComponent<SpriteRenderer>().sprite = sprite;
				}
			}

		}
	}
}
