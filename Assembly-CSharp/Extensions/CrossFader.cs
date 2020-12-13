using UnityEngine;

public class CrossFader : ISoundPlayer
{
	public float MaxVolume = 1f;

	public AudioClip target;

	public float Duration = 1.5f;

	private float timer;

	private bool didSwitch;

	public string Name
	{
		get;
		set;
	}

	public AudioSource Player
	{
		get;
		set;
	}

	public void Update(float dt)
	{
		if (!(timer < Duration))
		{
			return;
		}
		timer += dt;
		float num = timer / Duration;
		if (num < 0.5f)
		{
			Player.volume = (1f - num * 2f) * MaxVolume;
			return;
		}
		if (!didSwitch)
		{
			didSwitch = true;
			Player.Stop();
			Player.clip = target;
			if ((bool)target)
			{
				Player.Play();
			}
		}
		Player.volume = (num - 0.5f) * 2f * MaxVolume;
	}

	public void SetTarget(AudioClip clip)
	{
		if (!Player.clip)
		{
			didSwitch = false;
			Player.volume = 0f;
			timer = 0.5f;
		}
		else
		{
			if (Player.clip == clip)
			{
				return;
			}
			if (didSwitch)
			{
				didSwitch = false;
				if (timer >= Duration)
				{
					timer = 0f;
				}
				else
				{
					timer = Duration - timer;
				}
			}
		}
		target = clip;
	}
}
