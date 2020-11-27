using System;
using UnityEngine;

// Token: 0x020000CA RID: 202
public class DemoKeyboardStick : VirtualJoystick
{
	// Token: 0x0600044D RID: 1101 RVA: 0x00002265 File Offset: 0x00000465
	protected override void FixedUpdate()
	{
	}

	// Token: 0x0600044E RID: 1102 RVA: 0x0001A67C File Offset: 0x0001887C
	public override void UpdateJoystick(FingerBehaviour finger, Vector2 velocity, bool syncFinger)
	{
		this.UpKey.enabled = (velocity.y > 0.1f);
		this.DownKey.enabled = (velocity.y < -0.1f);
		this.RightKey.enabled = (velocity.x > 0.1f);
		this.LeftKey.enabled = (velocity.x < -0.1f);
	}

	// Token: 0x0400043B RID: 1083
	public SpriteRenderer UpKey;

	// Token: 0x0400043C RID: 1084
	public SpriteRenderer DownKey;

	// Token: 0x0400043D RID: 1085
	public SpriteRenderer LeftKey;

	// Token: 0x0400043E RID: 1086
	public SpriteRenderer RightKey;
}
