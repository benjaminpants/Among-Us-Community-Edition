using System;
using UnityEngine;

// Token: 0x020001FF RID: 511
public class SoundStarter : MonoBehaviour
{
	// Token: 0x06000B07 RID: 2823 RVA: 0x00008982 File Offset: 0x00006B82
	public void Awake()
	{
		if (this.StopAll)
		{
			SoundManager.Instance.StopAllSound();
		}
		SoundManager.Instance.CrossFadeSound(this.Name, this.SoundToPlay, this.Volume, 1.5f);
	}

	// Token: 0x04000AA4 RID: 2724
	public string Name;

	// Token: 0x04000AA5 RID: 2725
	public AudioClip SoundToPlay;

	// Token: 0x04000AA6 RID: 2726
	public bool StopAll;

	// Token: 0x04000AA7 RID: 2727
	[Range(0f, 1f)]
	public float Volume = 1f;
}
