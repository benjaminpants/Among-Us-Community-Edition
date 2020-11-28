using System;
using UnityEngine;

// Token: 0x02000017 RID: 23
public class DialBehaviour : MonoBehaviour
{
	// Token: 0x06000066 RID: 102 RVA: 0x0000C7C0 File Offset: 0x0000A9C0
	public void Update()
	{
		this.Engaged = false;
		this.myController.Update();
		DragState dragState = this.myController.CheckDrag(this.collider, false);
		if (dragState == DragState.Dragging)
		{
			Vector2 vector = this.myController.DragPosition - (Vector2)base.transform.position;
			float num = Vector2.up.AngleSigned(vector);
			if (num < -180f)
			{
				num += 360f;
			}
			num = this.DialRange.Clamp(num);
			this.SetValue(num);
			this.Engaged = true;
		}
	}

	// Token: 0x06000067 RID: 103 RVA: 0x0000C850 File Offset: 0x0000AA50
	public void SetValue(float angle)
	{
		this.Value = angle;
		Vector3 localEulerAngles = new Vector3(0f, 0f, angle);
		this.DialTrans.localEulerAngles = localEulerAngles;
		this.DialShadTrans.localEulerAngles = localEulerAngles;
	}

	// Token: 0x04000081 RID: 129
	public FloatRange DialRange;

	// Token: 0x04000082 RID: 130
	public Collider2D collider;

	// Token: 0x04000083 RID: 131
	public Controller myController = new Controller();

	// Token: 0x04000084 RID: 132
	public float Value;

	// Token: 0x04000085 RID: 133
	public bool Engaged;

	// Token: 0x04000086 RID: 134
	public Transform DialTrans;

	// Token: 0x04000087 RID: 135
	public Transform DialShadTrans;
}
