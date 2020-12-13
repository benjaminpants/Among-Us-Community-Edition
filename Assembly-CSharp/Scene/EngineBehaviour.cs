using UnityEngine;

public class EngineBehaviour : MonoBehaviour
{
	public AudioClip ElectricSound;

	public AudioClip SteamSound;

	public float SoundDistance = 5f;

	public void PlayElectricSound()
	{
		if (Constants.ShouldPlaySfx())
		{
			SoundManager.Instance.PlayDynamicSound("EngineShock" + base.name, ElectricSound, loop: false, GetSoundDistance);
		}
	}

	public void PlaySteamSound()
	{
		if (Constants.ShouldPlaySfx())
		{
			float pitch = FloatRange.Next(0.7f, 1.1f);
			SoundManager.Instance.PlayDynamicSound("EngineSteam" + base.name, SteamSound, loop: false, delegate(AudioSource p, float d)
			{
				GetSoundDistance(p, d, pitch);
			});
		}
	}

	private void GetSoundDistance(AudioSource player, float dt)
	{
		GetSoundDistance(player, dt, 1f);
	}

	private void GetSoundDistance(AudioSource player, float dt, float pitch)
	{
		float num = 1f;
		if ((bool)PlayerControl.LocalPlayer)
		{
			float num2 = Vector2.Distance(base.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
			num = 1f - num2 / SoundDistance;
		}
		player.volume = num * 0.8f;
		player.pitch = pitch;
	}
}
