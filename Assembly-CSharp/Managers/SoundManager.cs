using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Token: 0x020001FE RID: 510
public class SoundManager : MonoBehaviour
{
	// Token: 0x170001A4 RID: 420
	// (get) Token: 0x06000AF8 RID: 2808 RVA: 0x000088C8 File Offset: 0x00006AC8
	public static SoundManager Instance
	{
		get
		{
			if (!SoundManager._Instance)
			{
				SoundManager._Instance = (UnityEngine.Object.FindObjectOfType<SoundManager>() ?? new GameObject("SoundManager").AddComponent<SoundManager>());
			}
			return SoundManager._Instance;
		}
	}

	// Token: 0x06000AF9 RID: 2809 RVA: 0x000088F8 File Offset: 0x00006AF8
	public void Start()
	{
		if (SoundManager._Instance && SoundManager._Instance != this)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		SoundManager._Instance = this;
		this.UpdateVolume();
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
	}

	// Token: 0x06000AFA RID: 2810 RVA: 0x000372FC File Offset: 0x000354FC
	public void Update()
	{
		for (int i = 0; i < this.soundPlayers.Count; i++)
		{
			this.soundPlayers[i].Update(Time.deltaTime);
		}
	}

	// Token: 0x06000AFB RID: 2811 RVA: 0x00008936 File Offset: 0x00006B36
	private void UpdateVolume()
	{
		this.ChangeSfxVolume(SaveManager.SfxVolume);
		this.ChangeMusicVolume(SaveManager.MusicVolume);
	}

	// Token: 0x06000AFC RID: 2812 RVA: 0x00037338 File Offset: 0x00035538
	public void ChangeSfxVolume(float volume)
	{
		if (volume <= 0f)
		{
			SoundManager.SfxVolume = -80f;
		}
		else
		{
			SoundManager.SfxVolume = Mathf.Log10(volume) * 20f;
		}
		this.musicMixer.audioMixer.SetFloat("SfxVolume", SoundManager.SfxVolume);
	}

	// Token: 0x06000AFD RID: 2813 RVA: 0x00037388 File Offset: 0x00035588
	public void ChangeMusicVolume(float volume)
	{
		if (volume <= 0f)
		{
			SoundManager.MusicVolume = -80f;
		}
		else
		{
			SoundManager.MusicVolume = Mathf.Log10(volume) * 20f;
		}
		this.musicMixer.audioMixer.SetFloat("MusicVolume", SoundManager.MusicVolume);
	}

	// Token: 0x06000AFE RID: 2814 RVA: 0x000373D8 File Offset: 0x000355D8
	public void StopSound(AudioClip clip)
	{
		AudioSource audioSource;
		if (this.allSources.TryGetValue(clip, out audioSource))
		{
			this.allSources.Remove(clip);
			audioSource.Stop();
			UnityEngine.Object.Destroy(audioSource);
		}
		for (int i = 0; i < this.soundPlayers.Count; i++)
		{
			ISoundPlayer soundPlayer = this.soundPlayers[i];
			if (soundPlayer.Player.clip == clip)
			{
				UnityEngine.Object.Destroy(soundPlayer.Player);
				this.soundPlayers.RemoveAt(i);
				return;
			}
		}
	}

