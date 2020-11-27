using System;
using UnityEngine;

// Token: 0x02000051 RID: 81
[CreateAssetMenu]
public class SoundGroup : ScriptableObject
{
	// Token: 0x060001DA RID: 474 RVA: 0x0000327E File Offset: 0x0000147E
	public AudioClip Random()
	{
		return this.Clips.Random<AudioClip>();
	}

	// Token: 0x040001BC RID: 444
	public AudioClip[] Clips;
}
