using UnityEngine;

public class SoundStarter : MonoBehaviour
{
	public string Name;

	public AudioClip SoundToPlay;

	public bool StopAll;

	[Range(0f, 1f)]
	public float Volume = 1f;

	public void Awake()
	{
		if (StopAll)
		{
			SoundManager.Instance.StopAllSound();
		}
		SoundManager.Instance.CrossFadeSound(Name, SoundToPlay, Volume);
	}
}