	// Token: 0x06000AFF RID: 2815 RVA: 0x0003745C File Offset: 0x0003565C
	public void StopAllSound()
	{
		for (int i = 0; i < this.soundPlayers.Count; i++)
		{
			UnityEngine.Object.Destroy(this.soundPlayers[i].Player);
		}
		this.soundPlayers.Clear();
		foreach (KeyValuePair<AudioClip, AudioSource> keyValuePair in this.allSources)
		{
			AudioSource value = keyValuePair.Value;
			value.volume = 0f;
			value.Stop();
			UnityEngine.Object.Destroy(keyValuePair.Value);
		}
		this.allSources.Clear();
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00037510 File Offset: 0x00035710
	public void PlayDynamicSound(string name, AudioClip clip, bool loop, DynamicSound.GetDynamicsFunction volumeFunc, bool playAsSfx = false)
	{
		DynamicSound dynamicSound = null;
		for (int i = 0; i < this.soundPlayers.Count; i++)
		{
			ISoundPlayer soundPlayer = this.soundPlayers[i];
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
			dynamicSound.Player.outputAudioMixerGroup = ((loop && !playAsSfx) ? this.musicMixer : this.sfxMixer);
			dynamicSound.Player.playOnAwake = false;
			this.soundPlayers.Add(dynamicSound);
		}
		dynamicSound.Player.loop = loop;
		dynamicSound.SetTarget(clip, volumeFunc);
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x000375D0 File Offset: 0x000357D0
	public void CrossFadeSound(string name, AudioClip clip, float maxVolume, float duration = 1.5f)
	{
		CrossFader crossFader = null;
		for (int i = 0; i < this.soundPlayers.Count; i++)
		{
			ISoundPlayer soundPlayer = this.soundPlayers[i];
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
			crossFader.Player.outputAudioMixerGroup = this.musicMixer;
			crossFader.Player.playOnAwake = false;
			crossFader.Player.loop = true;
			this.soundPlayers.Add(crossFader);
		}
		crossFader.SetTarget(clip);
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x00037688 File Offset: 0x00035888
	public AudioSource PlaySoundImmediate(AudioClip clip, bool loop, float volume = 1f, float pitch = 1f)
	{
		if (clip == null)
		{
			Debug.LogWarning("Missing audio clip");
			return null;
		}
		AudioSource audioSource;
		if (this.allSources.TryGetValue(clip, out audioSource))
		{
			audioSource.pitch = pitch;
			audioSource.loop = loop;
			audioSource.Play();
		}
		else
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = (loop ? this.musicMixer : this.sfxMixer);
			audioSource.playOnAwake = false;
			audioSource.volume = volume;
			audioSource.pitch = pitch;
			audioSource.loop = loop;
			audioSource.clip = clip;
			audioSource.Play();
			this.allSources.Add(clip, audioSource);
		}
		return audioSource;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x0003772C File Offset: 0x0003592C
	public bool SoundIsPlaying(AudioClip clip)
	{
		AudioSource audioSource;
		return this.allSources.TryGetValue(clip, out audioSource) && !audioSource.isPlaying;
	}

	// Token: 0x06000B04 RID: 2820 RVA: 0x00037754 File Offset: 0x00035954
	public AudioSource PlaySound(AudioClip clip, bool loop, float volume = 1f)
	{
		if (clip == null)
		{
			Debug.LogWarning("Missing audio clip");
			return null;
		}
		AudioSource audioSource;
		if (this.allSources.TryGetValue(clip, out audioSource))
		{
			if (!audioSource.isPlaying)
			{
				audioSource.loop = loop;
				audioSource.Play();
			}
		}
		else
		{
			audioSource = base.gameObject.AddComponent<AudioSource>();
			audioSource.outputAudioMixerGroup = (loop ? this.musicMixer : this.sfxMixer);
			audioSource.playOnAwake = false;
			audioSource.volume = volume;
			audioSource.loop = loop;
			audioSource.clip = clip;
			audioSource.Play();
			this.allSources.Add(clip, audioSource);
		}
		return audioSource;
	}

	// Token: 0x04000A9D RID: 2717
	private static SoundManager _Instance;

	// Token: 0x04000A9E RID: 2718
	public AudioMixerGroup musicMixer;

	// Token: 0x04000A9F RID: 2719
	public AudioMixerGroup sfxMixer;

	// Token: 0x04000AA0 RID: 2720
	public static float MusicVolume = 1f;

	// Token: 0x04000AA1 RID: 2721
	public static float SfxVolume = 1f;

	// Token: 0x04000AA2 RID: 2722
	private Dictionary<AudioClip, AudioSource> allSources = new Dictionary<AudioClip, AudioSource>();

	// Token: 0x04000AA3 RID: 2723
	private List<ISoundPlayer> soundPlayers = new List<ISoundPlayer>();
}
