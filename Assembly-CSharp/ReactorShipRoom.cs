using System;
using PowerTools;
using UnityEngine;

// Token: 0x020001E1 RID: 481
public class ReactorShipRoom : ShipRoom
{
	// Token: 0x06000A5D RID: 2653 RVA: 0x0003562C File Offset: 0x0003382C
	public void StartMeltdown()
	{
		DestroyableSingleton<HudManager>.Instance.StartReactorFlash();
		this.Manifolds.sprite = this.meltdownManifolds;
		this.Reactor.Play(this.meltdownReactor, 1f);
		this.HighFloor.Play(this.meltdownHighFloor, 1f);
		this.MidFloor1.Play(this.meltdownMidFloor, 1f);
		this.MidFloor2.Play(this.meltdownMidFloor, 1f);
		this.LowFloor.Play(this.meltdownLowFloor, 1f);
		for (int i = 0; i < this.Pipes.Length; i++)
		{
			this.Pipes[i].Play(this.meltdownPipes[i], 1f);
		}
	}

	// Token: 0x06000A5E RID: 2654 RVA: 0x000356F0 File Offset: 0x000338F0
	public void StopMeltdown()
	{
		DestroyableSingleton<HudManager>.Instance.StopReactorFlash();
		this.Manifolds.sprite = this.normalManifolds;
		this.Reactor.Play(this.normalReactor, 1f);
		this.HighFloor.Play(this.normalHighFloor, 1f);
		this.MidFloor1.Play(this.normalMidFloor, 1f);
		this.MidFloor2.Play(this.normalMidFloor, 1f);
		this.LowFloor.Play(this.normalLowFloor, 1f);
		for (int i = 0; i < this.Pipes.Length; i++)
		{
			this.Pipes[i].Play(this.normalPipes[i], 1f);
		}
	}

	// Token: 0x040009F5 RID: 2549
	public Sprite normalManifolds;

	// Token: 0x040009F6 RID: 2550
	public Sprite meltdownManifolds;

	// Token: 0x040009F7 RID: 2551
	public SpriteRenderer Manifolds;

	// Token: 0x040009F8 RID: 2552
	public AnimationClip normalReactor;

	// Token: 0x040009F9 RID: 2553
	public AnimationClip meltdownReactor;

	// Token: 0x040009FA RID: 2554
	public SpriteAnim Reactor;

	// Token: 0x040009FB RID: 2555
	public AnimationClip normalHighFloor;

	// Token: 0x040009FC RID: 2556
	public AnimationClip meltdownHighFloor;

	// Token: 0x040009FD RID: 2557
	public SpriteAnim HighFloor;

	// Token: 0x040009FE RID: 2558
	public AnimationClip normalMidFloor;

	// Token: 0x040009FF RID: 2559
	public AnimationClip meltdownMidFloor;

	// Token: 0x04000A00 RID: 2560
	public SpriteAnim MidFloor1;

	// Token: 0x04000A01 RID: 2561
	public SpriteAnim MidFloor2;

	// Token: 0x04000A02 RID: 2562
	public AnimationClip normalLowFloor;

	// Token: 0x04000A03 RID: 2563
	public AnimationClip meltdownLowFloor;

	// Token: 0x04000A04 RID: 2564
	public SpriteAnim LowFloor;

	// Token: 0x04000A05 RID: 2565
	public AnimationClip[] normalPipes;

	// Token: 0x04000A06 RID: 2566
	public AnimationClip[] meltdownPipes;

	// Token: 0x04000A07 RID: 2567
	public SpriteAnim[] Pipes;
}
