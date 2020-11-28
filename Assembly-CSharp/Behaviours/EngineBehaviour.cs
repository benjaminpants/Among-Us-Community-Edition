using System;
using UnityEngine;

// Token: 0x02000089 RID: 137
public class EngineBehaviour : MonoBehaviour
{
	// Token: 0x060002E4 RID: 740 RVA: 0x00003E62 File Offset: 0x00002062
	public void PlayElectricSound()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlayDynamicSound("EngineShock" + base.name, this.ElectricSound, false, new DynamicSound.GetDynamicsFunction(this.GetSoundDistance), false);
		}
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x000161CC File Offset: 0x000143CC
	public void PlaySteamSound()
	{
		if (Constants.ShouldPlaySfx())
		{
			float pitch = FloatRange.Next(0.7f, 1.1f);
			SoundManager.Instance.PlayDynamicSound("EngineSteam" + base.name, this.SteamSound, false, delegate(AudioSource p, float d)
			{
				this.GetSoundDistance(p, d, pitch);
			}, false);
		}
	}

	// Token: 0x060002E6 RID: 742 RVA: 0x00003E99 File Offset: 0x00002099
	private void GetSoundDistance(AudioSource player, float dt)
	{
		this.GetSoundDistance(player, dt, 1f);
	}

	// Token: 0x060002E7 RID: 743 RVA: 0x00016230 File Offset: 0x00014430
	private void GetSoundDistance(AudioSource player, float dt, float pitch)
	{
		float num = 1f;
		if (PlayerControl.LocalPlayer)
		{
			float num2 = Vector2.Distance(base.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
			num = 1f - num2 / this.SoundDistance;
		}
		player.volume = num * 0.8f;
		player.pitch = pitch;
	}

	// Token: 0x040002E2 RID: 738
	public AudioClip ElectricSound;

	// Token: 0x040002E3 RID: 739
	public AudioClip SteamSound;

	// Token: 0x040002E4 RID: 740
	public float SoundDistance = 5f;
}
