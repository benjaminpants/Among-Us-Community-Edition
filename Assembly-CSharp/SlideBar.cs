using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x020001F9 RID: 505
public class SlideBar : MonoBehaviour
{
	// Token: 0x06000ADD RID: 2781 RVA: 0x0000879E File Offset: 0x0000699E
	public void OnEnable()
	{
		if (this.Title)
		{
			this.Title.Color = Color.white;
		}
		this.Bar.color = Color.white;
		this.Dot.color = Color.white;
	}

	// Token: 0x06000ADE RID: 2782 RVA: 0x000087DD File Offset: 0x000069DD
	public void OnDisable()
	{
		if (this.Title)
		{
			this.Title.Color = Color.gray;
		}
		this.Bar.color = Color.gray;
		this.Dot.color = Color.gray;
	}

	// Token: 0x06000ADF RID: 2783 RVA: 0x00037054 File Offset: 0x00035254
	public void Update()
	{
		Vector3 localPosition = this.Dot.transform.localPosition;
		switch (DestroyableSingleton<PassiveButtonManager>.Instance.Controller.CheckDrag(this.HitBox, false))
		{
		case DragState.Dragging:
		{
			Vector2 vector = DestroyableSingleton<PassiveButtonManager>.Instance.Controller.DragPosition - (Vector2)this.Bar.transform.position;
			if (this.Vertical)
			{
				localPosition.y = this.Range.Clamp(vector.y);
				this.Value = this.Range.ReverseLerp(localPosition.y);
			}
			else
			{
				localPosition.x = this.Range.Clamp(vector.x);
				this.Value = this.Range.ReverseLerp(localPosition.x);
			}
			this.OnValueChange.Invoke();
			break;
		}
		case DragState.Released:
			this.OnValueChange.Invoke();
			break;
		}
		if (this.Vertical)
		{
			localPosition.y = this.Range.Lerp(this.Value);
		}
		else
		{
			localPosition.x = this.Range.Lerp(this.Value);
		}
		this.Dot.transform.localPosition = localPosition;
	}

	// Token: 0x04000A8B RID: 2699
	public TextRenderer Title;

	// Token: 0x04000A8C RID: 2700
	public SpriteRenderer Bar;

	// Token: 0x04000A8D RID: 2701
	public Collider2D HitBox;

	// Token: 0x04000A8E RID: 2702
	public SpriteRenderer Dot;

	// Token: 0x04000A8F RID: 2703
	public FloatRange Range;

	// Token: 0x04000A90 RID: 2704
	public bool Vertical;

	// Token: 0x04000A91 RID: 2705
	public float Value;

	// Token: 0x04000A92 RID: 2706
	public UnityEvent OnValueChange;
}
