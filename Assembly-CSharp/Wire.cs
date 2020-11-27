using System;
using UnityEngine;

// Token: 0x02000083 RID: 131
public class Wire : MonoBehaviour
{
	// Token: 0x1700007B RID: 123
	// (get) Token: 0x060002C4 RID: 708 RVA: 0x00003CF9 File Offset: 0x00001EF9
	// (set) Token: 0x060002C5 RID: 709 RVA: 0x00003D01 File Offset: 0x00001F01
	public Vector2 BaseWorldPos { get; internal set; }

	// Token: 0x060002C6 RID: 710 RVA: 0x00003D0A File Offset: 0x00001F0A
	public void Start()
	{
		this.BaseWorldPos = base.transform.position;
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x000156B4 File Offset: 0x000138B4
	public void ResetLine(Vector3 targetWorldPos, bool reset = false)
	{
		if (reset)
		{
			this.Liner.transform.localScale = new Vector3(0f, 0f, 0f);
			this.WireTip.transform.eulerAngles = Vector3.zero;
			this.WireTip.transform.position = base.transform.position;
			return;
		}
		Vector2 vector = targetWorldPos - base.transform.position;
		Vector2 normalized = vector.normalized;
		Vector3 localPosition = default(Vector3);
		localPosition = vector - normalized * 0.075f;
		localPosition.z = -0.01f;
		this.WireTip.transform.localPosition = localPosition;
		float magnitude = vector.magnitude;
		this.Liner.transform.localScale = new Vector3(magnitude, 1f, 1f);
		this.Liner.transform.localPosition = vector / 2f;
		this.WireTip.transform.LookAt2d(targetWorldPos);
		this.Liner.transform.localEulerAngles = new Vector3(0f, 0f, Vector2.SignedAngle(Vector2.right, vector));
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x000157F8 File Offset: 0x000139F8
	public void ConnectRight(WireNode node)
	{
		Vector3 position = node.transform.position;
		this.ResetLine(position, false);
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00003D22 File Offset: 0x00001F22
	public void SetColor(Color color)
	{
		this.Liner.material.SetColor("_Color", color);
		this.ColorBase.color = color;
		this.ColorEnd.color = color;
	}

	// Token: 0x040002BE RID: 702
	private const int WireDepth = -14;

	// Token: 0x040002BF RID: 703
	public SpriteRenderer Liner;

	// Token: 0x040002C0 RID: 704
	public SpriteRenderer ColorBase;

	// Token: 0x040002C1 RID: 705
	public SpriteRenderer ColorEnd;

	// Token: 0x040002C2 RID: 706
	public Collider2D hitbox;

	// Token: 0x040002C3 RID: 707
	public SpriteRenderer WireTip;

	// Token: 0x040002C4 RID: 708
	public sbyte WireId;
}
