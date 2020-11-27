using System;
using UnityEngine;

// Token: 0x0200001F RID: 31
public class AlphaPulse : MonoBehaviour
{
	// Token: 0x06000086 RID: 134 RVA: 0x000026BC File Offset: 0x000008BC
	public void SetColor(Color c)
	{
		this.rend = base.GetComponent<SpriteRenderer>();
		this.baseColor = c;
		this.Update();
	}

	// Token: 0x06000087 RID: 135 RVA: 0x000026D7 File Offset: 0x000008D7
	private void Start()
	{
		this.rend = base.GetComponent<SpriteRenderer>();
	}

	// Token: 0x06000088 RID: 136 RVA: 0x0000D000 File Offset: 0x0000B200
	public void Update()
	{
		float v = Mathf.Abs(Mathf.Cos((this.Offset + Time.time) * 3.14159274f / this.Duration));
		this.rend.color = new Color(this.baseColor.r, this.baseColor.g, this.baseColor.b, this.AlphaRange.Lerp(v));
	}

	// Token: 0x040000BF RID: 191
	public float Offset = 1f;

	// Token: 0x040000C0 RID: 192
	public float Duration = 2.5f;

	// Token: 0x040000C1 RID: 193
	private SpriteRenderer rend;

	// Token: 0x040000C2 RID: 194
	public FloatRange AlphaRange = new FloatRange(0.2f, 0.5f);

	// Token: 0x040000C3 RID: 195
	public Color baseColor = Color.white;
}
