using System;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class VirtualJoystick : MonoBehaviour, IVirtualJoystick
{
	// Token: 0x170000B8 RID: 184
	// (get) Token: 0x06000458 RID: 1112 RVA: 0x00004C68 File Offset: 0x00002E68
	// (set) Token: 0x06000459 RID: 1113 RVA: 0x00004C70 File Offset: 0x00002E70
	public Vector2 Delta { get; private set; }

	// Token: 0x0600045A RID: 1114 RVA: 0x0001A9F4 File Offset: 0x00018BF4
	protected virtual void FixedUpdate()
	{
		this.myController.Update();
		DragState dragState = this.myController.CheckDrag(this.Outer, false);
		if (dragState - DragState.TouchStart <= 1)
		{
			float maxLength = this.OuterRadius - this.InnerRadius;
			Vector2 vector = this.myController.DragPosition - (Vector2)base.transform.position;
			float magnitude = vector.magnitude;
			Vector2 a = new Vector2(Mathf.Sqrt(Mathf.Abs(vector.x)) * Mathf.Sign(vector.x), Mathf.Sqrt(Mathf.Abs(vector.y)) * Mathf.Sign(vector.y));
			this.Delta = Vector2.ClampMagnitude(a / this.OuterRadius, 1f);
			this.Inner.transform.localPosition = Vector3.ClampMagnitude(vector, maxLength) + Vector3.back;
			return;
		}
		if (dragState != DragState.Released)
		{
			return;
		}
		this.Delta = Vector2.zero;
		this.Inner.transform.localPosition = Vector3.back;
	}

	// Token: 0x0600045B RID: 1115 RVA: 0x0001AB08 File Offset: 0x00018D08
	public virtual void UpdateJoystick(FingerBehaviour finger, Vector2 velocity, bool syncFinger)
	{
		Vector3 vector = this.Inner.transform.localPosition;
		Vector3 vector2 = velocity.normalized * this.InnerRadius;
		vector2.z = vector.z;
		if (syncFinger)
		{
			vector = Vector3.Lerp(vector, vector2, Time.fixedDeltaTime * 5f);
			this.Inner.transform.localPosition = vector;
			vector = this.Inner.transform.position;
			vector.z = -26f;
			finger.transform.position = vector;
			return;
		}
		if (this.Inner.gameObject != finger.gameObject)
		{
			this.Inner.transform.localPosition = vector2;
		}
	}

	// Token: 0x04000444 RID: 1092
	public float InnerRadius = 0.64f;

	// Token: 0x04000445 RID: 1093
	public float OuterRadius = 1.28f;

	// Token: 0x04000446 RID: 1094
	public CircleCollider2D Outer;

	// Token: 0x04000447 RID: 1095
	public SpriteRenderer Inner;

	// Token: 0x04000449 RID: 1097
	private Controller myController = new Controller();
}
