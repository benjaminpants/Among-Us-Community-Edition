using System;
using PowerTools;
using UnityEngine;

// Token: 0x020001F1 RID: 497
public class SurvCamera : MonoBehaviour
{
	// Token: 0x06000ABD RID: 2749 RVA: 0x000086C6 File Offset: 0x000068C6
	public void Start()
	{
		this.Image = base.GetComponent<SpriteAnim>();
	}

	// Token: 0x06000ABE RID: 2750 RVA: 0x000086D4 File Offset: 0x000068D4
	public void SetAnimation(bool on)
	{
		this.Image.Play(on ? this.OnAnim : this.OffAnim, 1f);
	}

	// Token: 0x04000A58 RID: 2648
	public SpriteAnim Image;

	// Token: 0x04000A59 RID: 2649
	public float CamSize = 3f;

	// Token: 0x04000A5A RID: 2650
	public float CamAspect = 1f;

	// Token: 0x04000A5B RID: 2651
	public Vector3 Offset;

	// Token: 0x04000A5C RID: 2652
	public AnimationClip OnAnim;

	// Token: 0x04000A5D RID: 2653
	public AnimationClip OffAnim;
}
