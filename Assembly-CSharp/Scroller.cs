using System;
using UnityEngine;

// Token: 0x020000FF RID: 255
public class Scroller : MonoBehaviour
{
	// Token: 0x170000D2 RID: 210
	// (get) Token: 0x0600056D RID: 1389 RVA: 0x000056DC File Offset: 0x000038DC
	public bool AtTop
	{
		get
		{
			return this.Inner.localPosition.y <= this.YBounds.min + 0.25f;
		}
	}

	// Token: 0x170000D3 RID: 211
	// (get) Token: 0x0600056E RID: 1390 RVA: 0x00005704 File Offset: 0x00003904
	public bool AtBottom
	{
		get
		{
			return this.Inner.localPosition.y >= this.YBounds.max - 0.25f;
		}
	}

	// Token: 0x0600056F RID: 1391 RVA: 0x0002309C File Offset: 0x0002129C
	public void FixedUpdate()
	{
		if (!this.Inner)
		{
			return;
		}
		Vector2 mouseScrollDelta = Input.mouseScrollDelta;
		mouseScrollDelta.y = -mouseScrollDelta.y;
		this.DoScroll(this.Inner.transform.localPosition, mouseScrollDelta);
		this.myController.Update();
		DragState dragState = this.myController.CheckDrag(this.HitBox, false);
		if (dragState == DragState.TouchStart)
		{
			this.origin = this.Inner.transform.localPosition;
			return;
		}
		if (dragState != DragState.Dragging)
		{
			return;
		}
		Vector2 del = this.myController.DragPosition - this.myController.DragStartPosition;
		this.DoScroll(this.origin, del);
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x0002314C File Offset: 0x0002134C
	private void DoScroll(Vector3 origin, Vector2 del)
	{
		if (del.magnitude < 0.05f)
		{
			return;
		}
		if (!this.allowX)
		{
			del.x = 0f;
		}
		if (!this.allowY)
		{
			del.y = 0f;
		}
		Vector3 vector = origin + (Vector3)del;
		vector.x = this.XBounds.Clamp(vector.x);
		int childCount = this.Inner.transform.childCount;
		float max = Mathf.Max(this.YBounds.min, this.YBounds.max + this.YBoundPerItem * (float)childCount);
		vector.y = Mathf.Clamp(vector.y, this.YBounds.min, max);
		this.Inner.transform.localPosition = vector;
	}

	// Token: 0x04000535 RID: 1333
	public Transform Inner;

	// Token: 0x04000536 RID: 1334
	public Collider2D HitBox;

	// Token: 0x04000537 RID: 1335
	private Controller myController = new Controller();

	// Token: 0x04000538 RID: 1336
	private Vector3 origin;

	// Token: 0x04000539 RID: 1337
	public bool allowX;

	// Token: 0x0400053A RID: 1338
	public FloatRange XBounds = new FloatRange(-10f, 10f);

	// Token: 0x0400053B RID: 1339
	public bool allowY;

	// Token: 0x0400053C RID: 1340
	public FloatRange YBounds = new FloatRange(-10f, 10f);

	// Token: 0x0400053D RID: 1341
	public float YBoundPerItem;
}
