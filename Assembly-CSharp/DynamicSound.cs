using System;
using UnityEngine;

// Token: 0x020001FC RID: 508
public class DynamicSound : ISoundPlayer
{
	// Token: 0x170001A2 RID: 418
	// (get) Token: 0x06000AED RID: 2797 RVA: 0x0000885C File Offset: 0x00006A5C
	// (set) Token: 0x06000AEE RID: 2798 RVA: 0x00008864 File Offset: 0x00006A64
	public string Name { get; set; }

	// Token: 0x170001A3 RID: 419
	// (get) Token: 0x06000AEF RID: 2799 RVA: 0x0000886D File Offset: 0x00006A6D
	// (set) Token: 0x06000AF0 RID: 2800 RVA: 0x00008875 File Offset: 0x00006A75
	public AudioSource Player { get; set; }

	// Token: 0x06000AF1 RID: 2801 RVA: 0x0000887E File Offset: 0x00006A7E
	public void Update(float dt)
	{
		this.volumeFunc(this.Player, dt);
	}

	// Token: 0x06000AF2 RID: 2802 RVA: 0x00008892 File Offset: 0x00006A92
	public void SetTarget(AudioClip clip, DynamicSound.GetDynamicsFunction volumeFunc)
	{
		this.volumeFunc = volumeFunc;
		this.Player.clip = clip;
		this.volumeFunc(this.Player, 1f);
		this.Player.Play();
	}

	// Token: 0x04000A9C RID: 2716
	public DynamicSound.GetDynamicsFunction volumeFunc;

	// Token: 0x020001FD RID: 509
	// (Invoke) Token: 0x06000AF5 RID: 2805
	public delegate void GetDynamicsFunction(AudioSource source, float dt);
}
