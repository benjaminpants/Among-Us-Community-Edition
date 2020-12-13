using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
	private static SoundManager _Instance;

	public AudioMixerGroup musicMixer;

	public AudioMixerGroup sfxMixer;

	public static float MusicVolume = 1f;

	public static float SfxVolume = 1f;

	private Dictionary<AudioClip, AudioSource> allSources = new Dictionary<AudioClip, AudioSource>();

	private List<ISoundPlayer> soundPlayers = new List<ISoundPlayer>();

	public static SoundManager Instance
	{
		get
		{
			if (!_Instance)
			{
				_Instance = Object.FindObjectOfType<SoundManager>() ?? new GameObject("SoundManager").AddComponent<SoundManager>();
			}
			return _Instance;
		}
	}

	public void Start()
	{
		if ((bool)_Instance && _Instance != this)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		_Instance = this;
		UpdateVolume();
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public void Update()
	{
		for (int i = 0; i < soundPlayers.Count; i++)
		{
			soundPlayers[i].Update(Time.deltaTime);
		}
	}

	private void UpdateVolume()
	{
		ChangeSfxVolume(SaveManager.SfxVolume);
		ChangeMusicVolume(SaveManager.MusicVolume);
	}

	public void ChangeSfxVolume(float volume)
	{
		if (volume <= 0f)
		{
			SfxVolume = -80f;
		}
		else
		{
			SfxVolume = Mathf.Log10(volume) * 20f;
		}
		musicMixer.audioMixer.SetFloat("SfxVolume", SfxVolume);
	}

	public void ChangeMusicVolume(float volume)
	{
		if (volume <= 0f)
		{
			MusicVolume = -80f;
		}
		else
		{
			MusicVolume = Mathf.Log10(volume) * 20f;
		}
		musicMixer.audioMixer.SetFloat("MusicVolume", MusicVolume);
	}

	public void StopSound(AudioClip clip)
	{
		if (allSources.TryGetValue(clip, out var value))
		{
			allSources.Remove(clip);
			value.Stop();
			Object.Destroy(value);
		}
		for (int i = 0; i < soundPlayers.Count; i++)
		{
			ISoundPlayer soundPlayer = soundPlayers[i];
			if (soundPlayer.Player.clip == clip)
			{
				Object.Destroy(soundPlayer.Player);
				soundPlayers.RemoveAt(i);
				break;
			}
		}
	}

	public void StopAllSound()
	{
		for (int i = 0; i < soundPlayers.Count; i++)
		{
			Object.Destroy(soundPlayers[i].Player);
		}
		soundPlayers.Clear();
		foreach (KeyValuePair<AudioClip, AudioSource> allSource in allSources)
		{
			AudioSource value = allSource.Value;
			value.volume = 0f;
			value.Stop();
			Object.Destroy(allSource.Value);
		}
		allSources.Clear();
	}

	public void PlayDynamicSound(string name, AudioClip clip, bool loop, DynamicSound.GetDynamicsFunction volumeFunc, bool playAsSfx = false)
	{
		DynamicSound dynamicSound = null;
		for (int i = 0; i < soundPlayers.Count; i++)
		{
			ISoundPlayer soundPlayer = soundPlayers[i];
			if (soundPlayer.Name == name && soundPlayer is DynamicSound)
			{
				dynamicSound = (DynamicSound)soundPlayer;
				break;
			}
		}
		if (dynamicSound == null)
		{
			dynamicSound = new DynamicSound();
			dynamicSound.Name = name;
			dynamicSound.Player = base.gameObject.AddComponent<AudioSource>();
			dynamicSound.Player.outputAudioMixerGroup = ((loop && !playAsSfx) ? musicMixer : sfxMixer);
			dynamicSound.Player.playOnAwake = false;
			soundPlayers.Add(dynamicSound);
		}
		dynamicSound.Player.loop = loop;
		dynamicSound.SetTarget(clip, volumeFunc);
	}

	public void CrossFadeSound(string name, AudioClip clip, float maxVolume, float duration = 1.5f)
	{
		CrossFader crossFader = null;
		for (int i = 0; i < soundPlayers.Count; i++)
		{
			ISoundPlayer soundPlayer = soundPlayers[i];
			if (soundPlayer.Name == name && soundPlayer is CrossFader)
			{
				crossFader = (CrossFader)soundPlayer;
				break;
			}
		}
		if (crossFader == null)
		{
			crossFader = new CrossFader();
			crossFader.Name = name;
			crossFader.MaxVolume = maxVolume;
			crossFader.Player = base.gameObject.AddComponent<AudioSource>();
			crossFader.Player.outputAudioMixerGroup = musicMixer;
			crossFader.Player.playOnAwake = false;
			crossFader.Player.loop = true;
			soundPlayers.Add(crossFader);
		}
		crossFader.SetTarget(clip);
	}

	public AudioSource PlaySoundImmediate(AudioClip clip, bool loop, float volume = 1f, float pitch = 1f)
	{
		if (clip == null)
		{
			Debug.LogWarning("Missing audio clip");
			return null;
		}
		if (allSources.TryGetValue(clip, out var value))
		{
			value.pitch = pitch;
			value.loop = loop;
			value.Play();
		}
		else
		{
			value = base.gameObject.AddComponent<AudioSource>();
			value.outputAudioMixerGroup = (loop ? musicMixer : sfxMixer);
			value.playOnAwake = false;
			value.volume = volume;
			value.pitch = pitch;
			value.loop = loop;
			value.clip = clip;
			value.Play();
			allSources.Add(clip, value);
		}
		return value;
	}

	public bool SoundIsPlaying(AudioClip clip)
	{
		if (allSources.TryGetValue(clip, out var value))
		{
			return !value.isPlaying;
		}
		return false;
	}

	public AudioSource PlaySound(AudioClip clip, bool loop, float volume = 1f)
	{
		if (clip == null)
		{
			Debug.LogWarning("Missing audio clip");
			return null;
		}
		if (allSources.TryGetValue(clip, out var value))
		{
			if (!value.isPlaying)
			{
				value.loop = loop;
				value.Play();
			}
		}
		else
		{
			value = base.gameObject.AddComponent<AudioSource>();
			value.outputAudioMixerGroup = (loop ? musicMixer : sfxMixer);
			value.playOnAwake = false;
			value.volume = volume;
			value.loop = loop;
			value.clip = clip;
			value.Play();
			allSources.Add(clip, value);
		}
		return value;
	}
}
