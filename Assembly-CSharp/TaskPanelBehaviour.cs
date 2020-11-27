using System;
using UnityEngine;

// Token: 0x0200020D RID: 525
public class TaskPanelBehaviour : MonoBehaviour
{
	// Token: 0x06000B5A RID: 2906 RVA: 0x00038B1C File Offset: 0x00036D1C
	private void Update()
	{
		this.background.transform.localScale = new Vector3(this.TaskText.Width + 0.2f, this.TaskText.Height + 0.2f, 1f);
		Vector3 vector = this.background.sprite.bounds.extents;
		vector.y = -vector.y;
		vector = vector.Mul(this.background.transform.localScale);
		this.background.transform.localPosition = vector;
		Vector3 vector2 = this.tab.sprite.bounds.extents;
		vector2 = vector2.Mul(this.tab.transform.localScale);
		vector2.y = -vector2.y;
		vector2.x += vector.x * 2f;
		this.tab.transform.localPosition = vector2;
		this.ClosedPosition.y = (this.OpenPosition.y = 0.6f);
		this.ClosedPosition.x = -this.background.sprite.bounds.size.x * this.background.transform.localScale.x;
		if (this.open)
		{
			this.timer = Mathf.Min(1f, this.timer + Time.deltaTime / this.Duration);
		}
		else
		{
			this.timer = Mathf.Max(0f, this.timer - Time.deltaTime / this.Duration);
		}
		Vector3 relativePos = new Vector3(Mathf.SmoothStep(this.ClosedPosition.x, this.OpenPosition.x, this.timer), Mathf.SmoothStep(this.ClosedPosition.y, this.OpenPosition.y, this.timer), this.OpenPosition.z);
		base.transform.localPosition = AspectPosition.ComputePosition(AspectPosition.EdgeAlignments.LeftTop, relativePos);
	}

	// Token: 0x06000B5B RID: 2907 RVA: 0x00008D92 File Offset: 0x00006F92
	public void ToggleOpen()
	{
		this.open = !this.open;
	}

	// Token: 0x04000AF2 RID: 2802
	public Vector3 OpenPosition;

	// Token: 0x04000AF3 RID: 2803
	public Vector3 ClosedPosition;

	// Token: 0x04000AF4 RID: 2804
	public SpriteRenderer background;

	// Token: 0x04000AF5 RID: 2805
	public SpriteRenderer tab;

	// Token: 0x04000AF6 RID: 2806
	public TextRenderer TaskText;

	// Token: 0x04000AF7 RID: 2807
	public bool open;

	// Token: 0x04000AF8 RID: 2808
	private float timer;

	// Token: 0x04000AF9 RID: 2809
	public float Duration;
}
