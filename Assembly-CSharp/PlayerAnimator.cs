using System;
using System.Collections;
using PowerTools;
using UnityEngine;

// Token: 0x020000AA RID: 170
public class PlayerAnimator : MonoBehaviour
{
	// Token: 0x06000391 RID: 913 RVA: 0x00018288 File Offset: 0x00016488
	private void Start()
	{
		this.Animator = base.GetComponent<SpriteAnim>();
		this.rend = base.GetComponent<SpriteRenderer>();
		this.rend.material.SetColor("_BackColor", Palette.ShadowColors[0]);
		this.rend.material.SetColor("_BodyColor", Palette.PlayerColors[0]);
		this.rend.material.SetColor("_VisorColor", Palette.VisorColor);
	}

	// Token: 0x06000392 RID: 914 RVA: 0x000044FD File Offset: 0x000026FD
	public void FixedUpdate()
	{
		base.transform.Translate(this.velocity * Time.fixedDeltaTime);
		this.UseButton.enabled = (this.NearbyConsoles > 0);
	}

	// Token: 0x06000393 RID: 915 RVA: 0x00018318 File Offset: 0x00016518
	public void LateUpdate()
	{
		if (this.velocity.sqrMagnitude >= 0.1f)
		{
			if (this.Animator.GetCurrentAnimation() != this.RunAnim)
			{
				this.Animator.Play(this.RunAnim, 1f);
			}
			this.rend.flipX = (this.velocity.x < 0f);
			return;
		}
		if (this.Animator.GetCurrentAnimation() == this.RunAnim)
		{
			this.Animator.Play(this.IdleAnim, 1f);
		}
	}

	// Token: 0x06000394 RID: 916 RVA: 0x00004533 File Offset: 0x00002733
	public IEnumerator WalkPlayerTo(Vector2 worldPos, bool relax, float tolerance = 0.01f)
	{
		worldPos.y += 0.3636f;
		if (!(this.joystick is DemoKeyboardStick))
		{
			this.finger.ClickOn();
		}
		for (;;)
		{
			Vector2 vector2;
			Vector2 vector = vector2 = worldPos - (Vector2)base.transform.position;
			if (vector2.sqrMagnitude <= tolerance)
			{
				break;
			}
			float d = Mathf.Clamp(vector.magnitude * 2f, 0.01f, 1f);
			this.velocity = vector.normalized * this.Speed * d;
			this.joystick.UpdateJoystick(this.finger, this.velocity, true);
			yield return null;
		}
		if (relax)
		{
			this.finger.ClickOff();
			this.velocity = Vector2.zero;
			this.joystick.UpdateJoystick(this.finger, this.velocity, false);
		}
		yield break;
	}

	// Token: 0x0400037B RID: 891
	public float Speed = 2.5f;

	// Token: 0x0400037C RID: 892
	public VirtualJoystick joystick;

	// Token: 0x0400037D RID: 893
	public SpriteRenderer UseButton;

	// Token: 0x0400037E RID: 894
	public FingerBehaviour finger;

	// Token: 0x0400037F RID: 895
	public AnimationClip RunAnim;

	// Token: 0x04000380 RID: 896
	public AnimationClip IdleAnim;

	// Token: 0x04000381 RID: 897
	private Vector2 velocity;

	// Token: 0x04000382 RID: 898
	[HideInInspector]
	private SpriteAnim Animator;

	// Token: 0x04000383 RID: 899
	[HideInInspector]
	private SpriteRenderer rend;

	// Token: 0x04000384 RID: 900
	public int NearbyConsoles;
}
